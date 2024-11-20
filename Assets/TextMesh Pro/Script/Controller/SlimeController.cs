using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public SlimeData slimeData; // 슬라임의 데이터
    private GameObject target; // 현재 타겟 (GameObject)
    private bool isAttacking; // 공격 중 여부

    private void Start()
    {
        if (slimeData == null)
        {
            Debug.LogError($"슬라임 데이터가 null입니다: {gameObject.name}");
            return;
        }

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
                Attack(); // 공격
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
            Debug.Log($"슬라임이 고블린 발견: {target.name}");
        }
    }

    private void Attack()
    {
        if (isAttacking || target == null) return;

        isAttacking = true;

        // 데미지 계산
        var targetController = target.GetComponent<GoblinController>();
        if (targetController != null)
        {
            targetController.TakeDamage(slimeData.m_atk);
            Debug.Log($"슬라임이 {target.name}을(를) 공격! 데미지: {slimeData.m_atk}");
        }
        else
        {
            Debug.LogError("타겟에서 GoblinController를 찾을 수 없습니다!");
        }

        Invoke(nameof(ResetAttack), slimeData.m_reAtkTime); // 공격 쿨타임
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        slimeData.m_currentHp -= damage;
        Debug.Log($"슬라임이 피해를 입음: {damage}, 남은 체력: {slimeData.m_currentHp}");
        if (slimeData.m_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 죽었습니다.");
        Destroy(gameObject);
    }
}
