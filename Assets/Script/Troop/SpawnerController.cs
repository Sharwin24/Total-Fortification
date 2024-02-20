using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {
    public GameObject troopPrefab;

    // The area where the troop will spawn with X and Z bounds
    float xMin = -4.0f;
    float xMax = 7.0f;
    float yMin = 1.0f;
    float yMax = 1.0f;
    float zMin = -6.0f;
    float zMax = 8.0f;

    public void SpawnTroop() {
        Vector3 spawnPosition = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax));
        GameObject troop = Instantiate(troopPrefab, spawnPosition, Quaternion.identity);
        troop.tag = "Enemy";
        troop.transform.parent = gameObject.transform;
    }

    public void SpawnAlly() {
        Vector3 spawnPosition = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax));
        GameObject troop = Instantiate(troopPrefab, spawnPosition, Quaternion.identity);
        troop.tag = "Player";
        troop.transform.parent = gameObject.transform;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
