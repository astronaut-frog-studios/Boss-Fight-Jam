using UnityEngine;

namespace Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bullet;
        
        private void Update()
        {
            if (Input.GetButtonDown("Fire1")) //TODO: Change for new input system
            {
                Instantiate(bullet, firePoint.position, firePoint.rotation);
            }
        }
    }
}
