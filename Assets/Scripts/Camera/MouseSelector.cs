using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelector : MonoBehaviour {

    GameObject selectedObject;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // Get Ray from mouse position on screen
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f));
            Vector3 directionToWorldPoint = Vector3.Normalize(worldPoint - cameraPosition);
            Debug.DrawRay(cameraPosition, directionToWorldPoint, Color.red);
            if (Physics.Raycast(cameraPosition, directionToWorldPoint, out RaycastHit hitInfo, Mathf.Infinity)) {
                GameObject hitObject = hitInfo.transform.gameObject;
                Debug.Log(hitObject.name);
                SelectObject(hitObject);
            } else {
                ClearSelectedObject();
            }
        }
    }

    /// <summary>
    /// Returns the currently selected object, may be null.
    /// </summary>
    /// <returns></returns>
    public GameObject GetSelectedObject() {
        return selectedObject;
    }

    void ClearSelectedObject() {
        selectedObject = null;
    }

    void SelectObject(GameObject obj) {
        if (selectedObject != null) {
            if (obj == selectedObject) return;
            ClearSelectedObject();
        }
        selectedObject = obj;
        Debug.Log("Selected Object: " + selectedObject.name);
    }
}
