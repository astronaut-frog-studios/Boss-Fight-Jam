using System;
using UnityEngine;

namespace Player
{
    public abstract class PlayerController2D : MonoBehaviour
    {
        [Header("Run")] [SerializeField] private float speed;
        [SerializeField, Range(0.01f, 10)] private float runAcceleration;
        [SerializeField, Range(0.01f, 10)] private float runDeceleration;

        [Header("Jump")] [SerializeField] private float jumpSpeed = 30f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 1.8f;
        [SerializeField, Range(0.01f, 0.5f)] private float coyoteTime;
        [SerializeField, Range(0.01f, 0.5f)] private float jumpInputTime;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheckCenter;

        protected Rigidbody2D rb;
        private BoxCollider2D boxCollider;

        protected Vector2 moveInput;
        private bool isFacingRight = true;

        private const float groundedSize = 0.025f;
        public float lastGroundedTime;
        public float lastJumpTime;
        private bool isJumping;
        private Vector2 groundBoxSize;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        protected virtual void Start()
        {
            groundBoxSize = new Vector2(boxCollider.size.x, groundedSize);
        }

        protected virtual void Update()
        {
            TimersCheck();

            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), rb.velocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                lastJumpTime = jumpInputTime;
            }

            CheckPlayerDirection();
        }

        protected virtual void FixedUpdate()
        {
            Run();

            if (CanJump())
            {
                Jump();
            }

            CheckJumpGravity();
        }

        private void TimersCheck()
        {
            lastGroundedTime -= Time.deltaTime;
            lastJumpTime -= Time.deltaTime;

            if (IsGrounded())
            {
                lastGroundedTime = coyoteTime;
            }
        }

        private void Run()
        {
            var targetSpeed = moveInput.x * speed;
            var speedDiff = targetSpeed - rb.velocity.x;
            var accelerationRate = (MathF.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDeceleration;
            var movement = speedDiff * accelerationRate;

            rb.AddForce(movement * Vector2.right);
        }

        private void Jump()
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            lastJumpTime = 0;
            isJumping = true;
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
                    isJumping = false;
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

        protected bool IsGrounded()
        {
            return Physics2D.OverlapBox(groundCheckCenter.position, groundBoxSize, 0f, groundLayer);
        }

        private bool CanJump()
        {
            return lastGroundedTime > 0 && lastJumpTime >= 0 && !isJumping;
        }
    }
}