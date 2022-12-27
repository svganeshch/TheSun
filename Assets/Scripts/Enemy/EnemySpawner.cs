using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject trackerEnemyPreFab;
    private List<IEnemy> enemyObjects;
    void Start()
    {
        trackerEnemyPreFab = GameObject.Find("Circle");

        for (int i=1; i <= 10; i++)
        {
            GameObject enemyClone = Instantiate(trackerEnemyPreFab, new Vector3(i * 2f, 0, 0), Quaternion.identity);
            IEnemy enemy = enemyClone.GetComponent<EnemyController>();
            enemy.Health = i * 100;
            enemy.Speed = i * 2f; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
