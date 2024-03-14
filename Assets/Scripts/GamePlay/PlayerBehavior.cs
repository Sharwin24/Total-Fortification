using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class PlayerBehavior : MonoBehaviour
{

    public GameObject playerUI;

    PlayerUIInteraction graphicUIRaycast;

    void Start() {
        graphicUIRaycast = GetComponent<PlayerUIInteraction>();
    }

   public IEnumerator TakeAction(GameObject current)
   {
        Vector3 currentPosition = current.transform.position;
        current.transform.position = new Vector3(currentPosition.x, 1.32f, currentPosition.z);

        // render move range and attack range circle
        (GameObject moveRendererObject, GameObject attackRendererObject) = RenderRanges(current);
        yield return new WaitForSeconds(10);

        // wait for move or attack
        while (true) {
            
            // 1. Select an Enemy troop and click attack
            // Need to select an gameobject as target
            if (chooseToAttack()) {

            }

            // 2. Click on ground within the green circle to moves


            yield return null; // wait for next frame
        }

        // Mark action as done
        yield return new WaitForSeconds(0);
        Destroy(moveRendererObject);
        Destroy(attackRendererObject);
        LevelManager.actionDone = true;
    }

    private bool chooseToAttack() {
        
        bool attackClicked = graphicUIRaycast.GetRaycastTargets().Any(obj => obj.name == "AttackButton");
        if (attackClicked) {
            // do some work
        }

        return false;
    }

    private (GameObject moveRendererObject, GameObject attackRendererObject) RenderRanges(GameObject current) {
        GameObject moveRendererObject = new GameObject(name);
        GameObject attackRendererObject = new GameObject(name);

        LineRenderer moveRangeRenderer = moveRendererObject.AddComponent<LineRenderer>();
        LineRenderer attackRangeRenderer = attackRendererObject.AddComponent<LineRenderer>();

        moveRangeRenderer.transform.SetParent(current.transform, false);
        attackRangeRenderer.transform.SetParent(current.transform, false);

        TroopBase currentTroop = current.GetComponent<TroopBase>();

        SetupRenderer(moveRangeRenderer, current.transform.position, currentTroop.MoveRange, 100, Color.green);
        SetupRenderer(attackRangeRenderer, current.transform.position, currentTroop.AttackRange, 100, Color.magenta);

        return (moveRendererObject, attackRendererObject);
    }

    private void SetupRenderer(LineRenderer renderer, Vector3 center, float radius, int segments, Color color) {
        renderer.startWidth = 0.05f;
        renderer.endWidth = 0.05f;
        renderer.positionCount = segments + 1;
        renderer.useWorldSpace = true; 
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.startColor = color;
        renderer.endColor = color;

        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++) {
            float x = center.x + radius * Mathf.Cos(theta);
            float z = center.z + radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, center.y, z); // Adjust for the center position
            renderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}
