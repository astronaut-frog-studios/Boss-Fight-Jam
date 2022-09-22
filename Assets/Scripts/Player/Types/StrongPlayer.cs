using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Player.Types
{
    public class StrongPlayer : PlayerController2D
    {
        [Header("MasterPlayer")] [SerializeField]
        private UnityEvent onLockRotation;

        [SerializeField] private UnityEvent onUnlockRotation;

        [Header("Stamina")] [SerializeField] private float maxStamina;
        [SerializeField] private Transform objectAttach;
        [SerializeField] private Slider staminaSlider;

        [Header("pushPull")] [SerializeField] private float pushValue;
        [SerializeField] private float pullValue;

        [Header("Lift")] [SerializeField] private float liftValue;
        [SerializeField] private float liftSpeed;

        // [Header("RayShoot")] [SerializeField] private Transform firePoint;
        // [SerializeField] private GameObject rayPrefab;
        // [SerializeField] private float raySpeed = 20f;

        private float currentStamina;
        private GameObject carryObj;
        private Vector2 carryObjPos;
        private bool canRestoreStamina, canPushPull, canLift, triggerExitCall;

        protected override void Start()
        {
            currentStamina = maxStamina;
            staminaSlider.maxValue = maxStamina;
            triggerExitCall = true;
        }

        protected override void Update()
        {
            base.Update();

            CheckStamina();

            if (!carryObj) return;
            if (SkillInputReleased() || currentStamina <= 0)
            {
                OnInputReleased();

                staminaSlider.gameObject.SetActive(false);
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                onUnlockRotation?.Invoke();

                canRestoreStamina = true;
                triggerExitCall = true;
                return;
            }

            if (SkillInputHold() && canPushPull)
            {
                carryObj.transform.SetParent(objectAttach);

                CheckMovingDirection();
                staminaSlider.gameObject.SetActive(true);

                if (isMaster)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                    onLockRotation?.Invoke();
                }

                canRestoreStamina = false;
                triggerExitCall = false;
                return;
            }

            if (SkillInputPressed() && canLift)
            {
                carryObjPos = carryObj.transform.position;
                carryObjPos += new Vector2(0, liftSpeed);
                carryObj.transform.position = carryObjPos;

                staminaSlider.gameObject.SetActive(true);

                if (isMaster)
                {
                    rb.isKinematic = true;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    onLockRotation?.Invoke();
                }

                canRestoreStamina = false;
                triggerExitCall = false;
            }

            if (canLift && !canRestoreStamina)
            {
                currentStamina -= liftValue * Time.deltaTime;
            }
        }

        private void CheckStamina()
        {
            staminaSlider.value = currentStamina;

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                return;
            }

            if (canRestoreStamina)
            {
                currentStamina += Time.deltaTime;
            }
        }

        private void CheckMovingDirection()
        {
            if (moveInput.x > 0)
            {
                currentStamina -= pushValue * Time.deltaTime;
            }

            if (moveInput.x < 0)
            {
                currentStamina -= pullValue * Time.deltaTime;
            }
        }

        private void OnInputReleased()
        {
            if (carryObj.CompareTag("PushPull"))
            {
                carryObj.transform.SetParent(null);
                return;
            }

            carryObjPos -= new Vector2(0, liftSpeed);
            carryObj.transform.position = carryObjPos;
            if (isMaster)
                rb.isKinematic = false;
        }

        private static bool CanSetCarryObj(Collider2D col) => col.CompareTag("PushPull") || col.CompareTag("Lift");

        protected override void OnTriggerStay2D(Collider2D col)
        {
            base.OnTriggerStay2D(col);

            if (!CanSetCarryObj(col) || carryObj || !IsGrounded()) return;

            carryObj = col.gameObject;
            if (col.CompareTag("PushPull"))
                canPushPull = true;
            if (col.CompareTag("Lift"))
                canLift = true;
        }

        protected override void OnTriggerExit2D(Collider2D col)
        {
            base.OnTriggerExit2D(col);

            if (!CanSetCarryObj(col) || !triggerExitCall) return;

            carryObj = null;
            if (col.CompareTag("PushPull"))
                canPushPull = false;
            if (col.CompareTag("Lift"))
                canLift = false;
        }
    }
}