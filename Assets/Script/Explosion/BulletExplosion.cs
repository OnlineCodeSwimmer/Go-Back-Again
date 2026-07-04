using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    protected Animator animator;

    //Bullet Destroy Animation Controller
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        ExplosionAnimation();
    }

    protected void ExplosionAnimation()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            gameObject.SetActive(false);
        }
    }
}
