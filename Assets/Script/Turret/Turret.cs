using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    //Time Varible
    private float shootTimer=0f;

    //Component
    private Transform muzzle;


    private void Awake()
    {
        muzzle = transform.Find("Muzzle");
    }

    private void Update()
    {
        CalculateShootTime();
    }
    private bool CanShoot()
    {

        if (shootTimer <= 0 && Time.timeScale != 0)
        {

            shootTimer = 0.5f;
            return true;
        }
        return false;
    }

    private void CalculateShootTime()
    {
        if (shootTimer != 0)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
                shootTimer = 0;
        }
    }

    public void Fire()
    {
        if (!CanShoot()) return;
        
        GameObject bullet = PoolManager.instance.Get("Bullet");
        bullet.GetComponent<Bullet>().SetSpeed(transform.right);//Call the flight function of the bullet
        bullet.transform.position = muzzle.position;
        
    }


}





