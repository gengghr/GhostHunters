using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Samples;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetGenericGhostBehavior : NetworkBehaviour
{

    public List<GameObject> players;
    public NavMeshAgent ghostagent;

    public float Aggression = 100;
    public bool Visible = true;
    public float WanderDistance = 10f;
    public float MinWanderInterval = 5f;
    public float MaxWanderInterval = 10f; // In seconds
    private float WanderTime;
    float closestDist;

    private int AniIDDistance;//the address of distance in Animator
    private int AniGhostDie;//the address of Die in Animator
    private bool DieAnimationLock;//a locker to make sure animation will only play once
    public Animator animator;
    bool ghostDead;
    float timer;

    public GameObject gameController;

    NavMeshPath trypath;

    public List<GameObject> boundItems = new List<GameObject>();
    private NetworkVariable<bool> netGhostDead = new NetworkVariable<bool>();
    private NetworkVariable<float> netGhostDis = new NetworkVariable<float>();
    bool hunting = false;

    // Start is called before the first frame update
    void Start()
    {
        // Setup variables
        players = PlayerControl.playerlist;
        ghostagent = GetComponent<NavMeshAgent>();

        WanderTime = MaxWanderInterval;

        trypath = new NavMeshPath();

        AniIDDistance = Animator.StringToHash("Distance");
        AniGhostDie = Animator.StringToHash("Die");
        DieAnimationLock = false;
        ghostDead = false;
        timer = 0;
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Behaviors are calculated by the server
        if (IsServer)
        {

            if (!ghostDead)
            {
                // Detect if the bound objects are burnt,
                // killing the ghost
                if (boundItems.Count > 0)
                {
                    bool ghostDead = true;
                    foreach (GameObject bo in boundItems)
                    {
                        if (bo.activeSelf)
                        {
                            ghostDead = false;
                        }
                    }
                    // Let the client know the results
                    if (ghostDead)
                    {
                        netGhostDead.Value = true;
                    }
                    else
                    {
                        netGhostDis.Value = (this.transform.position - getClosestPlayer().transform.position).magnitude;
                    }
                }

                // Update the animator
                animator.SetFloat(AniIDDistance, closestDist);

                // aggression 0-100 - wander
                if (Aggression < 100)
                {
                    hunting = false;
                    WanderFrame();
                }

                // aggression 100+ - follow players and hunt them
                else
                {
                    // At start of hunt, learns the location of the closest player
                    if (!hunting)
                    {
                        hunting = true;
                        FollowingPlayer = getClosestPlayer();
                        LastSeen = FollowingPlayer.transform.position;
                        hasTarget = true;
                    }
                    FollowFrame();


                }
            }
            else
            {
                // If the ghost is dead, do the death animation
                timer += Time.deltaTime;
                if (!DieAnimationLock)
                {
                    animator.SetBool(AniGhostDie, true);
                    DieAnimationLock = true;
                    gameController.GetComponent<NetAggression>().ghostDead();
                    gameController.GetComponent<NetAggression>().RenderGhostClientRpc(true);
                    if (gameController.GetComponent<NetAggression>().AggressionLevel >= 100)
                    {
                        gameController.GetComponent<NetAggression>().EndHuntingClientRpc();
                    }

                }
                if (timer >= 10)
                {
                    //SceneManager.LoadScene("Won");
                }
            }
        }
        // Client-side functions
        if (IsClient)
        {
            // If the ghost is dead, start the death animation
            if (netGhostDead.Value)
            {
                animator.SetBool(AniGhostDie, true);
                timer += Time.deltaTime;
                if (timer >= 10)
                {
                    // Send the player to the win screen after 10 seconds
                    SceneManager.LoadScene("Won");
                }
            }
            else
            {
                // Update the client-side animator
                animator.SetFloat(AniIDDistance, netGhostDis.Value);
            }
        }

    }

    // A frame in the wander loop
    private void WanderFrame()
    {
        // Only pick a new position every so often
        WanderTime -= Time.deltaTime;
        // When ready, try picking a valid destination
        // If it fails, it will try again at the next iteration
        if (WanderTime <= 0 && TryRandomPosition(out Vector3 target))
        {
            WanderTime = Random.Range(MinWanderInterval, MaxWanderInterval);
            ghostagent.SetDestination(target);
        }
    }

    // A frame in the follow loop
    private void FollowFrame()
    {
        // Check to see if there's a valid target
        Vector3 target = GetPlayerTargetLOS();
        if (hasTarget)
        {
            ghostagent.SetDestination(target);
        }
        else
        {
            // If not, start wandering
            WanderFrame();
        }

    }

    // Returns a position for a targeted player
    // If no target can be found, returns positive infinity
    private GameObject FollowingPlayer = null;
    private bool hasTarget = false; // False if the ghost doesn't have a target to folllow (since LastSeen isn't nullable)
    private Vector3 LastSeen = Vector3.zero;
    private Vector3 GetPlayerTargetLOS()
    {
        // Check if the target can still be seen by the ghost
        if (FollowingPlayer != null)
        {
            if (CheckLineOfSight(FollowingPlayer))
            {
                LastSeen = FollowingPlayer.transform.position;
                hasTarget = true;
                return LastSeen;
            }

        }

        // Check if the last known location has been reached
        if (hasTarget)
        {
            if ((LastSeen - ghostagent.transform.position).magnitude > ghostagent.stoppingDistance)
                return LastSeen;
            else
                hasTarget = false;
        }

        // If not pursuing a player, pick a target
        if (!hasTarget)
        {
            // picks closest line-of-sight player
            float curdist = 0f;
            FollowingPlayer = null;
            foreach (GameObject p in players)
            {
                Vector3 heading = p.transform.position - ghostagent.transform.position;
                float distance = heading.magnitude;
                //Debug.Log(CheckLineOfSight(p).ToString());
                if (CheckLineOfSight(p) && (FollowingPlayer == null || distance < curdist))
                {
                    FollowingPlayer = p;
                    LastSeen = p.transform.position;
                    curdist = distance;
                    hasTarget = true;
                }
            }
            if (FollowingPlayer != null)
                return LastSeen;
        }

        // If here, a valid target could not be found
        return Vector3.positiveInfinity;

    }

    // A method to check if the ghost can see the target player
    // These parameters are set in the scene
    public float ghostTopOffset = 0f;
    public float playerTopOffset = 0f;
    private bool CheckLineOfSight(GameObject target)
    {
        // Get the position of the top of their heads
        Vector3 ghosttop = transform.position + new Vector3(0f, ghostTopOffset, 0f);
        Vector3 playertop = target.transform.position + new Vector3(0f, playerTopOffset, 0f);
        // Calculate stuff for the raycast
        Vector3 heading = playertop - ghosttop;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        RaycastHit hitInfo;

        bool hit = Physics.Raycast(ghosttop, direction, out hitInfo, distance);
        if (hit && hitInfo.transform == target.transform)
        {
            // If the raycast hits the player, returns true
            // Also draws a line with the result in the editor
            Debug.DrawLine(ghosttop, playertop, Color.red);
            return true;
        }
        else
        {
            // IF the raycast doesn't hit the player, return false
            Debug.DrawLine(ghosttop, playertop, Color.white);
            return false;
        }

    }

    // Returns the the closest player
    // returns null if there are no players
    private GameObject getClosestPlayer()
    {
        float closestDist = -1;
        GameObject closest = null;
        foreach (GameObject p in players)
        {
            float dist = (p.transform.position - this.transform.position).magnitude;
            if (closestDist == -1 || dist < closestDist)
            {
                closestDist = dist;
                closest = p;
            }
        }
        return closest;
    }

    // Samples a random position close to the Agent
    // Returns true if the position is on the nav mesh
    // Otherwise, returns false (out target becomes (0,0,0))
    private bool TryRandomPosition(out Vector3 target)
    {
        // Get a random direction and length
        Vector3 dir = Random.insideUnitSphere;
        dir *= WanderDistance;
        // Offset by world coordinates to get target
        target = new Vector3(dir.x, dir.y, dir.z) + transform.position;
        // Check if it's close enough to the nav mesh
        // This is to prevent bias towards pathing into walls
        NavMeshHit hit;
        NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas);
        if (!hit.hit)
        {
            target = Vector3.zero;
            return false;
        }

        // Check if the length of the path isn't too large
        // This is to prevent weirdly long paths where the ghost
        // just goes to the same spot but on a different floor
        NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, trypath);
        float dist = CalcPathDistance(transform.position, trypath);
        if (dist > WanderDistance)
        {
            target = Vector3.zero;
            return false;
        }

        // If this point is reached, a good path has been found
        target = hit.position;
        return true;
    }


    // Given an AI.NavMeshPath, calculates the total length
    // If the path is incomplete, returns positive infinity
    private float CalcPathDistance(Vector3 start, NavMeshPath path)
    {
        if (path.status != NavMeshPathStatus.PathComplete)
            return float.PositiveInfinity;

        float dist = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            dist += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return dist;
    }

    // Client-side command to disable/enable the ghost
    [ClientRpc]
    public void SetActiveClientRpc(bool v)
    {
        gameObject.SetActive(false);
    }

}
