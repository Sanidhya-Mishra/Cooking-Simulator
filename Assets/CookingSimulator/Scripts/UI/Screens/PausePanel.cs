using UnityEngine;
using UnityEngine.UI;
using CookingSimulator.Scripts.Gameplay;

namespace CookingSimulator.Scripts.UI.Screens
{
    public sealed class PausePanel : MonoBehaviour
    {
        [SerializeField] private GameManager game;
        [SerializeField] private GameObject root;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if(root == gameObject)Debug.LogError($"[{name}] root must be a child object.", this);

            game.StateChanged += OnStateChanged;
            resumeButton.onClick.AddListener(game.ResumeGame);
            quitButton.onClick.AddListener(game.QuitGame);
        }
        private void OnDestroy()=> game.StateChanged -= OnStateChanged;
        private void OnStateChanged(GameState gameState) => root.SetActive(gameState == GameState.Paused);
        
    }
}