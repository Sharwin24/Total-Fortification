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
        print("Enemy Taking Action");
        // // Animator anim = currentTroop.GetComponent<Animator>();
        TroopBase currentTroop = current.GetComponent<TroopBase>();
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Ally");
        GameObject closestTarget = findClosestTarget(current.transform.position, targets);
        TroopBase closestTargetTroop = closestTarget.GetComponent<TroopBase>();

        
        if (!inAttackRange(current, currentTroop, closestTarget)) {
            print("Ally troop not in range");

;           MoveTowardTarget(current, currentTroop, closestTarget); // When moving animtaion is added, remove this line and add wait.

            //  Attack if in range
            if (inAttackRange(current, currentTroop, closestTarget)) {
                Attack(currentTroop, closestTargetTroop);
            }
            yield return new WaitForSeconds(0); // modify this to wait for longer
        // Attack twice
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
        targetTroop.TakeDamage(currentTroop.AttackPower);
    }

    private bool inAttackRange(GameObject current, TroopBase currentTroop, GameObject target) {
        return Vector3.Distance(target.transform.position, current.transform.position) <= currentTroop.AttackRange;
    }

    private bool canContinueMove(GameObject current, TroopBase currentTroop, GameObject target, Vector3 positionBeforeAction) {
        return Vector3.Distance(positionBeforeAction, current.transform.position) < currentTroop.MoveRange;
    }

    private void MoveTowardTarget(GameObject current, TroopBase currentTroop, GameObject target) {
        print("MoveToward called");
        Vector3 positionBeforeAction = current.transform.position;
        print(!inAttackRange(current, currentTroop, target));
        print(canContinueMove(current, currentTroop, target, positionBeforeAction));

        while (!inAttackRange(current, currentTroop, target) && canContinueMove(current, currentTroop, target, positionBeforeAction)) {
            print("IN LOOP");
            // Temp - let animation handle this later
            current.transform.position = Vector3.MoveTowards(current.transform.position, target.transform.position, 3);
            // Should wait for this to look better but this is temp so leave it
        }
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
