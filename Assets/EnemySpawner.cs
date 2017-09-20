using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private GameObject tankPrefab;
    [SerializeField]
    private float timeBetweenWaves = 3f;
    [SerializeField]
    private SpawnpointsScript groundSpawns;
    [SerializeField]
    private SpawnpointsScript airSpawns;
    [SerializeField]
    Camera cam;

    public enum EnemyType { Land = 0, Air = 1 };

    [SerializeField]
    private Wave_Tag[] waves;

    private int waveIndex = 0;
    private float countdown = 3f;

    bool playerdead = false;

    private void Update()
    {
        if (playerdead)
        {
            return;
        }

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < waves[waveIndex].Amount; i++)
        {
            SpawnThisWave(waves[waveIndex].prefab,waves[waveIndex].enemytype);
            yield return new WaitForSeconds(0.5f);
        }

        waveIndex++; 

        if (waveIndex >= waves.Length-1)
        {
            waveIndex = 0;
        }

    }

    private void SpawnThisWave(GameObject pPrefab, EnemyType pType)
    {
        if (pType == EnemyType.Land)
        {
            int spawnPointIndex = Random.Range(0, groundSpawns.points.Length);

            if (!CheckIfInCam(spawnPointIndex, pType))
            {
                Instantiate(pPrefab, groundSpawns.points[spawnPointIndex].transform.position, groundSpawns.points[spawnPointIndex].transform.rotation);
            }
            else
            {
                SpawnThisWave(pPrefab, pType);
            }
        }
        else if (pType == EnemyType.Air)
        {
            int spawnPointIndex = Random.Range(0, groundSpawns.points.Length);

            if (!CheckIfInCam(spawnPointIndex, pType))
            {
                Instantiate(pPrefab, airSpawns.points[spawnPointIndex].transform.position, airSpawns.points[spawnPointIndex].transform.rotation);
            }
            else
            {
                SpawnThisWave(pPrefab, pType);
            }
        }
    }

    private bool CheckIfInCam(int pSpawnPointIndex, EnemyType pType)
    {
        Vector3 viewPos;

        if (pType == EnemyType.Land)
        {
            viewPos = cam.WorldToViewportPoint(groundSpawns.points[pSpawnPointIndex].transform.position);
        }
        else if (pType == EnemyType.Air)
        {
            viewPos = cam.WorldToViewportPoint(airSpawns.points[pSpawnPointIndex].transform.position);
        }
        else
        {
            viewPos = Vector3.zero;
        }

        if (viewPos.x > 1.0f || viewPos.x < 0.0f)
        {
            Debug.Log("Spawning ground enemy");
            return false;
        }
        else
        {
            Debug.Log("This ground spawnpoint is in view of camera");
            return true;
        }
    }

    [System.Serializable]
    private struct Wave_Tag
        {
        public GameObject prefab;
        public int Amount;
        public EnemyType enemytype;
        }
}
