using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldEnemySpawner : MonoBehaviour {

    public Hittable playerHittable;

    public int delayBeforeBatch;
    public int numberInBatch;
    public int distanceFromSpawner;
    public GameObject enemyToSpawn;
    public bool spawning;

    int batchCounter;
    GameObject currentEnemy;

    public void StartSpawning() {
        spawning = true;
        batchCounter = delayBeforeBatch;
    }

    public void SpawnSingle() {
        currentEnemy = Instantiate(enemyToSpawn); //spawn an enemy
        currentEnemy.gameObject.name = "Training Phantom";
        currentEnemy.GetComponent<AIDummy>().target = playerHittable.gameObject;
        currentEnemy.GetComponent<Hittable>().cam = playerHittable.cam;
        currentEnemy.GetComponent<Hittable>().damageNumberCanvas = playerHittable.damageNumberCanvas;
        currentEnemy.transform.Translate(Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f))) * distanceFromSpawner); //and move them away in a random direction
    }

    public void StopSpawning() {
        spawning = false;
    }
	
	// FixedUpdate is called once per frame
	void FixedUpdate () {
		if (spawning) {
            batchCounter++;
            if (batchCounter >= delayBeforeBatch) { //if the time has come to spawn a batch
                for (int i = 0; i < numberInBatch; i++) { //then for the amount of enemies in a batch
                    currentEnemy = Instantiate(enemyToSpawn); //spawn an enemy
                    currentEnemy.gameObject.name = "Training Phantom";
                    currentEnemy.GetComponent<AIDummy>().target = playerHittable.gameObject;
                    currentEnemy.GetComponent<Hittable>().cam = playerHittable.cam;
                    currentEnemy.GetComponent<Hittable>().damageNumberCanvas = playerHittable.damageNumberCanvas;
                    currentEnemy.transform.Translate(Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f))) * distanceFromSpawner); //and move them away in a random direction
                }
                batchCounter = 0; //reset the batch counter
            }
        }
	}
}
