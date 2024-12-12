using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour
{
    public SlimeData slimeData;      // ������ ������
    private GameObject target;       // ���� Ÿ��
    private bool isAttacking;        // ���� �� ����
    private HealthBarController healthBar;
    private Animator animator;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBarController>();
        animator = GetComponent<Animator>();

        if (healthBar == null)
        {
            Debug.LogError($"{gameObject.name}�� HealthBarController�� �����ϴ�!");
        }
        else
        {
            healthBar.UpdateHealthBar((int)slimeData.m_currentHp, (int)slimeData.m_maxHp);
            healthBar.gameObject.SetActive(true);  // �ʱ⿡�� Ȱ��ȭ ����
        }

        if (animator == null)
        {
            Debug.LogError($"{gameObject.name}�� Animator�� �����ϴ�!");
        }
    }

    private void Start()
    {
        if (slimeData == null)
        {
            Debug.LogError($"{gameObject.name}�� SlimeData�� ������� �ʾҽ��ϴ�!");
            return;
        }

        Initialize(slimeData);
    }

    public void Initialize(SlimeData data)
    {
        slimeData = data; // �ùٸ� ������ ������ �ʱ�ȭ
        slimeData.m_currentHp = data.m_maxHp; // ü�� �ʱ�ȭ
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
        
        // ���� ��� �ð� (�ִϸ��̼ǰ� ����ȭ �ʿ� �� �ִϸ��̼� �̺�Ʈ ���)
        yield return new WaitForSeconds(GManager.Instance.SlimeAttackDelay);
        SoundManager.instance.PlaySlimeESfx(SoundManager.SlimeESfx.Slime_Attack);
        animator.SetTrigger("Attack");
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= slimeData.m_stopDis)
        {
            DealDamageToTarget();
        }

        yield return new WaitForSeconds(slimeData.m_reAtkTime); // ���� ��Ÿ��

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
            healthBar.gameObject.SetActive(true);  // HP�� ǥ�� Ȱ��ȭ
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
