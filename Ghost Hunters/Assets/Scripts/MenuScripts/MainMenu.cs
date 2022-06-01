using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
  [Header("References")]
  [SerializeField]
  private InputField displayNameInputField;
  [SerializeField]
  private InputField joinCodeField;
  [SerializeField]
  private GameObject Nameinput;

  public static MainMenu instance;

  private string clientName;

  private void Start()
  {
    PlayerPrefs.GetString("PlayerName");
    instance = this;
  }

  public async void OnHostClickedAsync()
  {
    PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

    await GameNetPortal.Instance.StartHostAsync();
  }

  public async void OnClientClickedAsync()
  {
    PlayerPrefs.SetString("PlayerName", clientName);

    await ClientGameNetPortal.Instance.StartClientAsync(JoinCode.instance.joinCode);
  }

  public void getClientName()
  {
    clientName = displayNameInputField.text;
    Nameinput.SetActive(false);
  }

  public void startVRGame()
  {
    SceneManager.LoadScene("VR");
  }

  public void startSingleGame()
  {
    SceneManager.LoadScene("Show");
  }

  public void startTutorial()
  {
    SceneManager.LoadScene("Tutorial");
  }
}
