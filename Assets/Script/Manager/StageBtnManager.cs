using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageBtnManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;  // 버튼에 할당된 스테이지 데이터
    [SerializeField] private Image stageImage;    // 버튼 이미지
    [SerializeField] private Sprite clearedSprite;  // 스테이지 클리어 이미지
    [SerializeField] private Sprite lockedSprite;   // 스테이지 잠금 이미지
    [SerializeField] private Button stageButton;    // 버튼 컴포넌트

    // 스테이지 데이터를 임시로 저장
    private static StageData selectedStageData;

    // 현재 선택된 스테이지 데이터 반환
    public static StageData GetSelectedStageData()
    {
        return selectedStageData;
    }

    private void Start()
    {
        UpdateStageButton();
    }

    // 버튼 클릭 시 호출할 메서드
    public void OnClickStageButton()
    {
        if (stageData != null && stageData.IsUnlocked)
        {
            selectedStageData = stageData;
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.LogError("스테이지가 잠겨 있습니다!");
        }
    }

    // 스테이지 상태 업데이트
    private void UpdateStageButton()
    {
        if (stageData == null)
        {
            Debug.LogError("StageData가 할당되지 않았습니다!");
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

    // 스테이지 클리어 처리
    public static void ClearStage(StageData clearedStageData, StageData nextStageData)
    {
        if (clearedStageData != null)
        {
            clearedStageData.IsCleared = true;
            Debug.Log($"스테이지 {clearedStageData.StageName} 클리어!");
        }

        if (nextStageData != null && !nextStageData.IsUnlocked)
        {
            nextStageData.IsUnlocked = true;
            Debug.Log($"다음 스테이지 {nextStageData.StageName} 해금!");
        }
    }
}
