using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour
{
    public SlimeData slimeData;      // 슬라임 데이터
    private GameObject target;       // 현재 타겟
    private bool isAttacking;        // 공격 중 여부
    private HealthBarController healthBar;
    private Animator animator;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBarController>();
        animator = GetComponent<Animator>();

        if (healthBar == null)
        {
            Debug.LogError($"{gameObject.name}에 HealthBarController가 없습니다!");
        }
        else
        {
            healthBar.UpdateHealthBar((int)slimeData.m_currentHp, (int)slimeData.m_maxHp);
            healthBar.gameObject.SetActive(true);  // 초기에는 활성화 상태
        }

        if (animator == null)
        {
            Debug.LogError($"{gameObject.name}에 Animator가 없습니다!");
        }
    }

    private void Start()
    {
        if (slimeData == null)
        {
            Debug.LogError($"{gameObject.name}에 SlimeData가 연결되지 않았습니다!");
            return;
        }

        Initialize(slimeData);
    }

    public void Initialize(SlimeData data)
    {
        slimeData = data; // 올바른 슬라임 데이터 초기화
        slimeData.m_currentHp = data.m_maxHp; // 체력 초기화
        healthBar.UpdateHealthBar((int)slimeData.m_currentHp, (int)slimeData.m_maxHp);
    }

    private void Update()
    {
        if (!isAttacking && target == null)
        {
            FindClosestTarget();
        }

        if (!isAttacking && target != null && Vector3.Distance(transform.position, target.transform.position) <= slimeData.m_stopDis)
        {
            StartCoroutine(PerformAttackWithAnimation());
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

    private IEnumerator PerformAttackWithAnimation()
    {
        if (isAttacking || target == null) yield break;

        isAttacking = true;
        
        // 공격 대기 시간 (애니메이션과 동기화 필요 시 애니메이션 이벤트 사용)
        yield return new WaitForSeconds(GManager.Instance.SlimeAttackDelay);
        SoundManager.instance.PlaySlimeESfx(SoundManager.SlimeESfx.Slime_Attack);
        animator.SetTrigger("Attack");
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= slimeData.m_stopDis)
        {
            DealDamageToTarget();
        }

        yield return new WaitForSeconds(slimeData.m_reAtkTime); // 공격 쿨타임

        isAttacking = false;
    }

    public void DealDamageToTarget()
    {
        if (target == null) return;
        
        var targetController = target.GetComponent<GoblinController>();
        if (targetController != null)
        {
            targetController.TakeDamage((int)slimeData.m_atk);
        }
    }

    public void TakeDamage(int damage)
    {
        slimeData.m_currentHp -= damage;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar((int)slimeData.m_currentHp, (int)slimeData.m_maxHp);
            healthBar.gameObject.SetActive(true);  // HP바 표시 활성화
        }

        if (slimeData.m_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SoundManager.instance.PlaySlimeESfx(SoundManager.SlimeESfx.Slime_Death);
        Destroy(gameObject);
    }
}
