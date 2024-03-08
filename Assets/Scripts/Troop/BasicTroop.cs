using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class BasicSoldier : TroopBase
{

    public GameObject[] enemies;
    Transform self;



    void Start() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        self = gameObject.transform;
        Health = 100;
        
        Debug.Log("Health " + Health);

       // InvokeRepeating("LaunchAttack", 0f, 2f);
    }

    public void LaunchAttack() {

        Debug.Log("Launch Attack");

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        List<GameObject> enemiesInRange = GetInRangeEnemies();

        AttackEnemies(enemiesInRange);

    }

    private List<GameObject> GetInRangeEnemies() {

        List<GameObject> inRangeEnemies = new List<GameObject>();
        foreach (GameObject enemy in enemies) {
            float distance = Vector3.Distance(self.position, enemy.transform.position);
            if (distance <= AttackRange) {
                inRangeEnemies.Add(enemy);
            }
        }
        return inRangeEnemies;
    }

    private void AttackEnemies(List<GameObject> enemies) {
    foreach (GameObject enemy in enemies) {
        ITroop troopComponent = enemy.GetComponent<ITroop>();
        if (troopComponent != null) {
            Attack(troopComponent);
            Debug.Log("Attacking");
        }
    }
}

   
    public override void Attack(ITroop target)
    {
        Debug.Log("BasicSoldier attacking with power: " + AttackPower);
        // Implement attack logic, for example:
        target.TakeDamage(AttackPower); 
        
    }

    public override void MoveTo(Vector3 position)
    {
        Debug.Log("BasicSoldier moving to position: " + position);
        // Implement movement logic, for example:
        transform.position = position;
    }
}
