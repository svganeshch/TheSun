using UnityEngine;
using UnityEngine.Events;

public interface IPlayer
{
    float Health { get; set; }
    float HealthMax { get; set; }
    float Speed { get; set; }
    float DashSpeed { get; set; }
    float DashTime { get; set; }
    Animator PlayerAnimator { get; set; }
    Transform PlayerPosition { get; set; }
    Rigidbody2D PlayerRigidbody { get; set; }

    void UpdateHealth(float damage);
}
