using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject scene1Button;
    [SerializeField] private GameObject scene2Button;

    private void Start()
    {
        if (scene1Button != null)
            scene1Button.SetActive(false);
        if (scene2Button != null)
            scene2Button.SetActive(false);
    }

    public void OnHostClicked()
    {
        Debug.Log("Host clicked");
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("No NetworkManager found!");
            return;
        }
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777);
        NetworkManager.Singleton.StartHost();
        if (scene1Button != null)
            scene1Button.SetActive(true);
        if (scene2Button != null)
            scene2Button.SetActive(true);
    }

    public void OnClientClicked()
    {
        Debug.Log("Client clicked");
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("No NetworkManager found!");
            return;
        }
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777);
        NetworkManager.Singleton.StartClient();
    }

    public void OnScene1Clicked()
    {
        Debug.Log("Scene 1 clicked");
        if (!NetworkManager.Singleton.IsHost) return;
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        NetworkManager.Singleton.SceneManager.LoadScene("DuelScene", LoadSceneMode.Single);
    }

    public void OnScene2Clicked()
    {
        Debug.Log("Scene 2 clicked");
        if (!NetworkManager.Singleton.IsHost) return;
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        NetworkManager.Singleton.SceneManager.LoadScene("DuelScene2", LoadSceneMode.Single);
    }
}