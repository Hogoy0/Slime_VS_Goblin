using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TMPro;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [SerializeField] private StageData currentStageData;

    public GameObject WinUI;

    private TMP_Text WaveText;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float goblinInterval = 0.5f;
    [SerializeField] private List<GoblinData> goblinDataList;

    private int currentWaveIndex = 0;
    private int remainingGoblinsInWave = 0;
    private List<GameObject> activeGoblins = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        WaveText = GameObject.Find("WaveText").GetComponent<TMP_Text>();
        LoadStageData();
    }

    private void LoadStageData()
    {
        currentStageData = StageBtnManager.GetSelectedStageData();

        if (currentStageData == null)
        {
            Debug.LogError("선택된 스테이지 데이터가 없습니다!");
            return;
        }

        Debug.Log($"스테이지 데이터 로드 완료: {currentStageData.StageName}");
    }

    public StageData GetCurrentStageData()
    {
        return currentStageData;
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
            SoundManager.instance.PlayEtcESfx(SoundManager.EtcESfx.SFX_WaveStart);
            currentWaveIndex++;
            remainingGoblinsInWave = 0;

            ShowWaveText($"Wave {currentWaveIndex}");

            yield return new WaitForSeconds(1f);

            int spawnIndex = 0;

            foreach (var goblin in wave.goblinSpawnList)
            {
                remainingGoblinsInWave += goblin.count;

                for (int i = 0; i < goblin.count; i++)
                {
                    Transform spawnPoint = GetNextSpawnPoint(ref spawnIndex);
                    SpawnGoblin(goblin.goblinType, spawnPoint.position);
                    yield return new WaitForSeconds(goblinInterval);
                }
            }

            yield return new WaitUntil(() => remainingGoblinsInWave == 0);
        }

        Debug.Log("모든 웨이브가 완료되었습니다!");
        UnlockNextStage();  // 다음 스테이지 해금
        WinUI.SetActive(true);
        SoundManager.instance.PlayEtcESfx(SoundManager.EtcESfx.SFX_Win);
    }


    private void ShowWaveText(string text)
    {
        if (WaveText == null) return;

        WaveText.text = text;
        WaveText.gameObject.SetActive(true);
        Invoke(nameof(HideWaveText), 1f);
    }

    private void HideWaveText()
    {
        if (WaveText != null)
        {
            WaveText.gameObject.SetActive(false);
        }
    }

    private Transform GetNextSpawnPoint(ref int spawnIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("소환 위치가 설정되지 않았습니다.");
            return null;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length;
        return spawnPoint;
    }

    private void SpawnGoblin(GType.GoblinType goblinType, Vector3 spawnPosition)
    {
        GoblinData goblinData = GetGoblinData(goblinType);
        if (goblinData == null)
        {
            Debug.LogError($"GoblinData를 찾을 수 없습니다: {goblinType}");
            return;
        }

        AssetReferenceGameObject prefab = goblinData.GetPrefab();
        if (prefab != null)
        {
            prefab.InstantiateAsync(spawnPosition, Quaternion.identity).Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    GameObject spawnedGoblin = handle.Result;

                    GoblinController controller = spawnedGoblin.GetComponent<GoblinController>();
                    if (controller != null)
                    {
                        controller.Initialize(goblinData, spawnPosition);
                    }

                    activeGoblins.Add(spawnedGoblin);
                }
                else
                {
                    Debug.LogError("고블린 생성 실패!");
                }
            };
        }
    }

    public void HandleGoblinDeath(GameObject goblin)
    {
        if (activeGoblins.Contains(goblin))
        {
            activeGoblins.Remove(goblin);
            remainingGoblinsInWave--;
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

    private void UnlockNextStage()
    {
        if (currentStageData == null)
        {
            Debug.LogError("현재 스테이지 데이터가 없습니다!");
            return;
        }

        if (currentStageData.NextStage == null)
        {
            Debug.LogWarning("다음 스테이지 데이터가 설정되지 않았습니다!");
            return;
        }

        if (!currentStageData.NextStage.IsUnlocked)
        {
            currentStageData.NextStage.IsUnlocked = true;
            Debug.Log($"다음 스테이지 '{currentStageData.NextStage.StageName}'가 해금되었습니다!");
        }
        else
        {
            Debug.Log("다음 스테이지는 이미 해금되어 있습니다.");
        }
    }

}
