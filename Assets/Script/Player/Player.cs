using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Move Varible
    private float speed = 0f;
    private float rotateSpeed = 100f;
    private float currentAngle;


    //Input Varible
    private PlayerController playerController;

    //Component
    Transform firstTurret;
    private void Awake()
    {
        playerController = new PlayerController();
        firstTurret = transform.Find("FirstTurret");
    }

    private void OnEnable()
    {
        playerController.Enable();
        Subscribe();
    }

    private void OnDisable()
    {
        playerController.Disable();
        Unsubscribe();
    }


    private void Update()
    {
        AnchorMove();
        FirstTurretRotate();
    }

    private void FirstTurretRotate()
    {
        float rotateInput = playerController.Player.Rotate.ReadValue<float>();
        currentAngle += rotateInput * rotateSpeed * Time.deltaTime;
        currentAngle = Mathf.Repeat(currentAngle, 360f);
        firstTurret.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }

    private void AnchorMove()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        firstTurret.GetComponent<Turret>().Fire();
    }


    private void Subscribe()
    {
        playerController.Player.Fire.performed += Shoot;
    }

    private void Unsubscribe()
    {
        playerController.Player.Fire.performed -= Shoot;
    }
}

