using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public string stageName; // �������� �̸�
    public List<WaveData> waves; // ���̺� ����Ʈ
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
