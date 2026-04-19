using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI winsText;

    private void Start()
    {
        string winnerName = DatabaseManager.Instance.LastWinnerName;
        winnerText.text = $"{winnerName} Wins!";

        DatabaseManager.Instance.AddWin(winnerName);
        int totalWins = DatabaseManager.Instance.GetWins(winnerName);
        winsText.text = $"{winnerName} has {totalWins} total wins";

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
    }

    public void GoToMainMenu()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenuScene");
    }
}