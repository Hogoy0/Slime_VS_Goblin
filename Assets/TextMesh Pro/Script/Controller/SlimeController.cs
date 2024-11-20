using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public SlimeData slimeData; // �������� ������
    private GameObject target; // ���� Ÿ�� (GameObject)
    private bool isAttacking; // ���� �� ����

    private void Start()
    {
        if (slimeData == null)
        {
            Debug.LogError($"������ �����Ͱ� null�Դϴ�: {gameObject.name}");
            return;
        }

        Initialize(slimeData);
    }

    public void Initialize(SlimeData data)
    {
        slimeData.m_currentHp = data.m_maxHp; // ü�� �ʱ�ȭ
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (target == null)
            {
                FindClosestTarget(); // Ÿ�� Ž��
            }

            if (target != null && Vector3.Distance(transform.position, target.transform.position) <= slimeData.m_stopDis)
            {
                Attack(); // ����
            }
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] goblins = GameObject.FindGameObjectsWithTag("Goblin");
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject goblin in goblins)
        {
            float distance = Vector3.Distance(transform.position, goblin.transform.position);
            if (distance < closestDistance && distance <= slimeData.m_searchLength)
            {
                closestDistance = distance;
                closestTarget = goblin;
            }
        }

        target = closestTarget;

        if (target != null)
        {
            Debug.Log($"�������� ��� �߰�: {target.name}");
        }
    }

    private void Attack()
    {
        if (isAttacking || target == null) return;

        isAttacking = true;

        // ������ ���
        var targetController = target.GetComponent<GoblinController>();
        if (targetController != null)
        {
            targetController.TakeDamage(slimeData.m_atk);
            Debug.Log($"�������� {target.name}��(��) ����! ������: {slimeData.m_atk}");
        }
        else
        {
            Debug.LogError("Ÿ�ٿ��� GoblinController�� ã�� �� �����ϴ�!");
        }

        Invoke(nameof(ResetAttack), slimeData.m_reAtkTime); // ���� ��Ÿ��
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        slimeData.m_currentHp -= damage;
        Debug.Log($"�������� ���ظ� ����: {damage}, ���� ü��: {slimeData.m_currentHp}");
        if (slimeData.m_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name}��(��) �׾����ϴ�.");
        Destroy(gameObject);
    }
}
