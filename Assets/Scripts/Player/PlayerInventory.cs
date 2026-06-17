using UnityEngine;
using Unity.Netcode;

public class PlayerInventory : NetworkBehaviour
{
    [Header("Inventario Temporal Sincronizado")]
    public NetworkVariable<int> carriedItems = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void AddItems(int amount)
    {
        if (IsServer)
        {
            carriedItems.Value += amount;
            Debug.Log("Items en mochila (Server): " + carriedItems.Value);
        }
    }
}