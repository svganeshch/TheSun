using UnityEngine;

public class TorchCollider : MonoBehaviour
{
    private LayerMask enemyLayer;
    private PolygonCollider2D torchCollider;
    private SpriteRenderer torchColliderSprite;
    private RaycastHit2D detectEnemy;
    private Vector3 enemyPos;
    private float damage = 10;

    private IEnemy enemy;

    public virtual void Start()
    {
        torchCollider= GetComponentInChildren<PolygonCollider2D>();
        torchCollider.isTrigger= true;

        torchColliderSprite= GetComponentInChildren<SpriteRenderer>();
        torchColliderSprite.enabled= false;

        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public void OnHitEnemy()
    {
        if (detectEnemy.collider.TryGetComponent<IEnemy>(out enemy)) enemy.Health -= damage;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            //Debug.Log("Enemy inside torch...");
            detectEnemy = Physics2D.Raycast(transform.position, collider.transform.position - transform.position, 5f, enemyLayer);

            if (detectEnemy.collider != null)
            {
                enemyPos= collider.transform.position;
                OnHitEnemy();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, enemyPos - transform.position);
    }
}
