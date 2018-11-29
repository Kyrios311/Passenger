using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isEnemyAlive : MonoBehaviour {
    public int enemyAlive = 0;
    public GameObject[] enemies;
    public GameObject newPosition;
    private Vector3 startPosition;
	// Use this for initialization
	void Start () {
        enemyAlive = enemies.Length;
        startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (enemyAlive != 0)
        {
            enemyAlive = enemies.Length;
            for (int x = 0; x < enemies.Length; x++)
            {
                if (enemies[x].GetComponent<skeletonScript>() != null)
                {

                }
                else
                {
                    enemyAlive = enemyAlive - 1;
                }
            }
            
        }
        else
        {
            transform.position = Vector3.Lerp(startPosition, newPosition.transform.position, 1);
        }
	}
}
