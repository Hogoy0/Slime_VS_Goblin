using UnityEngine;
using System.Collections;

public class GoblinController : MonoBehaviour
{
    private GoblinData goblinData;
    private GameObject target;
    private bool isAttacking;

    /// <summary>
    /// ��� �ʱ�ȭ
    /// </summary>
    public void Initialize(GoblinData data, Vector3 position)
    {
        goblinData = data;
        goblinData.m_currentHp = data.m_maxHp;  // ü�� �ʱ�ȭ
        transform.position = position;
    }

    private void Update()
    {
        if (GManager.Instance.IsGameOver) return;

        // Ÿ�� Ž�� (������ > �� �켱)
        FindTarget();

        if (!isAttacking)
        {
            if (target != null)
            {
                float distanceToTarget = Mathf.Abs(transform.position.x - target.transform.position.x);

                // Ÿ���� ���� ���� �ȿ� ������ ����, �ƴϸ� �̵�
                if (distanceToTarget <= goblinData.m_stopDis)
                {
                    StartCoroutine(PerformAttack());
                }
                else
                {
                    MoveTowardsTarget();  // Ÿ���� ���� �̵�
                }
            }
            else
            {
                MoveLeft();  // �⺻ �̵� (����)
            }
        }
    }

    /// <summary>
    /// Ÿ�� Ž�� (������ > ��)
    /// </summary>
    private void FindTarget()
    {
        GameObject closestSlime = FindClosestSlime();
        if (closestSlime != null)
        {
            target = closestSlime;  // �������� ������ �켱 Ÿ�� ����
        }
        else
        {
            target = GameObject.FindWithTag("Castle");  // �������� ������ �� Ÿ�� ����
        }
    }

    /// <summary>
    /// ���� ����� ������ ã��
    /// </summary>
    private GameObject FindClosestSlime()
    {
        GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
        GameObject closestSlime = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject slime in slimes)
        {
            float distance = Mathf.Abs(transform.position.x - slime.transform.position.x);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSlime = slime;
            }
        }
        return closestSlime;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private IEnumerator PerformAttack()
    {
        if (isAttacking || target == null) yield break;

        isAttacking = true;

        if (target.CompareTag("Slime"))
        {
            var slimeController = target.GetComponent<SlimeController>();
            slimeController?.TakeDamage(goblinData.m_atk);  // ������ ����
        }
        else if (target.CompareTag("Castle"))
        {
            GManager.Instance.TakeDamageToCastle(goblinData.m_atk);  // �� ����
        }

        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("Attack", true);

        yield return new WaitForSeconds(goblinData.m_reAtkTime);  // ���� ��� �ð�
        animator.SetBool("Wait", false);
        animator.SetBool("Attack", false);

        isAttacking = false;
    }

    /// <summary>
    /// Ÿ���� ���� �̵�
    /// </summary>
    private void MoveTowardsTarget()
    {
        if (target == null) return;

        float step = goblinData.Speed * Time.deltaTime;
        Vector3 nextPosition = new Vector3(
            Mathf.MoveTowards(transform.position.x, target.transform.position.x, step),
            transform.position.y,
            transform.position.z
        );

        transform.position = nextPosition;
    }

    /// <summary>
    /// ���� �������� �̵� (�⺻ �̵�)
    /// </summary>
    private void MoveLeft()
    {
        float step = goblinData.Speed * Time.deltaTime;
        transform.Translate(Vector3.left * step);  // �������� �̵�
    }

    /// <summary>
    /// �������� ���� ��
    /// </summary>
    public void TakeDamage(int damage)
    {
        goblinData.m_currentHp -= damage;
        if (goblinData.m_currentHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ��� ó�� (StageManager�� ��� ��� �˸�)
    /// </summary>
    private void Die()
    {
        StageManager.Instance.HandleGoblinDeath(gameObject);  // ��� ���� �˸�
        Destroy(gameObject);  // ��� ����
    }
}
