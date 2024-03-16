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
    MouseSelector selector;
    Button moveButton;
    Button attackButton;

    void Start() {
        graphicUIRaycast = GetComponent<PlayerUIInteraction>();
        selector = Camera.main.GetComponent<MouseSelector>();
        moveButton = GameObject.FindGameObjectWithTag("MoveButton").GetComponent<Button>();
        attackButton = GameObject.FindGameObjectWithTag("AttackButton").GetComponent<Button>();
    }

   public IEnumerator TakeAction(GameObject current) {

        print("Player Taking Action!");

        Vector3 currentPosition = current.transform.position;
        current.transform.position = new Vector3(currentPosition.x, 1f, currentPosition.z);

        // Render move range and attack range circle
        (GameObject moveRendererObject, GameObject attackRendererObject) = RenderRanges(current);

    
        bool actionTaken = false;

        moveButton.onClick.AddListener(() => StartCoroutine(MovePlayer(current, () => actionTaken = true)));
        attackButton.onClick.AddListener(() => StartCoroutine(AttackEnemy(current, () => actionTaken = true)));

        // Wait for action to be taken
        yield return new WaitUntil(() => actionTaken);

        moveButton.onClick.RemoveAllListeners();
        attackButton.onClick.RemoveAllListeners();

        Destroy(moveRendererObject);
        Destroy(attackRendererObject);
        LevelManager.actionDone = true;
    }

    private IEnumerator MovePlayer(GameObject player, Action onComplete) {
        Debug.Log("Move initiated.");

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        Vector3 selectedPosition = selector.GetSelectedPosition();
        Vector3 intendedPosition = new Vector3(selectedPosition.x, player.transform.position.y, selectedPosition.z);
        float intendedMoveDistance = Vector3.Distance(player.transform.position, intendedPosition);
        
        TroopBase playerTroop = player.GetComponent<TroopBase>();
        // Time needed to move to the intended position
        float timeNeeded = intendedMoveDistance / playerTroop.MoveSpeed;
        if (intendedMoveDistance <= playerTroop.MoveRange) {
            Debug.Log("call MoveTo now");
            StartCoroutine(playerTroop.MoveTo(intendedPosition));
            yield return new WaitForSeconds(timeNeeded);
            onComplete();
        } else {
            Debug.Log("Cannot move beyond MoveRange");
            yield return new WaitForSeconds(0);
        }
    }

    private IEnumerator AttackEnemy(GameObject player, Action onComplete) {
        Debug.Log("Attack initiated.");
        yield return new WaitUntil(
            () => Input.GetMouseButtonDown(0) && selector.GetSelectedObject() != null);

        GameObject targetEnemy = selector.GetSelectedObject();

        if (targetEnemy.tag == "Enemy") {
        
            Vector3 enemyPosition = targetEnemy.transform.position;

            TroopBase playerTroop = player.GetComponent<TroopBase>();
            TroopBase enemyTroop = targetEnemy.GetComponent<TroopBase>();

            float intendedAttackDistance = Vector3.Distance(player.transform.position, enemyPosition);

            if (intendedAttackDistance <= playerTroop.AttackRange) {
                StartCoroutine(playerTroop.Attack(enemyTroop));
                yield return new WaitForSeconds(3);
                onComplete();
            } else {
                Debug.Log("Cannot attack beyond ATtackRange");
                yield return new WaitForSeconds(0);
            }
        } else {
            Debug.Log("Can only attack troop");
            yield return new WaitForSeconds(0);
        }    
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
