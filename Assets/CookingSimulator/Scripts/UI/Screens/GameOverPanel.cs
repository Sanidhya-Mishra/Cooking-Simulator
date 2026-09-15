using CookingSimulator.Scripts.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookingSimulator.Scripts.UI.Screens
{
    public sealed class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameManager game;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text finalScoreLabel;
        [SerializeField] private TMP_Text highScoreLabel;
        [SerializeField] private GameObject newHighScoreBanner;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if(root == gameObject)Debug.LogError($"[{name}] root must be a child object.", this);

            game.StateChanged += OnStateChanged;
            game.RoundEnded += OnRoundEnded;
            
            resetButton.onClick.AddListener(game.StartGame);
            quitButton.onClick.AddListener(game.QuitGame);
        }

        private void OnDestroy()
        {
            game.StateChanged -= OnStateChanged;
            game.RoundEnded -= OnRoundEnded;
        }
        
        private void OnStateChanged(GameState state) => root.SetActive(state == GameState.GameOver);

        private void OnRoundEnded(int finalScore, bool isNewHighScore)
        {
            finalScoreLabel.text = $"Final Score: {finalScore}";
            highScoreLabel.text = $"Best: {game.Scores.HighScore}";
            newHighScoreBanner.SetActive(isNewHighScore);
        }
    }
}