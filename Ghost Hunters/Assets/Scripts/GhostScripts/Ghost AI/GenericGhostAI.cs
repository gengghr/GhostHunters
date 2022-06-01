using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GenericGhostAI : MonoBehaviour
{
    public GameObject[] players;
    public NavMeshAgent ghostagent;

    public float Aggression = 100;
    public bool Visible = true;
    public float WanderDistance = 10f;
    public float MinWanderInterval = 5f;
    public float MaxWanderInterval = 10f; // In seconds
    private float WanderTime;
    public string WinScene = "Won";

    NavMeshPath trypath;

    public List<GameObject> boundItems = new List<GameObject>();


    private GameObject singlePlayer;
    public GameObject gameController;

    public Animator animator;
    private int AniIDDistance;//the address of distance in Animator
    private int AniGhostDie;//the address of Die in Animator
    private bool DieAnimationLock;//a locker to make sure the animation will only play once
    bool ghostDead;
    float timer;
    bool hunting = false;
    // Start is called before the first frame update
    void Start()
    {
        // Setup variables
        players = GameObject.FindGameObjectsWithTag("Player");
        singlePlayer = GameObject.FindGameObjectWithTag("Player");
        ghostagent = GetComponent<NavMeshAgent>();

        WanderTime = MaxWanderInterval;

        trypath = new NavMeshPath();

        AniIDDistance = Animator.StringToHash("Distance");
        AniGhostDie = Animator.StringToHash("Die");
        DieAnimationLock = false;
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0];
        ghostDead = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (!ghostDead)
        {
            // update the ghost animation
            animator.SetFloat(AniIDDistance, getPlayerDistance());
            // aggression 0-100 - wander around
            if (Aggression < 100)
            {
                WanderFrame();
                hunting = false;
            }
            else
            { // aggression 100+ - hunt players
                if (!hunting)
                {
                    // At the start of the hunt, the ghost knows the player's position
                    hunting = true;
                    FollowingPlayer = players[0];
                    LastSeen = players[0].transform.position;
                    hasTarget = true;
                }
                FollowFrame();
            }

            // Check if the bound objects have been destroyed
            ghostDead = true;
            foreach (GameObject bo in boundItems)
            {

                if (bo.activeSelf)
                {
                    ghostDead = false;
                }
            }
        }

        // If the ghost died, start the death animation
        else if (ghostDead)
        {
            timer += Time.deltaTime;
            if (!DieAnimationLock)
            {
                animator.SetBool(AniGhostDie, true);
                DieAnimationLock = true;
                gameController.GetComponent<Aggression>().ghostAlive = false;
                gameController.GetComponent<Aggression>().renderGhost = true;
                if (gameController.GetComponent<Aggression>().AggressionLevel >= 100)
                {
                    gameController.GetComponent<Aggression>().stopHunt();
                }

            }
            if (timer >= 10)
            {
                // 10 seconds after the ghost dies, send the player to the win screen
                SceneManager.LoadScene(WinScene);
            }
        }


    }

    // A frame in the wander loop
    private void WanderFrame()
    {
        // Only pick a new position every so often
        WanderTime -= Time.deltaTime;
        // When ready, try to pick a random position
        // Will try again in the next iteration if the position isn't valid
        if (WanderTime <= 0 && TryRandomPosition(out Vector3 target))
        {
            WanderTime = Random.Range(MinWanderInterval, MaxWanderInterval);
            ghostagent.SetDestination(target);
        }
    }

    // A frame in the follow loop
    private void FollowFrame()
    {
        // Check if the ghost has a visible target
        Vector3 target = GetPlayerTargetLOS();
        if (hasTarget)
            ghostagent.SetDestination(target);
        else
            // If not, wander around
            WanderFrame();
    }

    // A debug method that shows the ghost's destination in the editor
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ghostagent.destination, 0.25f);
        }
    }

    // Returns a position for a targeted player
    // If no target can be found, returns positive infinity
    private GameObject FollowingPlayer = null;
    private bool hasTarget = false; // False if the ghost doesn't have a target to folllow (since LastSeen isn't nullable)
    private Vector3 LastSeen = Vector3.zero;
    private Vector3 GetPlayerTargetLOS()
    {
        // Check if the target can still be seen
        if (FollowingPlayer != null)
        {
            if (CheckLineOfSight(FollowingPlayer))
            {
                LastSeen = FollowingPlayer.transform.position;
                hasTarget = true;
                return LastSeen;
            }

        }

        // Check if the target's last known location has been reached
        if (hasTarget)
        {
            if ((LastSeen - ghostagent.transform.position).magnitude > ghostagent.stoppingDistance)
                return LastSeen;
            else
            {
                hasTarget = false;
            }

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

        // IF here, a valid target could not be found
        return Vector3.positiveInfinity;
    }

    // A method to check if the ghost can see a player
    // These parameters are set in the scene
    public float ghostTopOffset = 0f;
    public float playerTopOffset = 0f;
    private bool CheckLineOfSight(GameObject target)
    {
        // Get the top of the ghost's and player's heads
        Vector3 ghosttop = transform.position + new Vector3(0f, ghostTopOffset, 0f);
        Vector3 playertop = target.transform.position + new Vector3(0f, playerTopOffset, 0f);
        // Calculate info for raycast
        Vector3 heading = playertop - ghosttop;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ghosttop, direction, out hitInfo, distance);
        if (hit && hitInfo.transform == target.transform)
        {
            // If raycast from the ghost hits the player, returns true
            // Also draws a line with the result in the debugger
            Debug.DrawLine(ghosttop, playertop, Color.red);
            return true;
        }
        else
        {
            // If raycast is not successful, the ghost does not see the player
            Debug.DrawLine(ghosttop, playertop, Color.white);
            return false;
        }

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

    // Retrieves the distance between the player and the ghost
    private float getPlayerDistance()
    {
        return Vector3.Distance(singlePlayer.transform.position, transform.position);
    }

    //disable the ghost after *time seconds
    private IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        gameObject.SetActive(false);
    }

   
}
