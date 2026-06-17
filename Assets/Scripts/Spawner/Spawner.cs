using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject itemPrefab; // El prefab de tus Puntos
    [SerializeField] private float spawnInterval = 3f; // Cada cuántos segundos cae uno
    [SerializeField] private Vector3 spawnRange = new Vector3(10f, 0f, 10f); // Área de lluvia

    // Se ejecuta automáticamente cuando el objeto se inicializa en la red
    public override void OnNetworkSpawn()
    {
        // REGLA DE ORO: Solo el Servidor/Host tiene permitido spawnear objetos de red
        if (!IsServer) return;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("¡Falta asignar el ItemPrefab en el Inspector del Spawner!");
            return;
        }

        // Calcular una posición aleatoria alrededor del Spawner
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            spawnRange.y,
            Random.Range(-spawnRange.z, spawnRange.z)
        ) + transform.position;

        // 1. Instanciar el objeto localmente en el servidor
        GameObject spawnedItem = Instantiate(itemPrefab, randomPos, Quaternion.identity);

        // 2. 🔥 REGLA NETCODE: Sincronizarlo para que aparezca en las pantallas de todos los clientes
        NetworkObject netObj = spawnedItem.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogError("¡El prefab que pusiste en el Spawner NO tiene un componente NetworkObject!");
        }
    }
}