using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Selector : MonoBehaviour {

    public Color selectedObjectColor;
    public Color defaultCrosshairColor = Color.white;
    public Color selectedCrosshairColor = Color.red;

    Color originalColor;
    GameObject previouslySelectedObject;
    Image crosshair;
    Vector3 centerScreen = new(0.5f, 0.5f, 0);

    // Start is called before the first frame update
    void Start() {
        if (crosshair == null) {
            crosshair = GameObject.FindWithTag("MainCrosshair").GetComponent<Image>();
        }
        crosshair.color = defaultCrosshairColor;
    }

    // Update is called once per frame
    void Update() {
        // Get Ray from mouse position on screen
        Ray ray = Camera.main.ViewportPointToRay(centerScreen);
        //Debug.Log(ray);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) {
            GameObject hitObject = hit.collider.gameObject;
            SelectObject(hitObject);
            Debug.Log(hitObject.name);
        } else {
            ClearSelectedObject();
        }
    }

    void SetCrosshairColor(Color color) { crosshair.color = color; }

    void ClearSelectedObject() {
        if (previouslySelectedObject != null) {
            previouslySelectedObject.GetComponent<Renderer>().material.color = originalColor;
            previouslySelectedObject = null;
            SetCrosshairColor(defaultCrosshairColor);
        }
    }

    void SelectObject(GameObject obj) {
        if (obj != previouslySelectedObject) {
            ClearSelectedObject();
            previouslySelectedObject = obj;
            originalColor = obj.GetComponent<MeshRenderer>().sharedMaterial.color;
            obj.GetComponent<MeshRenderer>().sharedMaterial.color = selectedObjectColor;
            SetCrosshairColor(selectedCrosshairColor);
        }
    }
}
