using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Infra.Gameplay.UI;
using UnityEngine.UI;

namespace BatCave {
    /// <summary>
    /// The Game Manager.
    /// Allows starting and restarting the game.
    /// </summary>
    public class GameManager : MonoSingleton<GameManager> {
        
        public static event Action OnGameStarted;
        public static event Action OnGameReset;

        [SerializeField] private GameObject startGameUi;
        [SerializeField] private GameObject gameOverUi;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text bestScoreText;

        [Header("Read Only")]
        [SerializeField] bool hasStarted;
        [SerializeField] bool isGameOver;

        private int score;
        private int bestScore;

        public int Score
        {
            get => score;
            set => score = value;
        }

        public bool HasStarted {
            get {
                return hasStarted;
            }
        }

        public bool IsGameOver => isGameOver;

        private Transform cameraTransform;
        private Vector3 cameraInitialPosition;

        protected override void Init() {
            GameInputCapture.OnTouchDown += OnTouchDown;

            startGameUi.SetActive(true);
            gameOverUi.SetActive(false);
            
            SetScore(score);
            bestScore = PlayerPrefs.GetInt("BestScore", 0);
            SetBestScore(bestScore);

            // FindObjectOfType is expensive, but we can do it here because Init is
            // called only once.
            var sceneCamera = FindObjectOfType<Camera>();
            cameraTransform = sceneCamera.transform;
            cameraInitialPosition = cameraTransform.position;
        }

        protected void OnDestroy() {
            GameInputCapture.OnTouchDown -= OnTouchDown;
        }

        protected void Update() {
            if (hasStarted)
            {
                SetScore(score);
                return;
            }

            // Handle keyboard input.
            if (Input.GetKeyDown(KeyCode.Space)) {
                StartNewGame();
            }
        }

        public void OnGameOver() {
            hasStarted = false;
            isGameOver = true;
            startGameUi.SetActive(false);
            gameOverUi.SetActive(true);
            if (score > bestScore)
            {
                bestScore = score;
                SetBestScore(bestScore);
                PlayerPrefs.SetInt("BestScore", bestScore);
            }

        }

        private void OnTouchDown(PointerEventData e) {
            if (hasStarted) return;
            if (!StartNewGame()) {
                // Don't let the bat handle this event.
                e.Use();
            }
        }

        private bool StartNewGame() {
            if (isGameOver) {
                ResetGame();
                return false;
            }

            hasStarted = true;
            isGameOver = false;
            startGameUi.SetActive(false);
            gameOverUi.SetActive(false);
            

            OnGameStarted?.Invoke();
            return true;
        }

        private void ResetGame() {
            hasStarted = false;
            isGameOver = false;
            startGameUi.SetActive(true);
            gameOverUi.SetActive(false);

            cameraTransform.position = cameraInitialPosition;

            score = 0;
            SetScore(score);

            OnGameReset?.Invoke();
        }

        private void SetScore(int score)
        {
            scoreText.text = "Score: " + score;
        }

        private void SetBestScore(int score)
        {
            bestScoreText.text = "Best: " + score;
        }
    }
}
