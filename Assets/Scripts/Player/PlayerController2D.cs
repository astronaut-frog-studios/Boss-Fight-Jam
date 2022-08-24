using UnityEngine;

namespace Player
{
    public class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private float speed;

        [Header("Jump")] [SerializeField] private float jumpSpeed = 30f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 1.8f;

        private Rigidbody2D rb;

        private Vector2 moveInput;
        private bool isFacingRight = true;

        private bool isJumping;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.velocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
            }

            CheckPlayerDirection();
        }

        private void FixedUpdate()
        {
            rb.velocity = moveInput;

            if (!isJumping) return;
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            isJumping = false;
        }

        private void CheckJumpGravity()
        {
            switch (rb.velocity.y)
            {
                case < 0:
                    rb.gravityScale = fallMultiplier;
                    return;
                case > 0 when !Input.GetButton("Jump"):
                    rb.gravityScale = lowJumpMultiplier;
                    return;
                default:
                    rb.gravityScale = 1f;
                    break;
            }
        }

        private void CheckPlayerDirection()
        {
            transform.localScale = isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

            if (moveInput.x > 0 && !isFacingRight)
            {
                isFacingRight = !isFacingRight;
            }
            else if (moveInput.x < 0 && isFacingRight)
            {
                isFacingRight = !isFacingRight;
            }
        }
    }
}