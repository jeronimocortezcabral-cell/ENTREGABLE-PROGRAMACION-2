using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameOverManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI finalScoreText;

    private void Start()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"{GameManager.WinnerText}\n\n" +
                                  $"Jugador 1 (Host): {GameManager.HostFinalScore:F1} pts\n" +
                                  $"Jugador 2 (Cliente): {GameManager.ClientFinalScore:F1} pts";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolverAlMenu()
    {
        Debug.Log("🔌 [GAME OVER] Apagando Netcode y regresando al menú principal...");

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }
}