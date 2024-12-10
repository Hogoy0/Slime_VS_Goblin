using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour
{
    public SlimeData slimeData;      // 슬라임 데이터
    private GameObject target;       // 현재 타겟
    private bool isAttacking;        // 공격 중 여부

    private void Start()
    {
        if (slimeData == null) return;

        Initialize(slimeData);
    }

    public void Initialize(SlimeData data)
    {
        slimeData.m_currentHp = data.m_maxHp; // 체력 초기화
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (target == null)
            {
                FindClosestTarget(); // 타겟 탐색
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
        
        // 공격 준비 시간 대기
        yield return new WaitForSeconds(GManager.Instance.SlimeAttackDelay);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger("Attack");
        // 타겟이 여전히 유효한지 확인
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= slimeData.m_stopDis)
        {
            var targetController = target.GetComponent<GoblinController>();
            if (targetController != null)
            {
                targetController.TakeDamage(slimeData.m_atk);
            }
        }
        
        yield return new WaitForSeconds(slimeData.m_reAtkTime); // 공격 쿨타임

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
