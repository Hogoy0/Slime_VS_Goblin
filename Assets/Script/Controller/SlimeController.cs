using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour
{
    public SlimeData slimeData;      // ������ ������
    private GameObject target;       // ���� Ÿ��
    private bool isAttacking;        // ���� �� ����

    private void Start()
    {
        if (slimeData == null) return;

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
                StartCoroutine(PerformAttackWithDelay());
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
    }

    private IEnumerator PerformAttackWithDelay()
    {
        if (isAttacking || target == null) yield break;

        isAttacking = true;
        
        // ���� �غ� �ð� ���
        yield return new WaitForSeconds(GManager.Instance.SlimeAttackDelay);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger("Attack");
        // Ÿ���� ������ ��ȿ���� Ȯ��
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= slimeData.m_stopDis)
        {
            var targetController = target.GetComponent<GoblinController>();
            if (targetController != null)
            {
                targetController.TakeDamage(slimeData.m_atk);
            }
        }
        
        yield return new WaitForSeconds(slimeData.m_reAtkTime); // ���� ��Ÿ��

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        slimeData.m_currentHp -= damage;
        if (slimeData.m_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
