using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Disable on start
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate slowly
        //transform.Rotate(Vector3.up, 1f);
    }

}
