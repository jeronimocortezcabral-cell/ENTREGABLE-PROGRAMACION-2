using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [Header("Configuración del Tiempo")]
    public float maxTime = 60f;
    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>(60f);
    private bool isGameOver = false;

    [Header("Puntajes Competitivos Sincronizados")]
    public NetworkVariable<float> hostScore = new NetworkVariable<float>(0f);
    public NetworkVariable<float> clientScore = new NetworkVariable<float>(0f);

    [Header("Referencias de la UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pointsText;

    // Variables estáticas para exportar los resultados a la pantalla de Game Over
    public static string WinnerText = "";
    public static float HostFinalScore = 0f;
    public static float ClientFinalScore = 0f;

    private void Start()
    {
        Time.timeScale = 1f;

        if (IsServer)
        {
            timeRemaining.Value = maxTime;
            hostScore.Value = 0f;
            clientScore.Value = 0f;
        }
    }

    private void Update()
    {
        if (IsServer && !isGameOver)
        {
            if (timeRemaining.Value > 0)
            {
                timeRemaining.Value -= Time.deltaTime;
            }
            else
            {
                timeRemaining.Value = 0;
                isGameOver = true;

                // COMPARACIÓN DE PUNTAJES EN EL SERVIDOR
                string winnerMessage = "";
                if (hostScore.Value > clientScore.Value)
                {
                    winnerMessage = "¡GANÓ EL JUGADOR 1 (HOST)!";
                }
                else if (clientScore.Value > hostScore.Value)
                {
                    winnerMessage = "¡GANÓ EL JUGADOR 2 (CLIENTE)!";
                }
                else
                {
                    winnerMessage = "¡HUBO UN EMPATE!";
                }

                // Mandamos el veredicto y los puntajes finales a todos los clientes
                ExecuteGameOverClientRpc(winnerMessage, hostScore.Value, clientScore.Value);
            }
        }

        UpdateTimerUI();
        UpdatePointsUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining.Value / 60);
        int seconds = Mathf.FloorToInt(timeRemaining.Value % 60);
        timerText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
    }

    void UpdatePointsUI()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.LocalClient != null)
        {
            var localPlayerObj = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (localPlayerObj != null)
            {
                PlayerInventory inventory = localPlayerObj.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    pointsText.text = $"En mochila: {inventory.carriedItems.Value}\nJ1 (Host): {hostScore.Value:F1} | J2 (Cliente): {clientScore.Value:F1}";
                    return;
                }
            }
        }
        pointsText.text = $"J1 (Host): {hostScore.Value:F1} | J2 (Cliente): {clientScore.Value:F1}";
    }

    public void DepositItems(int amount, ulong playerId)
    {
        if (!IsServer) return;

        float pointsGained = amount * 1.25f;

        if (playerId == NetworkManager.ServerClientId)
        {
            hostScore.Value += pointsGained;
        }
        else
        {
            clientScore.Value += pointsGained;
        }
    }

    [ClientRpc]
    void ExecuteGameOverClientRpc(string winner, float hostPts, float clientPts)
    {
        WinnerText = winner;
        HostFinalScore = hostPts;
        ClientFinalScore = clientPts;

        // 🍏 SOLUCIÓN: No apagamos la red. El Host usa Netcode para mudar a todos en sincronía.
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }
}