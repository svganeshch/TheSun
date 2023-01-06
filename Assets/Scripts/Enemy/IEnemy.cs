public interface IEnemy
{
    float Health { get; set; }
    float HealthMax { get; set; }
    float DamageAmount { get; set; }
    float Speed { get; set; }
    float DetectionRange { get; set; }
    void AttackPlayer() { }
    void EvadePlayer() { }

    void UpdateHealth(float damage) { }
    void DestroyEnemyObject();
}