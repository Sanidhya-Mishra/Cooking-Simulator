using System;
using UnityEngine;
using UnityEngine.UI;
using CookingSimulator.Scripts.Gameplay;

namespace CookingSimulator.Scripts.UI.Screens
{
    public sealed class ControlPanel : MonoBehaviour
    {
        [SerializeField] private GameManager game;
        [SerializeField] private GameObject root;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if(root== gameObject)Debug.LogError($"[{name}] root must be a child object.", this);

            game.StateChanged += OnStateChanged;
            startButton.onClick.AddListener(game.StartGame);
            quitButton.onClick.AddListener(game.QuitGame);
        }

        private void OnDestroy()=> game.StateChanged -= OnStateChanged;
        private void OnStateChanged(GameState state) => root.SetActive(state == GameState.Menu);
        
    }
}