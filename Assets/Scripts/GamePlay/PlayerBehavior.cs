using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehavior : MonoBehaviour
{

    public GameObject playerUI;
    public TextMeshProUGUI warningMessage;
    public float verticalMovementLimit = 3f;

    public int playerScore = 200;
    PlayerUIInteraction graphicUIRaycast;
    MouseSelector selector;
    Button moveButton;
    Button attackButton;
    bool actionInProgress = false;

    void Awake() {
        graphicUIRaycast = GetComponent<PlayerUIInteraction>();
        selector = Camera.main.GetComponent<MouseSelector>();
        moveButton = GameObject.FindGameObjectWithTag("MoveButton").GetComponent<Button>();
        attackButton = GameObject.FindGameObjectWithTag("AttackButton").GetComponent<Button>();
        warningMessage.text = "";
    }

   public IEnumerator TakeAction(GameObject current) {

        print("Player Taking Action!");

        Vector3 currentPosition = current.transform.position;

        // Render move range and attack range circle
        (GameObject moveRendererObject, GameObject attackRendererObject) = RenderRanges(current);

    
        bool actionTaken = false;

        moveButton.onClick.AddListener(() => StartCoroutine(MovePlayer(current, () => actionTaken = true)));
        attackButton.onClick.AddListener(() => StartCoroutine(AttackEnemy(current, () => actionTaken = true)));

        // Wait for action to be taken
        yield return new WaitUntil(() => actionTaken);

        moveButton.onClick.RemoveAllListeners();
        attackButton.onClick.RemoveAllListeners();

        print("Current Ally Troop Position: " + current.transform.position);

        Destroy(moveRendererObject);
        Destroy(attackRendererObject);
        LevelManager.actionDone = true;
    }

    private IEnumerator MovePlayer(GameObject player, Action onComplete) {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        if (actionInProgress) {
            yield break;
        }

        Vector3 selectedPosition = selector.GetSelectedPosition();
        Vector3 intendedPosition = new Vector3(selectedPosition.x, player.transform.position.y, selectedPosition.z);
        float intendedMoveDistance = Vector3.Distance(player.transform.position, intendedPosition);
        
        TroopBase playerTroop = player.GetComponent<TroopBase>();
        // Time needed to move to the intended position
        float timeNeeded = intendedMoveDistance / playerTroop.MoveSpeed;

        if (selectedPosition.y >= verticalMovementLimit) {
            warningMessage.text = "Intended Move too high.";
            yield return new WaitForSeconds(1);
            warningMessage.text = "";
        } else if (intendedMoveDistance <= playerTroop.MoveRange) {
            actionInProgress = true;
            StartCoroutine(playerTroop.MoveTo(intendedPosition));
            yield return new WaitForSeconds(timeNeeded);
            onComplete();
        } else {
            warningMessage.text = "Can't Move Beyond Move Range";
            yield return new WaitForSeconds(1);
            warningMessage.text = "";
        }

        actionInProgress = false;   
    }

    private IEnumerator AttackEnemy(GameObject player, Action onComplete) {
        yield return new WaitUntil(
            () => Input.GetMouseButtonDown(0) && selector.GetSelectedObject() != null);

        if (actionInProgress) {
            yield break;
        }

        GameObject targetEnemy = selector.GetSelectedObject();

        if (targetEnemy.tag == "Enemy") {
        
            Vector3 enemyPosition = targetEnemy.transform.position;

            TroopBase playerTroop = player.GetComponent<TroopBase>();
            TroopBase enemyTroop = targetEnemy.GetComponent<TroopBase>();

            float intendedAttackDistance = Vector3.Distance(player.transform.position, enemyPosition);

            if (intendedAttackDistance <= playerTroop.AttackRange) {
                actionInProgress = true;
                StartCoroutine(playerTroop.Attack(enemyTroop));
                yield return new WaitForSeconds(4);
                if(enemyTroop.Health <= 0) {
                    playerScore += enemyTroop.GetTroopScore();
                    Debug.Log("Player Score: " + playerScore);
                }
                onComplete();
            } else {
                warningMessage.text = "Can't Attack Beyond Attack Range";
                yield return new WaitForSeconds(1);
                warningMessage.text = "";
            }
        } else {
            warningMessage.text = "Can only attack troop";
            yield return new WaitForSeconds(1);
            warningMessage.text = "";
        }

        actionInProgress = false;    
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

