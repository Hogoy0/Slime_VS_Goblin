using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] int dotsNumber;
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float dotSpacing;

    Transform[] dotsList;

    Vector2 pos; //dot pos 변수
    float timestamp;

    void Start()
    {
        //시작하면 trajectory 숨기기
        hide();

        PrepareDots();
        
    }

    void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];

        for (int i = 0; i < dotsList.Length; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;
        }
    }

    public void UpdateDots(Vector3 ballPos, Vector2 forceApplied)
    {
        timestamp = dotSpacing;
        for (int i = 0; i < dotsNumber; i++)
        {
            pos.x = (ballPos.x + forceApplied.x * timestamp);
            pos.y = (ballPos.y + forceApplied.y * timestamp) - (Physics2D.gravity.magnitude * timestamp * timestamp) / 2f;

            dotsList[i].position = pos;
            timestamp += dotSpacing;
        }
    }

    public void show()
    {
        dotsParent.SetActive(true);
    }

    public void hide() 
    {
        dotsParent.SetActive(false);
    }




}
