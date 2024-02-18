using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;

    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //the health bar always faces the camera
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }
}
