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
        SoundManager.instance.PlayUIESfx(SoundManager.UIESfx.UI_BasicBtn);
        switch (currentType)
        {
            case BtnType.Start:
                Debug.Log("�� ����");
                SceneManager.LoadScene("StageScene");
                break;
            case BtnType.Option:
                Debug.Log("����");
                break;
            case BtnType.Quit:
                Debug.Log("���� ����");
                Application.Quit();
                break;
            case BtnType.Resume:
                Debug.Log("���� ����ϱ�");
                ResumeGame();
                break;
            case BtnType.Restart:
                Debug.Log("���� �������� �ٽ� ����");
                RestartStageWithCurrentData();
                break;
            case BtnType.ReturnToStageScene:
                Debug.Log("�������� ���� ȭ������ ���ư���");
                ReturnToStageScene();
                break;
            case BtnType.ReturnToTitleScene:
                Debug.Log("Ÿ��Ʋ ȭ������ ���ư���");
                ReturnToTitleScene();
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
            Debug.Log($"���� �������� �����ͷ� �����: {currentStageData.StageName}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogError("���� �������� �����͸� �ҷ��� �� �����ϴ�!");
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
