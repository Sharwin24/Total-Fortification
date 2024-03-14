using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{


    public Slider healthSlider;

    public Color enemyColor = Color.red;
    public Color playerColor = Color.green;

    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;

        //set the color of the health bar
    GameObject parentGameObject = transform.parent.gameObject;

    if (parentGameObject.tag == "Enemy")
    {
        healthSlider.fillRect.GetComponent<Image>().color = enemyColor;
    }
    else if (parentGameObject.tag == "Ally")
    {
        healthSlider.fillRect.GetComponent<Image>().color = playerColor;
    }
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
