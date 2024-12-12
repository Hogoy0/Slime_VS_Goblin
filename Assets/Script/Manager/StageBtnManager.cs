using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageBtnManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;  // ��ư�� �Ҵ�� �������� ������
    [SerializeField] private Image stageImage;    // ��ư �̹���
    [SerializeField] private Sprite clearedSprite;  // �������� Ŭ���� �̹���
    [SerializeField] private Sprite lockedSprite;   // �������� ��� �̹���
    [SerializeField] private Button stageButton;    // ��ư ������Ʈ

    // �������� �����͸� �ӽ÷� ����
    private static StageData selectedStageData;

    // ���� ���õ� �������� ������ ��ȯ
    public static StageData GetSelectedStageData()
    {
        return selectedStageData;
    }

    private void Start()
    {
        UpdateStageButton();
    }

    // ��ư Ŭ�� �� ȣ���� �޼���
    public void OnClickStageButton()
    {
        if (stageData != null && stageData.IsUnlocked)
        {
            selectedStageData = stageData;
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.LogError("���������� ��� �ֽ��ϴ�!");
        }
    }

    // �������� ���� ������Ʈ
    private void UpdateStageButton()
    {
        if (stageData == null)
        {
            Debug.LogError("StageData�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        if (stageData.IsUnlocked)
        {
            stageImage.sprite = stageData.DefaultSprite;
            stageButton.interactable = true;
        }
        else
        {
            stageImage.sprite = lockedSprite;
            stageButton.interactable = false;
        }
    }

    // �������� Ŭ���� ó��
    public static void ClearStage(StageData clearedStageData, StageData nextStageData)
    {
        if (clearedStageData != null)
        {
            clearedStageData.IsCleared = true;
            Debug.Log($"�������� {clearedStageData.StageName} Ŭ����!");
        }

        if (nextStageData != null && !nextStageData.IsUnlocked)
        {
            nextStageData.IsUnlocked = true;
            Debug.Log($"���� �������� {nextStageData.StageName} �ر�!");
        }
    }
}
