using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;
    [HideInInspector] public Vector3 pos { get { return transform.position; }}

    public GameObject prefabToSpawn;
    public string targetLayerName = "Ground";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        DeActivateRb();

    }

    public void push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ActivateRb()
    {
        rb.isKinematic = false;
    }

    public void DeActivateRb()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        Debug.Log("������");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ������Ʈ�� ���̾ Ÿ�� ���̾����� Ȯ��
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            // ���� ��ġ�� ������ ��ȯ
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

            // �ڱ� �ڽ��� ����
            Destroy(gameObject);
        }
    }



}
