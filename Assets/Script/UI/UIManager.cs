using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BtnType
{
    Start,
    Option,
    Quit,
    Resume,
    Restart,
    ReturnToStageScene,
    ReturnToTitleScene,
    NextStage
}

public class BtnManager : MonoBehaviour
{
    public BtnType currentType;
    public GameObject PauseUI;
    public GameObject DefeatUI;
    public GameObject WinUI;

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BtnType.Start:
                Debug.Log("새 게임");
                SceneManager.LoadScene("StageScene");
                break;
            case BtnType.Option:
                Debug.Log("설정");
                break;
            case BtnType.Quit:
                Debug.Log("게임 종료");
                Application.Quit();
                break;
            case BtnType.Resume:
                Debug.Log("게임 계속하기");
                ResumeGame();
                break;
            case BtnType.Restart:
                Debug.Log("현재 스테이지 다시 시작");
                RestartStageWithCurrentData();
                break;
            case BtnType.ReturnToStageScene:
                Debug.Log("스테이지 선택 화면으로 돌아가기");
                ReturnToStageScene();
                break;
            case BtnType.ReturnToTitleScene:
                Debug.Log("타이틀 화면으로 돌아가기");
                ReturnToTitleScene();
                break;
            case BtnType.NextStage:
                Debug.Log("다음 스테이지로");
                LoadNextStage();
                break;
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        PauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    private void ResumeGame()
    {
        isPaused = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    private void RestartStageWithCurrentData()
    {
        Time.timeScale = 1;
        StageData currentStageData = StageManager.Instance.GetCurrentStageData();
        if (currentStageData != null)
        {
            Debug.Log($"현재 스테이지 데이터로 재시작: {currentStageData.StageName}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogError("현재 스테이지 데이터를 불러올 수 없습니다!");
        }
    }

    private void LoadNextStage()
    {
        Time.timeScale = 1;
        StageData currentStageData = StageManager.Instance.GetCurrentStageData();
        if (currentStageData != null && currentStageData.NextStage != null)
        {
            Debug.Log($"다음 스테이지로 이동: {currentStageData.NextStage.StageName}");

            // 다음 스테이지 데이터 설정
            StageManager.Instance.SetStageData(currentStageData.NextStage);

            // 현재 씬 다시 로드
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogError("다음 스테이지 데이터를 불러올 수 없습니다!");
        }
    }

    private void ReturnToStageScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StageScene");
    }

    private void ReturnToTitleScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }
}
