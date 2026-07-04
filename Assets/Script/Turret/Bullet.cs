using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    private Rigidbody2D rb;
    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }




    void Update()
    {
        FarToDestory(); //The bullet detonates after traveling a certain distance.
    }

    public void SetSpeed(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }


    public virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }


    public virtual void FarToDestory() // Bullet auto-destroys after traveling a certain distance from the player without hitting any target
    {
        float distanceX = Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x);
        float distanceY = Mathf.Abs(GameManager.instance.player.transform.position.y - transform.position.y);
        if (distanceX > 40 || distanceY > 22)
        {
            GameObject explosion = PoolManager.instance.Get("Explosion");
            explosion.transform.position = transform.position;
            gameObject.SetActive(false);
        }

    }
}
