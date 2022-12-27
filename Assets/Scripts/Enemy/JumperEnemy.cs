public class JumperEnemy : EnemyController
{
    public override void InitialiseStart()
    {
        Health = 400;
        Speed = 2f;
        DetectionRange= 5;
    }
}
