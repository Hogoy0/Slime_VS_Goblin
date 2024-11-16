using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    //소환 코스트 관련 변수
    [SerializeField] private int m_maxRes = 200;
    [SerializeField] private int m_resPerSec = 10;
    [SerializeField] private int m_nowRes = 0;
    [SerializeField] private float m_resInterval = 2.0f;
    public Text m_resText;


    //여기까지 소환 코스트 변수

    Camera cam;
    float CameraMoveSpeed = 5.0f;
    public GameObject[] slimePrefabs;
    public Vector3 launching_position;
    public SlimeController Slime;
    public Trajectory trajectory;
    public bool isSlimeReady = false;

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
        isSlimeReady = false;
        StartCoroutine(GeneRes());
    }

    void Update()
    {
        if (Slime != null && isSlimeReady == true) {
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

        if (Input.GetKey(KeyCode.A))
        {
            cam.transform.position += Vector3.left * CameraMoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            cam.transform.position += Vector3.right * CameraMoveSpeed * Time.deltaTime;
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
        isSlimeReady = false;

    }

    public void Spawn_Launching_Slime(int slimeIndex)
    {
        int SpawnCost = slimePrefabs[slimeIndex].GetComponent<SlimeController>().SlimeCost;
        if (Slime == null && m_nowRes >= SpawnCost)
        {
            Instantiate_Slime(slimeIndex);
            m_nowRes -= SpawnCost;
            m_resText.text = m_nowRes.ToString();
        }

    }

    void Instantiate_Slime(int slimeIndex)
    {
        Vector3 launching_position = new Vector3(-2.6f, -1.8f, 0f);
        Debug.Log(launching_position);
        GameObject spawnedSlime = Instantiate(slimePrefabs[slimeIndex], launching_position, Quaternion.identity);
        Slime = spawnedSlime.GetComponent<SlimeController>();
        Slime.DeActivateRb();
        StartCoroutine(ResetSlimeReady());
    }


    private IEnumerator ResetSlimeReady()
    {
        yield return new WaitForSeconds(1f);  // 1초 기다리기
        isSlimeReady = true;  // 슬라임 준비 상태를 true로 변경
    }

    private IEnumerator GeneRes()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_resInterval);
            AddResource(m_resPerSec);
            m_resText.text = m_nowRes.ToString();
        }
    }

    private void AddResource(int amount)
    {
        m_nowRes += amount;
        if (m_nowRes > m_maxRes)
        {
            m_nowRes = m_maxRes;
        }
    }

    public void ResetResource()
    {
        m_nowRes = 0;
    }


}
