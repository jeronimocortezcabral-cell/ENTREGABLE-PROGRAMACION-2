using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 🔥 NECESARIO PARA COMPLEMENTAR LAS ESCENAS

public class MainMenu : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private GameObject menuPrincipalPanel;
    [SerializeField] private GameObject pantallaClientePanel;

    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaJuego = "Juego"; // 🔥 Escribí acá el nombre exacto de tu escena de juego

    void Start()
    {
        if (ipInputField != null)
        {
            ipInputField.text = "127.0.0.1";
        }

        if (menuPrincipalPanel != null) menuPrincipalPanel.SetActive(true);
        if (pantallaClientePanel != null) pantallaClientePanel.SetActive(false);
    }

    // Función para el botón de HOST
    public void IniciarHost()
    {
        // 1. Iniciamos el Host
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("🌐 Host iniciado correctamente. Cambiando de escena...");

            // 2. 🔥 REGLA DE ORO DE NETCODE: El servidor cambia de escena usando su propio SceneManager.
            // Esto hace que la escena cambie para el Host Y para cualquier cliente que se conecte después.
            NetworkManager.Singleton.SceneManager.LoadScene(nombreEscenaJuego, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("🚨 No se pudo iniciar el Host.");
        }

        ApagarTodoElMenu();
    }

    public void AbrirPantallaCliente()
    {
        if (menuPrincipalPanel != null) menuPrincipalPanel.SetActive(false);
        if (pantallaClientePanel != null) pantallaClientePanel.SetActive(true);
    }

    public void VolverAlMenuPrincipal()
    {
        if (menuPrincipalPanel != null) menuPrincipalPanel.SetActive(true);
        if (pantallaClientePanel != null) pantallaClientePanel.SetActive(false);
    }

    public void ConectarComoCliente()
    {
        string ipIngresada = ipInputField.text.Trim();

        if (string.IsNullOrEmpty(ipIngresada))
        {
            Debug.LogError("🚨 ¡Escribí una IP antes de conectar!");
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = ipIngresada;
        }

        // El cliente solo avisa que se conecta. El servidor se encarga de mandarlo a la escena correcta automáticamente
        NetworkManager.Singleton.StartClient();
        ApagarTodoElMenu();
    }

    private void ApagarTodoElMenu()
    {
        if (menuPrincipalPanel != null) menuPrincipalPanel.SetActive(false);
        if (pantallaClientePanel != null) pantallaClientePanel.SetActive(false);
    }
}