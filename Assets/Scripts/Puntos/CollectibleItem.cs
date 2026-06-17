using UnityEngine;
using Unity.Netcode;

public class CollectibleItem : NetworkBehaviour
{
    public int itemValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.AddItems(itemValue);

                // Desaparece de la red para todos a la vez
                GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
            }
        }
    }
}