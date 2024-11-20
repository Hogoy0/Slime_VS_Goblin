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
        goblinData.m_currentHp = data.m_maxHp; // 체력 초기화
        spawnPosition = position;

        // 데이터 초기화
        transform.position = position;
        Debug.Log($"고블린 초기화 완료: {data.m_name}, 위치: {position}");
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (target == null)
            {
                FindClosestTarget(); // 타겟 탐색
            }

            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 슬라임이 공격 범위 안에 들어왔으면 공격, 아니면 이동
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
                BasicMovement(); // 슬라임이 없으면 기본 이동
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
            Debug.Log($"고블린이 슬라임 발견: {target.name}");
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
            Debug.Log($"고블린이 {target.name}을(를) 공격! 데미지: {goblinData.m_atk}");
        }
        else
        {
            Debug.LogError("타겟에서 SlimeController를 찾을 수 없습니다!");
        }

        Invoke(nameof(ResetAttack), goblinData.m_reAtkTime); // 공격 쿨타임
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;

        // 고블린이 타겟을 향해 이동
        float step = goblinData.Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
    }

    private void BasicMovement()
    {
        // 슬라임이 없으면 왼쪽 방향으로 이동 (기본 이동)
        transform.Translate(Vector3.left * goblinData.Speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        goblinData.m_currentHp -= damage;
        Debug.Log($"고블린이 피해를 입음: {damage}, 남은 체력: {goblinData.m_currentHp}");
        if (goblinData.m_currentHp <= 0)
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