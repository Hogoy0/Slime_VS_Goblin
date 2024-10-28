using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public GameObject Btns;

    void Update()
    {
        if (Input.anyKey)
        {
            Btns.gameObject.SetActive(true);
        }

    }
}
