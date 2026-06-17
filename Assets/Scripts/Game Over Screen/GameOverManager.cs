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
        // 1. Mostramos los puntajes de la partida que acaba de terminar
        if (finalScoreText != null)
        {
            finalScoreText.text = $"{GameManager.WinnerText}\n\n" +
                                  $"Jugador 1 (Host): {GameManager.HostFinalScore:F1} pts\n" +
                                  $"Jugador 2 (Cliente): {GameManager.ClientFinalScore:F1} pts";
        }

        // 2. Liberamos y mostramos el cursor para que puedan interactuar con la UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolverAlMenu()
    {
        Debug.Log("🔌 [GAME OVER] Apagando Netcode y regresando al menú principal...");

        // 3. Apagamos Netcode por completo. Esto desconecta al cliente,
        // cierra el servidor/host y limpia todos los objetos de red de la memoria.
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // 4. Cargamos la escena del menú de forma tradicional
        SceneManager.LoadScene("MainMenu");
    }
}