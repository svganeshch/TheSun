using UnityEngine;

public class TorchCollider : MonoBehaviour
{
    private LayerMask enemyLayer;
    private PolygonCollider2D torchCollider;
    private SpriteRenderer torchColliderSprite;
    private Vector3 enemyPos;
    public float maxDamage = 200;
    private float damage;

    public float damageTime = 0.5f;
    private float currentDamageTime;

    private RaycastHit2D detectEnemy;

    private IEnemy enemy;

    private WeaponTorch torchCharge;

    private void Awake()
    {
        torchCharge = GetComponentInParent<WeaponTorch>();
        torchCollider = GetComponentInChildren<PolygonCollider2D>();
        torchColliderSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void Start()
    {
        torchCollider.isTrigger= true;
        torchColliderSprite.enabled= false;
    }

    public void OnHitEnemy(Collider2D enemyCollider)
    {
        currentDamageTime += Time.deltaTime;
        if (currentDamageTime > damageTime)
        {
            if (enemyCollider.TryGetComponent<IEnemy>(out enemy))
            {
                enemy.UpdateHealth(damage);
            }
            currentDamageTime = 0;
            Debug.Log("hit enemy");
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (torchCharge.TorchCharge <= 0) return;

        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("Enemy inside torch...");
            detectEnemy = Physics2D.Raycast(transform.position, collider.transform.position - transform.position);

            if (detectEnemy.collider != null)
            {
                enemyPos = collider.transform.position;
                if (detectEnemy.distance > 2)
                    damage /= 2;
                else
                    damage = maxDamage;

                OnHitEnemy(collider);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, enemyPos - transform.position);
    }
}
