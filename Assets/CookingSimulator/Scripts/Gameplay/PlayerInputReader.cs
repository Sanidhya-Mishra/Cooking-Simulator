using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputAction move;
        [SerializeField] private InputAction interact;
        [SerializeField] private InputAction pause;
        [SerializeField] private InputAction drop;
        [SerializeField] private InputAction toggleRecipes;

        private bool movementSuppressed;
        public Vector2 Move { get; private set; }

        public event Action Interacted;
        public event Action PauseToggled;
        public event Action Dropped;
        public event Action RecipesToggled;

        private void OnEnable()
        {
            move.Enable();
            interact.Enable();
            pause.Enable();
            drop.Enable();
            toggleRecipes.Enable();

            interact.performed += OnInteract;
            pause.performed += OnPause;
            drop.performed += OnDrop;
            toggleRecipes.performed += OnToggleRecipes;
        }

        private void OnDisable()
        {
            interact.performed -= OnInteract;
            pause.performed -= OnPause;
            drop.performed -= OnDrop;
            toggleRecipes.performed -= OnToggleRecipes;

            move.Disable();
            interact.Disable();
            pause.Disable();
            drop.Disable();
            toggleRecipes.Disable();
            Move = Vector2.zero;
        }

        private void Update() => Move = movementSuppressed ? Vector2.zero : move.ReadValue<Vector2>();

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            Interacted?.Invoke();
        }

        private void OnPause(InputAction.CallbackContext ctx)
        {
            PauseToggled?.Invoke();
        }

        private void OnDrop(InputAction.CallbackContext ctx)
        {
            Dropped?.Invoke();
        }

        private void OnToggleRecipes(InputAction.CallbackContext ctx)
        {
            RecipesToggled?.Invoke();
        }

        public void SetMovementSuppressed(bool value)
        {
            movementSuppressed = value;
            if (value) Move = Vector2.zero;
        }
}
}