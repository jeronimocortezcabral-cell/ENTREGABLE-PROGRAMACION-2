using Unity.Netcode;
using UnityEngine;

public class ItemDespawn : NetworkBehaviour
{
    [Header("Tiempo de Vida")]
    [SerializeField] private float lifetime = 0.5f; // Se destruye a los 0,5 segundos

    public override void OnNetworkSpawn()
    {
        // REGLA: Solo el servidor/host maneja el ciclo de vida de los objetos de red
        if (!IsServer) return;

        // Invoca la función de autodestrucción cuando pase el tiempo establecido
        Invoke(nameof(DespawnMe), lifetime);
    }

    private void DespawnMe()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            // Despawn(true) borra el objeto de la red Y lo destruye de la escena
            NetworkObject.Despawn(true);
        }
    }
}