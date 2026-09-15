using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CookingSimulator.Scripts.Gameplay;

namespace CookingSimulator.Scripts.UI.Screens
{
    public sealed class HUDPanel : MonoBehaviour
    {
        [SerializeField] private GameManager game;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text highScoreLabel;
        [SerializeField] private TMP_Text timeLabel;
        [SerializeField] private Button pauseButton;

        private void Awake()
        {
            if (root == gameObject) Debug.LogError($"[{name}] root must be a child object.", this);
            game.StateChanged += OnStateChanged;
            game.TimeRemainingChanged += OnTimeChanged;
            game.Scores.ScoreChanged += OnScoreChanged;
            game.Scores.HighScoreChanged += OnHighScoreChanged;
            
            pauseButton.onClick.AddListener(game.PauseGame);
            OnScoreChanged(game.Scores.Score);
            OnHighScoreChanged(game.Scores.HighScore);
        }

        private void OnDestroy()
        {
            game.StateChanged -= OnStateChanged;
            game.TimeRemainingChanged -= OnTimeChanged;
            game.Scores.ScoreChanged -= OnScoreChanged;
            game.Scores.HighScoreChanged -= OnHighScoreChanged;   
        }
        
        private void OnStateChanged(GameState state) =>
        root.SetActive(state == GameState.Playing || state == GameState.Paused);
        private void OnScoreChanged(int value)=> scoreLabel.text = $"Score  {value}";
        private void OnHighScoreChanged(int value) => highScoreLabel.text = $"Best  {value}";

        private void OnTimeChanged(float seconds)
        {
            int total = Mathf.CeilToInt(seconds);
            timeLabel.text = $"{total/60:0}:{total%60:00}";
        }
    }
}