using System;
using Infra.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using Infra.Gameplay.UI;

namespace BatCave {
    /// <summary>
    /// The Bat controller. Responsible for playing bat animations, handling collision
    /// with the cave walls and responding to player input.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public class Bat : MonoBehaviour
    {

        public static event Action OnBatPassedCave;
        
        [Header("Movement")]
        [SerializeField] float flyYSpeed;
        [SerializeField] float xSpeed;

        [Header("Animation")]
        [SerializeField] string flyUpBoolAnimParamName;
        [SerializeField] string isAliveBoolAnimParamName;

        [Header("State")]
        [SerializeField] bool isAlive;

        [Header("Testing")]
        [SerializeField] bool isInvulnerable;

        private bool FlyUp { get; set; }

        private int flyUpBoolAnimParamId;
        private int isAliveBoolAnimParamId;

        private Animator animator;
        private Rigidbody2D body;
        private RotateByVelocity velocityRotation;

        private Vector2 initialPosition;

        protected void Awake() {
            body = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            velocityRotation = GetComponent<RotateByVelocity>();

            flyUpBoolAnimParamId = Animator.StringToHash(flyUpBoolAnimParamName);
            isAliveBoolAnimParamId = Animator.StringToHash(isAliveBoolAnimParamName);

            GameInputCapture.OnTouchDown += OnTouchDown;
            GameInputCapture.OnTouchUp += OnTouchUp;

            GameManager.OnGameStarted += OnGameStarted;
            GameManager.OnGameReset += OnGameReset;

            initialPosition = body.position;
            OnGameReset();
        }

        protected void OnDestroy() {
            GameInputCapture.OnTouchDown -= OnTouchDown;
            GameInputCapture.OnTouchUp -= OnTouchUp;
            GameManager.OnGameStarted -= OnGameStarted;
            GameManager.OnGameReset -= OnGameReset;
        }

        protected void Update() {
            if (!isAlive) return;

            // Handle keyboard input.
            if (Input.GetKeyDown(KeyCode.Space)) {
                FlyUp = true;
            } else if (Input.GetKeyUp(KeyCode.Space)) {
                FlyUp = false;
            }
            animator.SetBool(flyUpBoolAnimParamId, FlyUp);
            animator.SetBool(isAliveBoolAnimParamId, isAlive);

            GameManager.Instance.Score =  (int) (transform.position.x - initialPosition.x) ;
        }

        protected void FixedUpdate() {
            if (!isAlive) return;
            var velocity = body.velocity;
            velocity.x = xSpeed;
            if (FlyUp) {
                velocity.y = flyYSpeed;
            }
            body.velocity = velocity;

            if (CaveManager.Instance.DidBatPassCave(transform.position.x))
            {
                OnBatPassedCave?.Invoke();
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision) {
            if (isInvulnerable) return;

            // Stop flying.
            FlyUp = false;
            body.velocity = Vector2.zero;

            // Play death animation.
            isAlive = false;
            animator.SetBool(isAliveBoolAnimParamId, isAlive);
            enabled = false;

            velocityRotation.enabled = false;
            GameManager.Instance.OnGameOver();
        }

        private void OnGameStarted() {
            // Let the bat fly!
            body.constraints = RigidbodyConstraints2D.None;
            velocityRotation.enabled = true;
        }

        private void OnGameReset() {
            // Stop the bat.
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            // Reset it's position.
            body.rotation = 0f;
            body.position = initialPosition;
            transform.position = initialPosition;
            // Prevent it from moving.
            body.constraints = RigidbodyConstraints2D.FreezeAll;

            // Reanimate the bat. Bring it back from the dead.
            isAlive = true;
            FlyUp = false;

            enabled = true;
            velocityRotation.enabled = true;
        }

        private void OnTouchDown(PointerEventData e) {
            // Check that the event was not already handled by the GameManager.
            if (!isAlive || e.used) return;
            FlyUp = true;
        }

        private void OnTouchUp(PointerEventData e) {
            FlyUp = false;
        }
    }
}
