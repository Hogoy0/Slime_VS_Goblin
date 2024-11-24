using UnityEngine;

public class GoblinController : MonoBehaviour
{
    private GoblinData goblinData;
    private Vector3 spawnPosition;
    private GameObject target;
    private bool isAttacking;

    public void Initialize(GoblinData data, Vector3 position)
    {
        goblinData = data;
        goblinData.m_currentHp = data.m_maxHp; // ü�� �ʱ�ȭ
        spawnPosition = position;

        // ������ �ʱ�ȭ
        transform.position = position;
        Debug.Log($"��� �ʱ�ȭ �Ϸ�: {data.m_name}, ��ġ: {position}");
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (target == null)
            {
                FindClosestTarget(); // Ÿ�� Ž��
            }

            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                // �������� ���� ���� �ȿ� �������� ����, �ƴϸ� �̵�
                if (distanceToTarget <= goblinData.m_stopDis)
                {
                    Attack();
                }
                else
                {
                    MoveTowardsTarget();
                }
            }
            else
            {
                BasicMovement(); // �������� ������ �⺻ �̵�
            }
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject slime in slimes)
        {
            float distance = Vector3.Distance(transform.position, slime.transform.position);
            if (distance < closestDistance && distance <= goblinData.m_searchLength)
            {
                closestDistance = distance;
                closestTarget = slime;
            }
        }

        target = closestTarget;

        if (target != null)
        {
            Debug.Log($"����� ������ �߰�: {target.name}");
        }
    }

    private void Attack()
    {
        if (isAttacking || target == null) return;

        isAttacking = true;

        var targetController = target.GetComponent<SlimeController>();
        if (targetController != null)
        {
            targetController.TakeDamage(goblinData.m_atk);
            Debug.Log($"����� {target.name}��(��) ����! ������: {goblinData.m_atk}");
        }
        else
        {
            Debug.LogError("Ÿ�ٿ��� SlimeController�� ã�� �� �����ϴ�!");
        }

        Invoke(nameof(ResetAttack), goblinData.m_reAtkTime); // ���� ��Ÿ��
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;

        // ����� Ÿ���� ���� �̵�
        float step = goblinData.Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
    }

    private void BasicMovement()
    {
        // �������� ������ ���� �������� �̵� (�⺻ �̵�)
        transform.Translate(Vector3.left * goblinData.Speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        goblinData.m_currentHp -= damage;
        Debug.Log($"����� ���ظ� ����: {damage}, ���� ü��: {goblinData.m_currentHp}");
        if (goblinData.m_currentHp <= 0)
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