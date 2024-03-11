using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelector : MonoBehaviour {

    public Color selectedObjectColor = Color.green;

    GameObject selectedObject;
    Color previouslySelectedObjectColor;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // Get Ray from mouse position on screen
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToWorldPoint = worldPoint - cameraPosition;
        //Ray ray = Camera.main.ScreenToWorldPoint(mousePosition);
        //Debug.Log(ray);
        if (Physics.Raycast(cameraPosition, directionToWorldPoint, out RaycastHit hitInfo, Mathf.Infinity)) {
            GameObject hitObject = hitInfo.transform.root.gameObject;
            Debug.Log(hitObject.name);
            SelectObject(hitObject);
        } else {
            ClearSelectedObject();
        }
    }

    public GameObject GetSelectedObject() {
        return selectedObject;
    }

    void SetObjectColor(Color color) {
        if (true) return;
        Renderer[] rs = selectedObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs) {
            Material m = r.material;
            m.color = color;
            r.material = m;
        }
    }

    void ClearSelectedObject() {
        if (selectedObject == null) return;

        SetObjectColor(previouslySelectedObjectColor);
        selectedObject = null;
    }

    void SelectObject(GameObject obj) {
        if (selectedObject != null) {
            if (obj == selectedObject) return;
            ClearSelectedObject();
        }
        previouslySelectedObjectColor = obj.GetComponent<Renderer>().material.color;
        selectedObject = obj;

        SetObjectColor(selectedObjectColor);
    }
}
