using System.Collections.Generic;
using UnityEngine;

public class RandomCoinSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform vehicle;
    public Coin coinPrefab;

    [Header("Spawn Area")]
    public float spawnAheadDistance = 60f;
    public float recycleBehindDistance = 30f;

    [Header("Spawn Random Settings")]
    public Vector2 xGapRange = new Vector2(3f, 7f);
    public Vector2 yRange = new Vector2(-1f, 4f);

    [Header("Pool Settings")]
    public int maxActiveCoins = 30;

    private float nextSpawnX;
    private readonly Queue<Coin> coinPool = new Queue<Coin>();
    private readonly List<Coin> activeCoins = new List<Coin>();

    private void Start()
    {
        if (vehicle == null)
        {
            Debug.LogError("RandomCoinSpawner: Vehicle is not assigned.");
            return;
        }

        if (coinPrefab == null)
        {
            Debug.LogError("RandomCoinSpawner: Coin prefab is not assigned.");
            return;
        }

        nextSpawnX = vehicle.position.x + 10f;

        SpawnInitialCoins();
    }

    private void Update()
    {
        if (vehicle == null || coinPrefab == null)
            return;

        while (nextSpawnX < vehicle.position.x + spawnAheadDistance)
        {
            SpawnCoin();
        }

        RecycleOldCoins();
    }

    private void SpawnInitialCoins()
    {
        while (nextSpawnX < vehicle.position.x + spawnAheadDistance)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        if (activeCoins.Count >= maxActiveCoins)
            return;

        float gap = Random.Range(xGapRange.x, xGapRange.y);
        nextSpawnX += gap;

        float randomY = Random.Range(yRange.x, yRange.y);

        Coin coin = GetCoinFromPool();
        coin.transform.position = new Vector3(nextSpawnX, randomY, 0f);
        coin.transform.rotation = Quaternion.identity;
        coin.gameObject.SetActive(true);
        coin.Init(this);

        activeCoins.Add(coin);
    }

    private Coin GetCoinFromPool()
    {
        if (coinPool.Count > 0)
        {
            return coinPool.Dequeue();
        }

        Coin newCoin = Instantiate(coinPrefab);
        return newCoin;
    }

    public void RecycleCoin(Coin coin)
    {
        if (coin == null)
            return;

        activeCoins.Remove(coin);
        coin.gameObject.SetActive(false);
        coinPool.Enqueue(coin);
    }

    private void RecycleOldCoins()
    {
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            Coin coin = activeCoins[i];

            if (coin.transform.position.x < vehicle.position.x - recycleBehindDistance)
            {
                RecycleCoin(coin);
            }
        }
    }
}