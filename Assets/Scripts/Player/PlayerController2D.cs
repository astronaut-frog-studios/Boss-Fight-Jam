using System;
using UnityEngine;

namespace Player
{
    public class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private float speed;

        [Header("Jump")] [SerializeField] private float jumpSpeed = 30f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 1.8f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheckCenter;
        
        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;

        private Vector2 moveInput;
        private bool isFacingRight = true;

        private const float groundedSize = 0.025f;
        private bool isJumping;
        private Vector2 groundBoxSize;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            groundBoxSize = new Vector2(boxCollider.size.x, groundedSize);
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

            if (isJumping && IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
                isJumping = false;
            }

            CheckJumpGravity();
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

        private bool IsGrounded()
        {
            return Physics2D.OverlapBox(groundCheckCenter.position, groundBoxSize, 0f, groundLayer);
        }
    }
}