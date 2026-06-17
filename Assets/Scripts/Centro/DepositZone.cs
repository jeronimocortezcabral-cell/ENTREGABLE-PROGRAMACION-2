using UnityEngine;
using Unity.Netcode;

public class DepositZone : NetworkBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = Object.FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            NetworkObject playerNetObj = other.GetComponent<NetworkObject>(); // Buscamos su identidad de red

            if (inventory != null && playerNetObj != null && inventory.carriedItems.Value > 0)
            {
                int itemsToDeposit = inventory.carriedItems.Value;
                inventory.carriedItems.Value = 0;

                if (gameManager != null)
                {
                    // LE PASAMOS LOS PUNTOS Y EL ID DEL DUEÑO
                    gameManager.DepositItems(itemsToDeposit, playerNetObj.OwnerClientId);
                }
            }
        }
    }
}