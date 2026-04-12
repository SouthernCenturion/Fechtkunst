using Unity.Netcode;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    private void Start()
    {
        // Subscribe to score changes
        GameManager.Instance.Player1Score.OnValueChanged += UpdatePlayer1Score;
        GameManager.Instance.Player2Score.OnValueChanged += UpdatePlayer2Score;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Player1Score.OnValueChanged -= UpdatePlayer1Score;
            GameManager.Instance.Player2Score.OnValueChanged -= UpdatePlayer2Score;
        }
    }

    private void UpdatePlayer1Score(int oldValue, int newValue)
    {
        player1ScoreText.text = $"P1: {newValue}";
    }

    private void UpdatePlayer2Score(int oldValue, int newValue)
    {
        player2ScoreText.text = $"P2: {newValue}";
    }
}