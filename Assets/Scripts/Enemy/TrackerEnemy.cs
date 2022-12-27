public class TrackerEnemy : EnemyController
{
    public override void InitialiseStart()
    {
        Health = 200;
        Speed= 3f;
        DetectionRange= 5;
    }
}