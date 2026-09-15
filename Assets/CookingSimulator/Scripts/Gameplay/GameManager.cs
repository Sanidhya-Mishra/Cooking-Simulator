using UnityEngine;
using System;
using CookingSimulator.Core;
using CookingSimulator.Scripts.Core;
using CookingSimulator.Scripts.Data;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig config;
        
        [Header("Scene References")]
        [SerializeField] private CustomerWindow[] windows;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private PlayerHands playerHands;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Transform playerSpawn;
        
        public OrderService Orders { get; private set; }
        public ScoreService Scores { get; private set; }
        public GameState State { get; private set; } = GameState.Menu;
        public float TimeRemaining{get; private set;}
        public GameConfig Config => config;

        public event Action<GameState> StateChanged;
        public event Action<float> TimeRemainingChanged;
        public event Action<int, bool> RoundEnded;

        private void Awake()
        {
            Orders = new OrderService(config);
            Scores = new ScoreService(new PlayerPrefsHighScoreRepository());
            if (windows.Length != config.WindowCount)
            {
                Debug.LogError($"[GameManager] {windows.Length} windows in the scene but " +
                               $"GameConfig.WindowCount is {config.WindowCount}.", this);
            }
            int bindable = Mathf.Min(windows.Length, Orders.SlotCount);
            for (int i = 0; i < bindable; i++)
                windows[i].Bind(Orders, i);
            Orders.OrderCompleted += OnOrderCompleted;
        }
private void OnEnable() => input.PauseToggled += OnPauseToggled;
        private void OnDisable() => input.PauseToggled -= OnPauseToggled;

        private void Start() => EnterState(GameState.Menu);

        private void OnDestroy()
        {
            if (Orders != null) Orders.OrderCompleted -= OnOrderCompleted;
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (State != GameState.Playing) return;

            float dt = Time.deltaTime;
            Orders.Tick(dt);

            TimeRemaining -= dt;
            TimeRemainingChanged?.Invoke(Mathf.Max(0f, TimeRemaining));

            if (TimeRemaining <= 0f) EndRound();
        }
        public void StartGame()
        {
            playerHands.Clear();
            if (playerSpawn != null) TeleportPlayer(playerSpawn.position);

            Scores.ResetRun();
            Orders.StartRound();

            TimeRemaining = config.RoundDuration;
            TimeRemainingChanged?.Invoke(TimeRemaining);

            EnterState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (State != GameState.Playing) return;
            EnterState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused) return;
            EnterState(GameState.Playing);
        }

        public void ReturnToMenu()
        {
            Orders.StopRound();
            EnterState(GameState.Menu);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnPauseToggled()
        {
            if (State == GameState.Playing) PauseGame();
            else if (State == GameState.Paused) ResumeGame();
        }

        private void OnOrderCompleted(int _, int score) => Scores.Add(score);

        private void EndRound()
        {
            TimeRemaining = 0f;
            Orders.StopRound();
            int finalScore = Scores.Score;
            bool isRecord = Scores.CommitRun();
            EnterState(GameState.GameOver);
            RoundEnded?.Invoke(finalScore, isRecord);
        }

        private void EnterState(GameState next)
        {
            State = next;
            bool playerActive = next == GameState.Playing;
            playerMovement.SetMovementEnabled(playerActive);
            playerInteractor.SetInteractionEnabled(playerActive);
            Time.timeScale = next == GameState.Playing ? 1f : 0f;
            StateChanged?.Invoke(next);
        }

        private void TeleportPlayer(Vector3 position)
        {
            CharacterController controller = playerMovement.GetComponent<CharacterController>();
            controller.enabled = false;
            playerMovement.transform.position = position;
            controller.enabled = true;
        }
    }
    
}