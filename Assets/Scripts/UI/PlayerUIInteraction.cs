using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class PlayerUIInteraction : MonoBehaviour
{
    //The canvas that contains the player UI
    public GameObject playerUI;

    GraphicRaycaster ui_Raycaster;

    PointerEventData click_data;

    List<RaycastResult> click_results;

    void Start()
    {
        ui_Raycaster = playerUI.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            GetUiElementsClicked();
        }

    }

    void GetUiElementsClicked()
    {
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();

        ui_Raycaster.Raycast(click_data, click_results);

        foreach (RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;

            Debug.Log("Hit " + result.gameObject.name);
        }
    }

    public IEnumerable<GameObject> GetRaycastTargets() {
        List<RaycastResult> clickResults = new List<RaycastResult>();;

        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            click_data.position = Mouse.current.position.ReadValue();

            ui_Raycaster.Raycast(click_data, clickResults);
        }

        return clickResults.Select(target => target.gameObject);
    } 

}
