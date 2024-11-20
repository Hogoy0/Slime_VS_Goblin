using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    private Transform target;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;
    [HideInInspector] public Vector3 pos { get { return transform.position; } }

    public GameObject prefabToSpawn;
    public string targetLayerName = "Ground";
    private float health;
    private int attackPower;
    private CData cData;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        if (rb != null)
        {
            DeActivateRb();
        }
    }

    /// <summary>
    /// CData를 기반으로 슬라임 초기화
    /// </summary>
    public void Initialize(CData data)
    {
        cData = data;
        health = data.m_maxHp;
        attackPower = data.m_atk;
    }

    public void Push(Vector2 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void ActivateRb()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    public void DeActivateRb()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            if (prefabToSpawn == null)
            {
                return;
            }
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            GameObject spawnedSlime = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            // 소환된 슬라임 초기화
            SummonController newSlimeController = spawnedSlime.GetComponent<SummonController>();
            if (newSlimeController != null && cData != null)
            {
                newSlimeController.Initialize(cData);
            }
            // 자기 자신을 제거
            Destroy(gameObject);
        }
    }
}
