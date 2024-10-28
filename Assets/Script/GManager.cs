using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GManager : MonoBehaviour
{
    public static GManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    Camera cam;
    public SlimeController Slime;
    [SerializeField] float pushforce = 4f;

    bool isDragging = false;

    Vector2 startpoint;
    Vector2 endpoint;
    Vector2 direction;
    Vector2 force;
    float distance;

    void Start()
    {
        cam = Camera.main;
        Slime.DeActivateRb();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            OnDragStart();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            OnDragEnd();
        }

        if (isDragging)
        {
            OnDrag();
        }
        
    }

    void OnDragStart()
    {
        Slime.DeActivateRb();
        startpoint = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnDrag()
    {
        endpoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startpoint, endpoint);
        direction = (startpoint - endpoint).normalized;
        force = direction * distance * pushforce;

        Debug.DrawLine(startpoint, endpoint);

    }

    void OnDragEnd()
    {
        //슬라임 밀기
        Slime.ActivateRb();
        Slime.push(force);

    }


}
