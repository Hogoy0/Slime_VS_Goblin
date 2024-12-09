using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBtnManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;  // 버튼에 할당된 스테이지 데이터

    // 스테이지 데이터를 임시로 저장
    private static StageData selectedStageData;

    // 현재 선택된 스테이지 데이터 반환
    public static StageData GetSelectedStageData()
    {
        return selectedStageData;
    }

    // 버튼 클릭 시 호출할 메서드
    public void OnClickStageButton()
    {
        if (stageData != null)
        {
            selectedStageData = stageData;
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.LogError("StageData가 할당되지 않았습니다!");
        }
    }
}



