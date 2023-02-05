using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {   
    // TODO
    // [ ] Keep track of enemy spawning (not too much)
    //      Soft-core difficulty level (not really)
    // [ ] Add signal (public variable) for GameManager s.t. it can know
    //      When the enemy scene had complete (signalling change to next scene)
    // [ ] Add instance pool (reuse)

    [SerializeField] public BehaviourConfig[] behaviourPresets = new BehaviourConfig[] {};
    public Dictionary<string, BehaviourConfig> presetMap = new Dictionary<string, BehaviourConfig> {};
    [SerializeField] public GameObject[] enemiesPrefabs = new GameObject[] {};
    private Dictionary<int, GameObject[]> enemiesPool = new Dictionary<int, GameObject[]>{};

    [SerializeField] public static List<float> enemiesDistance;
    [SerializeField] public int[] SpawnStageCount = new int[] {};
    private int stageCounter = 0;
    private bool hasStageStart = false;
    private bool hasComplete = false;

    public float minX = 15f;
    public float maxX = 25f;
    public float minZ = -7f;
    public float maxZ = 7f;

    void Awake() {
        foreach(BehaviourConfig it in behaviourPresets) {
            presetMap[it.getCode()] = it; // Match with BehaviourConfigTypes
        }
    }

    GameObject getRandomEnemy(Vector3 position, string bPresetName) {
        int randomEnemyPrefabIdx = Random.Range(0, enemiesPrefabs.Length);
        GameObject spawnEnemy = Instantiate(enemiesPrefabs[randomEnemyPrefabIdx], position, Quaternion.identity);
        BehaviourConfig bConfig = presetMap[bPresetName];
        Enemy spawnEnemyEntity = spawnEnemy.GetComponent<Enemy>();
        spawnEnemyEntity.currentBehaviour = bConfig.getMovementBehaviour(
            GameObject.FindGameObjectsWithTag(EntityTagNames.playerTag)[0],
            spawnEnemy
        );
        spawnEnemyEntity.damageColor = bConfig.damageColor;
        spawnEnemyEntity.behaviourType = bConfig.behaviourType;
        spawnEnemyEntity.speedCurve = bConfig.speedCurve;
        return spawnEnemy;
    }

    void Start() {
        /*List<string> presetKeys = new List<string>(presetMap.Keys);
        for(int i = 0; i < 15; i++) {
            getRandomEnemy(
                GameObject.FindGameObjectsWithTag(EntityTagNames.playerTag)[0].transform.position
                + new Vector3(Random.Range(30.0f, 40.0f), 0, Random.Range(-7.0f, 7.0f)),
                presetKeys[Random.Range(0, presetKeys.Count)]
            );
        } */
        NextStage();
    }

    void Update() {
        PlayerScript player = GameObject.FindGameObjectsWithTag(EntityTagNames.playerTag)[0].GetComponent<PlayerScript>();
        enemiesDistance = player.getEnemiesDistance();
        if(hasStageStart && enemiesDistance.Count == 0) {
            // Stage Clear
            hasStageStart = false;
            stageCounter++;

            if(stageCounter == SpawnStageCount.Length) {
                // Enemy Spawner ends
                // Do something, I guess
                if(!hasComplete) {
                    hasComplete = true;
                    EndGame();
                }
            } else {
                // Assume spawn immediately
                player.setHealth(player.getHealth() + 3);
                NextStage();
            }
        }
    }

    void EndGame() {
        Debug.Log("Yay!");
    }

    public List<GameObject> NextStage() {
        int num_enemies = SpawnStageCount[stageCounter];
        List<GameObject> gameObjects = spawnEnemiesTowardPlayer(num_enemies );
        hasStageStart = true;
        return gameObjects;
    }

    public List<GameObject> spawnEnemiesTowardPlayer(
        int n, GameObject player = null
    ) 
    {
        List<string> presetKeys = new List<string>(presetMap.Keys);
        List<GameObject> spawnedEnemies = new List<GameObject>();

        if(player == null) {
            player = GameObject.FindGameObjectsWithTag(EntityTagNames.playerTag)[0];
        }
        for(int i = 0; i < n; i++) {
            spawnedEnemies.Add(
                getRandomEnemy(
                    player.transform.position + new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ)),
                    presetKeys[Random.Range(0, presetKeys.Count)]
                )
            );
        }
        return spawnedEnemies;
    }

}
