using UnityEngine;

public class SummonController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;

    public GameObject prefabToSpawn;
    public string targetLayerName = "Ground";

    private float health;
    private int attackPower;

    public bool IsLaunched { get; set; } = false;
    public SlimeData SlimeData { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        DeActivateRb();
    }

    /// <summary>
    /// 슬라임 초기화
    /// </summary>
    public void Initialize(SlimeData data)
    {
        SlimeData = data;
        health = data.m_maxHp;
        attackPower = data.m_atk;
        IsLaunched = false;
    }

    /// <summary>
    /// 드래그 입력 처리
    /// </summary>
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
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName) && prefabToSpawn != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            GameObject spawnedSlime = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            SummonController newSlimeController = spawnedSlime.GetComponent<SummonController>();
            if (newSlimeController != null && SlimeData != null)
            {
                newSlimeController.Initialize(SlimeData);
            }

            Destroy(gameObject);
        }
    }
}
