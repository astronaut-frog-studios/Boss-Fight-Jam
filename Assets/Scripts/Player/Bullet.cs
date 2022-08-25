using System.Collections;
using UnityEngine;

namespace Player
{
    public class Bullet : MonoBehaviour
    {
        public Rigidbody2D rb;
        [SerializeField] private GameObject impactEffect;
        [SerializeField] private LayerMask layersToCollide;

        private float autoDestroyTime = 5f;

        private void Start()
        {
            StartCoroutine(DestroySelfAfterSeconds(autoDestroyTime));
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if ((layersToCollide.value & (1 << col.gameObject.layer)) > 0)
            {
                // BossHealth enemy = col.GetComponent<BossHealth>();
                // if (enemy != null)
                // {
                //     enemy.TakeDamage(damage);
                // }

                Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        private IEnumerator DestroySelfAfterSeconds(float destroyTime)
        {
            yield return new WaitForSeconds(destroyTime);
            Destroy(gameObject);
        }
    }
}