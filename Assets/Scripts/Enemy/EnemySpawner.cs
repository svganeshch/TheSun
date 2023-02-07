using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject trackerEnemyPreFab;
    public Transform[] spawnPoints;

    private Transform player;

    private UIManager uimanager;

    private int score;
    private int spawnCount = 0;
    private float spawnWaitTime;
    private float currentSpawnWaitTime = 0;

    public Toggle spawncheck;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        uimanager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    void Start()
    {
        score = uimanager.currentScore;
        StartCoroutine(SpawnEnemy(spawnPoints[0], 3));
    }

    private void Update()
    {
        if (spawncheck!= null)
        {
            if (spawncheck.isOn)
            {
                Wave1();
            }
        }
    }

    void Wave1()
    {
        int spawnPoint = Random.Range(0, spawnPoints.Length);
        spawnWaitTime = 2;
        currentSpawnWaitTime += Time.deltaTime;

        if (currentSpawnWaitTime > spawnWaitTime)
        {
            if (spawnCount <= 3)
                StartCoroutine(SpawnEnemy(spawnPoints[spawnPoint], 3));
            else if (spawnCount <= 2)
                StartCoroutine(SpawnEnemy(spawnPoints[spawnPoint], 2));
            else if (spawnCount <= 1)
                StartCoroutine(SpawnEnemy(spawnPoints[spawnPoint], 1));

            currentSpawnWaitTime = 0;

            Debug.Log("Checking wave : " + spawnCount);
        }
    }

    public IEnumerator SpawnEnemy(Transform spawnPoint, int spawns)
    {
        while (spawns != 0)
        {
            yield return new WaitForSeconds(1);
            Instantiate(trackerEnemyPreFab, spawnPoint.position, Quaternion.identity);
            spawns--;
            spawnCount++;
        }
    }

    public void EnemyDead()
    {
        spawnCount--;
    }
}
