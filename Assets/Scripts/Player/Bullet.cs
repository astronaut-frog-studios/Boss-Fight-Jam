using UnityEngine;

namespace Player
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject impactEffect;

        private void Start()
        {
            rb.velocity = transform.right * speed;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Boss")) return;
        
            // BossHealth enemy = col.GetComponent<BossHealth>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(damage);
            // }

            // Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}