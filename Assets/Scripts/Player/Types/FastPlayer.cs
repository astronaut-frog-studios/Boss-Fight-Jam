using System.Collections;
using UnityEngine;

namespace Player.Types
{
    public class FastPlayer : PlayerController2D
    {
        [Header("Dash")] [SerializeField] private float dashSpeed;
        [SerializeField] private float dashTime;
        [SerializeField] private float dashCooldown;
        [SerializeField] private TrailRenderer dashTrail;
        private float initialSpeed, initialDashCooldown;

        protected override void Start()
        {
            base.Start();
            initialDashCooldown = dashCooldown;
            dashCooldown = 0;
        }

        protected override void Update()
        {
            base.Update();

            if (dashCooldown > 0)
            {
                dashCooldown -= Time.deltaTime;
            }

            if (CanDash)
            {
                StartCoroutine(Dash());
            }
        }

        private IEnumerator Dash()
        {
            var forceVector = new Vector2(moveInput.x * dashSpeed, rb.velocity.y);
            rb.AddForce(forceVector, ForceMode2D.Impulse);
            dashTrail.emitting = true;
            
            yield return new WaitForSeconds(dashTime);

            dashTrail.emitting = false;
            dashCooldown = initialDashCooldown;
        }

        private bool CanDash => IsGrounded() && Input.GetKeyDown(KeyCode.E) && moveInput.x != 0 && dashCooldown <= 0;
    }
}