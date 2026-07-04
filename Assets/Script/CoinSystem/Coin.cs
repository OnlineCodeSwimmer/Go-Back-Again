using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int value = 1;
    public float rotateSpeed = 180f;

    private RandomCoinSpawner spawner;

    public void Init(RandomCoinSpawner owner)
    {
        spawner = owner;
    }

    private void Update()
    {
        // 简单旋转效果，不需要可以删掉
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    public void Collect(VehicleCoinCollector collector)
    {
        collector.AddCoin(value);

        if (spawner != null)
        {
            spawner.RecycleCoin(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}