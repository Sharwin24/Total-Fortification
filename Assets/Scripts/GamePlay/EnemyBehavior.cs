using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBehavior : MonoBehaviour
{


    // MUST update LevelManager.actionDone upon ending execution!
    public IEnumerator TakeAction(GameObject current)
    {

        // Patch - forcing height to be 1.32 - animation changes y-coord for some reason
        Vector3 currentPosition = current.transform.position;
        current.transform.position = new Vector3(currentPosition.x, 1.32f, currentPosition.z);

        print("Enemy Taking Action");
        TroopBase currentTroop = current.GetComponent<TroopBase>();
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Ally");
        GameObject closestTarget = findClosestTarget(current.transform.position, targets);
        TroopBase closestTargetTroop = closestTarget.GetComponent<TroopBase>();

        
        if (!inAttackRange(current, currentTroop, closestTarget)) {
            print("Ally troop not in range");

            MoveTowardTarget(current, currentTroop, closestTarget); 
            yield return new WaitForSeconds(3); // wait for move to complete

        } else {
            print("Ally troop in range");
            Attack(currentTroop, closestTargetTroop);
            yield return new WaitForSeconds(2); // wait for animation to complete
        }

        // Mark action as done
        print("Enemy Action done");
        LevelManager.actionDone = true;
    }

    private void Attack(TroopBase currentTroop, TroopBase targetTroop) {
        currentTroop.Attack(targetTroop);
    }

    private bool inAttackRange(GameObject current, TroopBase currentTroop, GameObject target) {
        return Vector3.Distance(target.transform.position, current.transform.position) <= currentTroop.AttackRange;
    }

    private void MoveTowardTarget(GameObject current, TroopBase currentTroop, GameObject target) {
        Vector3 moverPosition = current.transform.position;
        Vector3 targetPosition = target.transform.position;
        float moveDistance = currentTroop.MoveRange;
        float stopDistance = currentTroop.AttackRange;

        Vector3 direction = (targetPosition - moverPosition).normalized;
        float distanceToTarget = Vector3.Distance(moverPosition, targetPosition);
        float moveDistanceAdjusted = Mathf.Min(moveDistance, distanceToTarget - stopDistance);

        Vector3 finalPosition = moverPosition + direction * moveDistanceAdjusted;

        StartCoroutine(currentTroop.MoveTo(finalPosition));
    }


    private GameObject findClosestTarget(Vector3 currentPosition, GameObject[] targets) {
        float minDistance = Mathf.Infinity;
        GameObject closestPlayerTroop = null;

        foreach (GameObject playerTroop in targets) {
            float distance = Vector3.Distance(playerTroop.transform.position, currentPosition);
            if (distance < minDistance) {
                closestPlayerTroop = playerTroop;
                minDistance = distance;
            }
        }

        if (closestPlayerTroop == null) {
            throw new InvalidOperationException("Target is null");
        }

        return closestPlayerTroop;
    }
}
