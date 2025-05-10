using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        restartButton.onClick.AddListener(() => gameManager.RestartField());

        gameManager.OnWin += ShowWinScreen;
        gameManager.OnLose += ShowLoseScreen;
    }

    private void OnDestroy()
    {
        gameManager.OnWin -= ShowWinScreen;
        gameManager.OnLose -= ShowLoseScreen;
    }

    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }

    private void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
    }
}