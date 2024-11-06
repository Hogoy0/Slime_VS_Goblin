using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GManager : MonoBehaviour
{
    public static GManager Instance;

    public GameObject launching_slime;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    Camera cam;


    public SlimeController Slime;
    public Trajectory trajectory;

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
        if (Slime != null)
        {
            Slime.DeActivateRb();
        }
    }

    void Update()
    {
        if (Slime != null) {
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

        
    }

    void OnDragStart()
    {
        Slime.DeActivateRb();
        startpoint = cam.ScreenToWorldPoint(Input.mousePosition);

        trajectory.show();
    }

    void OnDrag()
    {
        endpoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startpoint, endpoint);
        direction = (startpoint - endpoint).normalized;
        force = direction * distance * pushforce;

        Debug.DrawLine(startpoint, endpoint);

        trajectory.UpdateDots(Slime.pos, force);

    }

    void OnDragEnd()
    {
        //슬라임 밀기
        Slime.ActivateRb();
        Slime.push(force);

        trajectory.hide();

    }

    public void Spawn_Launching_Slime()
    {
        Vector3 launching_position = new Vector3(-4.6f, 5.6f, 0f);
        Debug.Log(launching_position);
        GameObject spawnedSlime = Instantiate(launching_slime, launching_position, Quaternion.identity);
        Slime = spawnedSlime.GetComponent<SlimeController>();
        Slime.DeActivateRb();

    }


}
