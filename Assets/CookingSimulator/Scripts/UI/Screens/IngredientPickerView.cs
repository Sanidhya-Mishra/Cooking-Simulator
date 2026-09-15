using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CookingSimulator.Scripts.Data;
using CookingSimulator.Scripts.Gameplay;

namespace CookingSimulator.Scripts.UI.Screens
{

    public sealed class IngredientPickerView : MonoBehaviour
    {
        [Serializable]
        private sealed class OptionSlot
        {
            public GameObject root;
            public Image icon;
            public TMP_Text label;
            public Button button;
        }

        [SerializeField] private GameManager game;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private GameObject root;
        [SerializeField] private OptionSlot[] slots;
        [SerializeField] private Button closeButton;

        private PlayerHands hands;
        private bool isOpen;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            if (root == gameObject)
                Debug.LogError($"[{name}] root must be a child object.", this);

            root.SetActive(false);
            if (closeButton != null) closeButton.onClick.AddListener(() => Close(true));
            if (game != null) game.StateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            if (game != null) game.StateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {

            if (isOpen && state != GameState.Playing)
                Close(reenableInput: false);
        }

        public void Open(IngredientDefinition[] offerings, PlayerHands targetHands)
        {
            if (isOpen || offerings == null || offerings.Length == 0) return;
            if (game != null && game.State != GameState.Playing) return;

            hands = targetHands;
            isOpen = true;
            root.SetActive(true);

            for (int i = 0; i < slots.Length; i++)
            {
                bool used = i < offerings.Length;
                slots[i].root.SetActive(used);
                if (!used) continue;

                IngredientDefinition def = offerings[i];
                slots[i].icon.color = def.TickColor;
                slots[i].label.text = def.DisplayName;

                slots[i].button.onClick.RemoveAllListeners();
                slots[i].button.onClick.AddListener(() => Choose(def));
            }

            if (playerMovement != null) playerMovement.SetMovementEnabled(false);
            if (playerInteractor != null) playerInteractor.SetInteractionEnabled(false);
        }

        private void Choose(IngredientDefinition def)
        {
            if (hands != null) hands.TryGive(KitchenItem.FromIngredient(def));
            Close(reenableInput: true);
        }

        public void Close(bool reenableInput)
        {
            isOpen = false;
            hands = null;
            root.SetActive(false);

            if (!reenableInput) return;
            if (playerMovement != null) playerMovement.SetMovementEnabled(true);
            if (playerInteractor != null) playerInteractor.SetInteractionEnabled(true);
        }
    }
}
