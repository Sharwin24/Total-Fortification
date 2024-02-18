public interface ISoldier
{
    int Health { get; set; }
    int AttackPower { get; }
    void Attack(ISoldier target);
    void TakeDamage(int physicalDamage, int magicalDamage);
}