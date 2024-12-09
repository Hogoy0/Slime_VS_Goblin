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
        }
    }
}
