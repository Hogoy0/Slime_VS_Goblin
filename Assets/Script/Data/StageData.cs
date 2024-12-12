using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public string StageName;             // �������� �̸�
    public bool IsUnlocked;              // �������� �ر� ����
    public bool IsCleared;               // �������� Ŭ���� ����
    public Sprite DefaultSprite;         // ��ư �̹���
    public StageData NextStage;          // ���� �������� ����
    public List<WaveData> waves; // ���̺� ����Ʈ
    public UnitUnlockData[] unlockedSlime;//�رݵ� ������
    public List<Vector3> spawnPoints;
}

[System.Serializable]
public class WaveData
{
    public string waveName; // ���̺� �̸�
    public List<GoblinSpawnInfo> goblinSpawnList; // ���̺꿡 ���Ե� ��� ����
}

[System.Serializable]
public class GoblinSpawnInfo
{
    public GType.GoblinType goblinType; // ��� Ÿ��
    public int count; // ��� ��
}
[System.Serializable]
public class UnitUnlockData
{
    public SType.SlimeType slimeType;  // ������ Ÿ��
    public bool isUnlocked;  // �ر� ����
}