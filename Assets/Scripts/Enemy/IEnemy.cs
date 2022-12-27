public interface IEnemy
{
    float Health { get; set; }
    float Speed { get; set; }
    float DetectionRange { get; set; }
    void ChasePlayer() { }
    void EvadePlayer() { }
    void DestroyEnemyObject();
}