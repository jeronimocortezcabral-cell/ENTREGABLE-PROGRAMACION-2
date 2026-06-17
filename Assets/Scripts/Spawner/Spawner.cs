using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject itemPrefab; 
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector3 spawnRange = new Vector3(10f, 0f, 10f); 

    public override void OnNetworkSpawn()
    {
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

        Vector3 randomPos = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            spawnRange.y,
            Random.Range(-spawnRange.z, spawnRange.z)
        ) + transform.position;

        GameObject spawnedItem = Instantiate(itemPrefab, randomPos, Quaternion.identity);

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