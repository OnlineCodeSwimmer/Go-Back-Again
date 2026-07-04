using UnityEngine;

public class VehicleCoinCollector : MonoBehaviour
{
    [Header("Coin Count")]
    public int coinCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Coin coin = other.GetComponent<Coin>();

        if (coin != null)
        {
            coin.Collect(this);
        }
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("Coin Count: " + coinCount);
    }
}