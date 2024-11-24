public interface ICombatUnit
{
    // ���� ü��
    int Health { get; }

    // ���� ��Ÿ�
    float m_stopDis { get; }

    // Ž�� ����
    float m_searchLength { get; }

    // �������� �޴� �޼���
    void TakeDamage(int damage);

    // �����ϴ� �޼���
    void Attack(ICombatUnit target);

    // ��� ó��
    void Die();
}
