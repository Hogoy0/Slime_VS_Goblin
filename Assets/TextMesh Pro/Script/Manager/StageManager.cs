using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [SerializeField] private StageData currentStageData; // 현재 스테이지 데이터
    [SerializeField] private Transform[] spawnPoints; // 소환 위치 배열
    [SerializeField] private float goblinSpawnInterval = 0.5f; // 고블린 생성 간격
    [SerializeField] private float waveInterval = 3.0f; // 웨이브 간 대기 시간
    [SerializeField] private List<GoblinData> goblinDataList; // 고블린 데이터 리스트

    private int currentWaveIndex = 0; // 현재 웨이브 인덱스
    private List<GameObject> activeGoblins = new List<GameObject>(); // 활성화된 고블린 리스트
    private List<GameObject> activeSlimes = new List<GameObject>(); // 활성화된 슬라임 리스트

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
        if (currentStageData == null || currentStageData.waves.Count == 0)
        {
            Debug.LogError("스테이지 데이터가 유효하지 않습니다.");
            return;
        }
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        foreach (var wave in currentStageData.waves)
        {
            int spawnIndex = 0;

            foreach (var goblin in wave.goblinSpawnList)
            {
                for (int i = 0; i < goblin.count; i++)
                {
                    Transform spawnPoint = GetNextSpawnPoint(ref spawnIndex);
                    GameObject spawnedGoblin = SpawnGoblin(goblin.goblinType, spawnPoint.position);
                    if (spawnedGoblin != null)
                    {
                        activeGoblins.Add(spawnedGoblin);
                    }
                    yield return new WaitForSeconds(goblinSpawnInterval); // 고블린 생성 간격
                }
            }

            yield return new WaitUntil(() => activeGoblins.Count == 0); // 모든 고블린 제거 대기
            currentWaveIndex++;

            yield return new WaitForSeconds(waveInterval); // 웨이브 간 대기 시간
        }
    }

    private Transform GetNextSpawnPoint(ref int spawnIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length;
        return spawnPoint;
    }

    private GameObject SpawnGoblin(GType.GoblinType goblinType, Vector3 spawnPosition)
    {
        GoblinData goblinData = GetGoblinData(goblinType);
        if (goblinData == null)
        {
            Debug.LogError($"GoblinData를 찾을 수 없습니다: {goblinType}");
            return null;
        }

        AssetReferenceGameObject prefab = goblinData.GetPrefab();
        if (prefab != null)
        {
            GameObject spawnedGoblin = null;
            prefab.InstantiateAsync(spawnPosition, Quaternion.identity).Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    spawnedGoblin = handle.Result;
                    GoblinController controller = spawnedGoblin.GetComponent<GoblinController>();
                    if (controller != null)
                    {
                        controller.Initialize(goblinData,spawnPosition);
                    }
                }
            };
            return spawnedGoblin;
        }
        return null;
    }

    private void HandleGoblinDeath(GameObject goblin)
    {
        if (activeGoblins.Contains(goblin))
        {
            activeGoblins.Remove(goblin);
        }
        Destroy(goblin);
    }

    private GoblinData GetGoblinData(GType.GoblinType goblinType)
    {
        foreach (var data in goblinDataList)
        {
            if (data.goblinType == goblinType)
                return data;
        }
        return null;
    }
}

