using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {
    public float rayDistance = 10000.0f;
    public Material selectedMaterial;

    Material originalMaterial;
    GameObject previouslySelectedObject;

    void ClearSelectedObject() {
        if (previouslySelectedObject != null) {
            previouslySelectedObject.GetComponent<Renderer>().material = originalMaterial;
            previouslySelectedObject = null;
        }
    }

    void SelectObject(GameObject obj) {
        if (obj != previouslySelectedObject) {
            ClearSelectedObject();
            previouslySelectedObject = obj;
            originalMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
            obj.GetComponent<MeshRenderer>().sharedMaterial = selectedMaterial;
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Get Ray from mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log(ray);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance)) {
            GameObject hitObject = hit.collider.gameObject;
            SelectObject(hitObject);
            Debug.Log(hitObject.name);
        } else {
            ClearSelectedObject();
        }
    }
}
