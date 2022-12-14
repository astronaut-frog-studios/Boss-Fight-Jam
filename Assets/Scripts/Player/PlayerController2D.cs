using System;
using UnityEngine;

namespace Player
{
    public abstract class PlayerController2D : MonoBehaviour
    {
        public bool isMaster;
        public bool grounded;

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
        public bool canLockRotation;

        private const float groundedSize = 0.025f;
        private float lastGroundedTime;
        private float lastJumpTime;
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
            grounded = IsGrounded();
            
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), rb.velocity.y);
            if (!isMaster) return;

            TimersCheck();
            
            if (JumpInputPressed() && !isJumping)
            {
                lastJumpTime = jumpInputTime;
            }

            CheckPlayerDirection();
        }

        protected virtual void FixedUpdate()
        {
            if (!isMaster) return;

            Run();

            if (CanJump())
            {
                Jump();
            }

            CheckJumpValues();
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
            isJumping = true;
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            lastJumpTime = 0;
            lastGroundedTime = 0;
        }

        private void CheckJumpValues()
        {
            switch (rb.velocity.y)
            {
                case < 0 when lastGroundedTime <= 0:
                    rb.gravityScale = fallMultiplier;
                    return;
                case > 0 when !Input.GetButton("Jump"):
                    rb.gravityScale = lowJumpMultiplier;
                    return;
                case 0:
                    rb.gravityScale = 1f;
                    isJumping = false;
                    break;
            }
        }

        private void CheckPlayerDirection()
        {
            if (canLockRotation) return;

            transform.localRotation = isFacingRight ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 180f, 0f, 0f);

            if (moveInput.x > 0 && !isFacingRight)
            {
                isFacingRight = !isFacingRight;
            }
            else if (moveInput.x < 0 && isFacingRight)
            {
                isFacingRight = !isFacingRight;
            }
        }

        public void LockPlayerRotation(bool isLocked) => canLockRotation = isLocked;

        #region Inputs

        private static bool JumpInputPressed()
        {
            return Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0;
        }

        private static bool InteractInput()
        {
            return Input.GetKeyDown(KeyCode.E);
        }

        protected static bool SkillInputPressed()
        {
            return Input.GetKeyDown(KeyCode.LeftShift);
        }

        protected static bool SkillInputHold()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }

        protected static bool SkillInputReleased()
        {
            return Input.GetKeyUp(KeyCode.LeftShift);
        }

        protected static bool ShootInput()
        {
            return Input.GetButtonDown("Fire2");
        }

        private static bool MeleeAttackInput()
        {
            return Input.GetButtonDown("Fire1");
        }

        #endregion

        protected bool IsGrounded()
        {
            return !isJumping && Physics2D.OverlapBox(groundCheckCenter.position, groundBoxSize, 0f, groundLayer);
        }

        private bool CanJump()
        {
            return lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping;
        }

        protected virtual void OnTriggerEnter2D(Collider2D col)
        {
            print("triggerEnter");
        }
        
        protected virtual void OnTriggerStay2D(Collider2D col)
        {
            print("triggerStay");
        }
        
        protected virtual void OnTriggerExit2D(Collider2D col)
        {
            print("triggerExit");
        }
    }
}