public interface ICombatUnit
{
    // 현재 체력
    int Health { get; }

    // 공격 사거리
    float m_stopDis { get; }

    // 탐지 범위
    float m_searchLength { get; }

    // 데미지를 받는 메서드
    void TakeDamage(int damage);

    // 공격하는 메서드
    void Attack(ICombatUnit target);

    // 사망 처리
    void Die();
}
