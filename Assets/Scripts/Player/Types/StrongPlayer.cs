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

        [Header("floats")] [SerializeField] private float pushValue;
        [SerializeField] private float pullValue;
        [SerializeField] private float liftValue;

        [SerializeField] private float distanceToCarryObj;

        // [Header("RayShoot")] [SerializeField] private Transform firePoint;
        // [SerializeField] private GameObject rayPrefab;
        // [SerializeField] private float raySpeed = 20f;

        private float currentStamina;
        private GameObject carryObj;
        private bool canRestoreStamina, canPushPull, canLift;

        protected override void Start()
        {
            currentStamina = maxStamina;
            staminaSlider.maxValue = maxStamina;
        }

        protected override void Update()
        {
            base.Update();

            CheckStamina();

            if (!carryObj) return;

            if (Vector2.Distance(transform.position, carryObj.transform.position) > distanceToCarryObj)
            {
                carryObj = null;
                canLift = false;
                canPushPull = false;
                return;
            }

            if (SkillInputReleased())
            {
                carryObj.transform.SetParent(null);

                staminaSlider.gameObject.SetActive(false);
                onUnlockRotation?.Invoke();

                canRestoreStamina = true;
                return;
            }

            if (SkillInputHold() && canPushPull)
            {
                carryObj.transform.SetParent(objectAttach);

                CheckMovingDirection();
                onLockRotation?.Invoke();
                staminaSlider.gameObject.SetActive(true);

                canRestoreStamina = false;
                return;
            }

            if (SkillInputPressed() && canLift)
            {
                //TODO: Move ojb ??? points up, on Y axis
                canRestoreStamina = false;
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

        private static bool CanSetCarryObj(Collider2D col) => col.CompareTag("PushPull") || col.CompareTag("Lift");

        protected override void OnTriggerEnter2D(Collider2D col)
        {
            base.OnTriggerEnter2D(col);

            if (!CanSetCarryObj(col)) return;

            carryObj = col.gameObject;

            if (col.CompareTag("PushPull"))
                canPushPull = true;
        }

        // protected override void OnTriggerExit2D(Collider2D col)
        // {
        //     base.OnTriggerExit2D(col);
        //
        //     if (!CanSetCarryObj(col)) return;
        //     carryObj = null;
        //
        //     if (col.CompareTag("PushPull"))
        //         canPushPull = false;
        // }
    }
}