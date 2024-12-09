using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBtnManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;  // ��ư�� �Ҵ�� �������� ������

    // �������� �����͸� �ӽ÷� ����
    private static StageData selectedStageData;

    // ���� ���õ� �������� ������ ��ȯ
    public static StageData GetSelectedStageData()
    {
        return selectedStageData;
    }

    // ��ư Ŭ�� �� ȣ���� �޼���
    public void OnClickStageButton()
    {
        if (stageData != null)
        {
            selectedStageData = stageData;
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.LogError("StageData�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }
}



