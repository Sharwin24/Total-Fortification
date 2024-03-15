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

        float currentPositionY = current.transform.position.y;

        print("Enemy Taking Action");
        TroopBase currentTroop = current.GetComponent<TroopBase>();
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Ally");
        GameObject closestTarget = findClosestTarget(current.transform.position, targets);
        TroopBase closestTargetTroop = closestTarget.GetComponent<TroopBase>();

        
        if (!inAttackRange(current, currentTroop, closestTarget)) {
            print("Ally troop not in range");

            MoveTowardTarget(current, currentTroop, closestTarget); 
            yield return new WaitForSeconds(5); // wait for move to complete

        } else {
            print("Ally troop in range");
            Attack(currentTroop, closestTargetTroop);
            yield return new WaitForSeconds(3); // wait for animation to complete
        }

        // Mark action as done
        print("Enemy Action done");
        LevelManager.actionDone = true;
        Vector3 updatedPosition = current.transform.position;
        current.transform.position = new Vector3(updatedPosition.x, currentPositionY, updatedPosition.z);
    }

    private void Attack(TroopBase currentTroop, TroopBase targetTroop) {
        StartCoroutine(currentTroop.Attack(targetTroop));
    }

    private bool inAttackRange(GameObject current, TroopBase currentTroop, GameObject target) {
        print("Attack range: " + currentTroop.AttackRange);
        print("Distance: " + Vector3.Distance(target.transform.position, current.transform.position));

        return Vector3.Distance(target.transform.position, current.transform.position) <= currentTroop.AttackRange;
    }

    private void MoveTowardTarget(GameObject current, TroopBase currentTroop, GameObject target) {
        Vector3 moverPosition = current.transform.position;
        Vector3 targetPosition = target.transform.position;
        float moveDistance = currentTroop.MoveRange;
        float stopDistance = currentTroop.AttackRange - 0.5f;

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
