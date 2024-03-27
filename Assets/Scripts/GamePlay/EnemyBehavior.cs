using System;
using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    enum EnmyFSM {
        IDLE,
        MOVING,
        ATTACKING
    }
    EnmyFSM enemyState;

    void Start() {
        enemyState = EnmyFSM.IDLE;
    }


    // MUST update LevelManager.actionDone upon ending execution!
    public IEnumerator TakeAction(GameObject current)
    {

        float currentPositionY = current.transform.position.y;

        print("Enemy Taking Action");
        TroopBase currentTroop = current.GetComponent<TroopBase>();
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Ally");
        GameObject closestTarget = findClosestTarget(current.transform.position, targets);
        TroopBase closestTargetTroop = closestTarget.GetComponent<TroopBase>();

        if (enemyState == EnmyFSM.IDLE && !inAttackRange(current, currentTroop, closestTarget)) {
            enemyState = EnmyFSM.MOVING;
        } else {
            enemyState = EnmyFSM.ATTACKING;
        }
        
        if (enemyState == EnmyFSM.MOVING) {
            print("Ally troop not in range");
            //Wait for the move time to complete
            float moveTime = MoveTowardTarget(current, currentTroop, closestTarget); 
            yield return new WaitForSeconds(moveTime);

        } else if (enemyState == EnmyFSM.ATTACKING) {
            print("Ally troop in range");
            Attack(currentTroop, closestTargetTroop);
            yield return new WaitForSeconds(3); // wait for animation to complete
        } else {
            throw new InvalidOperationException("Enemy state not recognized");
        }

        // Mark action as done
        print("Enemy Action done");
        enemyState = EnmyFSM.IDLE;
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

    private float MoveTowardTarget(GameObject current, TroopBase currentTroop, GameObject target) {
        Vector3 moverPosition = current.transform.position;
        Vector3 targetPosition = target.transform.position;
        float moveDistance = currentTroop.MoveRange;
        float stopDistance = currentTroop.AttackRange - 0.5f;

        Vector3 direction = (targetPosition - moverPosition).normalized;
        float distanceToTarget = Vector3.Distance(moverPosition, targetPosition);
        float moveDistanceAdjusted = Mathf.Min(moveDistance, distanceToTarget - stopDistance);

        Vector3 finalPosition = moverPosition + direction * moveDistanceAdjusted;

        StartCoroutine(currentTroop.MoveTo(finalPosition));

        return moveDistanceAdjusted / currentTroop.MoveSpeed;
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

