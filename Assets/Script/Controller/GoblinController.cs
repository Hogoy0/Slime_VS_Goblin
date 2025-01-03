using UnityEngine;
using System.Collections;
using static GType;

public class GoblinController : MonoBehaviour
{
    private GoblinData goblinData;
    private GameObject target;
    private bool isAttacking;
    private HealthBarController healthBar;

    /// <summary>
    /// 고블린 초기화
    /// </summary>
    public void Initialize(GoblinData data, Vector3 position)
    {
        goblinData = data;
        goblinData.m_currentHp = data.m_maxHp;  // 체력 초기화
        transform.position = position;

        // HealthBarController 찾기 및 초기 설정
        healthBar = GetComponentInChildren<HealthBarController>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBarController가 고블린에 없습니다!");
        }
        else
        {
            healthBar.UpdateHealthBar((int)goblinData.m_currentHp, (int)goblinData.m_maxHp);
            healthBar.gameObject.SetActive(true);  // 초기에는 활성화 상태
        }
    }

    private void Update()
    {
        if (GManager.Instance.IsGameOver) return;

        // 타겟 탐색 (슬라임 > 성 우선)
        FindTarget();

        if (!isAttacking)
        {
            if (target != null)
            {
                float distanceToTarget = Mathf.Abs(transform.position.x - target.transform.position.x);

                // 타겟이 공격 범위 안에 있으면 공격, 아니면 이동
                if (distanceToTarget <= goblinData.m_stopDis)
                {
                    StartCoroutine(PerformAttack());
                }
                else
                {
                    MoveTowardsTarget();  // 타겟을 향해 이동
                }
            }
            else
            {
                MoveLeft();  // 기본 이동 (왼쪽)
            }
        }
    }

    /// <summary>
    /// 타겟 탐색 (슬라임 > 성)
    /// </summary>
    private void FindTarget()
    {
        GameObject closestSlime = FindClosestSlime();
        if (closestSlime != null)
        {
            target = closestSlime;  // 슬라임이 있으면 우선 타겟 설정
        }
        else
        {
            target = GameObject.FindWithTag("Castle");  // 슬라임이 없으면 성 타겟 설정
        }
    }

    /// <summary>
    /// 가장 가까운 슬라임 찾기
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
    /// 공격 수행
    /// </summary>
    private IEnumerator PerformAttack()
    {
        if (isAttacking || target == null) yield break;

        isAttacking = true;

        if (target.CompareTag("Slime"))
        {
            var slimeController = target.GetComponent<SlimeController>();
            slimeController?.TakeDamage(goblinData.m_atk);  // 슬라임 공격
            AttackSound();
        }
        else if (target.CompareTag("Castle"))
        {
            GManager.Instance.TakeDamageToCastle(goblinData.m_atk);  // 성 공격
            AttackSound();
        }

        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("Attack", true);


        yield return new WaitForSeconds(goblinData.m_reAtkTime);  // 공격 대기 시간
        animator.SetBool("Wait", false);
        animator.SetBool("Attack", false);

        isAttacking = false;
    }

    /// <summary>
    /// 타겟을 향해 이동
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
    /// 왼쪽 방향으로 이동 (기본 이동)
    /// </summary>
    private void MoveLeft()
    {
        float step = goblinData.Speed * Time.deltaTime;
        transform.Translate(Vector3.left * step);  // 왼쪽으로 이동
    }

    /// <summary>
    /// 데미지를 받을 때
    /// </summary>
    public void TakeDamage(int damage)
    {
        goblinData.m_currentHp -= damage;
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar((int)goblinData.m_currentHp, (int)goblinData.m_maxHp);
            healthBar.gameObject.SetActive(true);  // HP바 표시 활성화
        }

        if (goblinData.m_currentHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 사망 처리 (StageManager에 고블린 사망 알림)
    /// </summary>
    private void Die()
    {
        StageManager.Instance.HandleGoblinDeath(gameObject);  // 고블린 제거 알림
        SoundManager.instance.PlayGoblinESfx(SoundManager.GoblinESfx.Goblin_Death);
        Destroy(gameObject);  // 고블린 제거
    }

    private void AttackSound()
    {
        switch (goblinData.goblinType)
        {
            case GType.GoblinType.Shovels:
                SoundManager.instance.PlayGoblinESfx(SoundManager.GoblinESfx.Goblin_DiggingTools);
                break;
            case GType.GoblinType.Pickax:
                SoundManager.instance.PlayGoblinESfx(SoundManager.GoblinESfx.Goblin_DiggingTools);
                break;
            case GType.GoblinType.Drill:
                SoundManager.instance.PlayGoblinESfx(SoundManager.GoblinESfx.Goblin_Drill);
                break;
            case GType.GoblinType.Chief:
                SoundManager.instance.PlayGoblinESfx(SoundManager.GoblinESfx.Goblin_Shief);
                break;
            case GType.GoblinType.Bomb:
                SoundManager.instance.PlayGoblinESfx(SoundManager.GoblinESfx.Goblin_Bomb);
                break;

        }
    }
}

