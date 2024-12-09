using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BtnType
{
    Start,
    Option,
    Quit
}
public class BtnManager : MonoBehaviour
{
    public BtnType currentType;
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
        }
    }
}
