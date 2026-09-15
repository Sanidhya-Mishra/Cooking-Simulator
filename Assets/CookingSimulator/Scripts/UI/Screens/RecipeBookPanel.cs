using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CookingSimulator.Scripts.Data;
using CookingSimulator.Scripts.Gameplay;

namespace CookingSimulator.Scripts.UI.Screens
{
    public sealed class RecipeBookPanel : MonoBehaviour
    {
        [SerializeField] private GameManager game;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private GameObject root;

        [Header("Buttons")]
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        [Header("Dish List")]
        [SerializeField] private Transform dishListParent;
        [SerializeField] private Button dishButtonTemplate;

        [Header("Details")]
        [SerializeField] private TMP_Text dishNameLabel;
        [SerializeField] private TMP_Text instructionsLabel;

        private readonly List<Button> dishButtons = new List<Button>();
        private bool isOpen;

        private void Awake()
        {
            if (root == gameObject) Debug.LogError($"[{name}] root must be a child object.", this);

            root.SetActive(false);
            dishButtonTemplate.gameObject.SetActive(false);

            if (openButton != null) openButton.onClick.AddListener(Toggle);
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            game.StateChanged += OnStateChanged;

            BuildDishList();
        }

        private void OnEnable()
        {
            if (input != null) input.RecipesToggled += Toggle;
        }

        private void OnDisable()
        {
            if (input != null) input.RecipesToggled -= Toggle;
        }

        private void OnDestroy()
        {
            game.StateChanged -= OnStateChanged;
        }

        public void Toggle()
        {
            if (isOpen) Close();
            else Open();
        }

        public void Open()
        {
            if (game.State != GameState.Playing && game.State != GameState.Paused) return;
            isOpen = true;
            root.SetActive(true);
        }

        public void Close()
        {
            isOpen = false;
            root.SetActive(false);
        }

        private void OnStateChanged(GameState state)
        {
            if (state != GameState.Playing && state != GameState.Paused) Close();
        }

        private void BuildDishList()
        {
            RecipeBook book = game.Config.RecipeBook;
            if (book == null)
            {
                dishNameLabel.text = "No Recipes";
                instructionsLabel.text = "GameConfig has no Recipe Book assigned.";
                return;
            }

            RecipeDefinition firstRecipe = null;

            foreach (RecipeDefinition recipe in book.Recipes)
            {
                if (recipe == null || recipe.Dish == null) continue;

                Button button = Instantiate(dishButtonTemplate, dishListParent);
                button.gameObject.SetActive(true);
                button.name = "Dish_" + recipe.Dish.DisplayName;
                button.GetComponentInChildren<TMP_Text>().text = recipe.Dish.DisplayName;

                RecipeDefinition thisRecipe = recipe;
                button.onClick.AddListener(() => ShowRecipe(thisRecipe, button));
                dishButtons.Add(button);

                if (firstRecipe == null) firstRecipe = recipe;
            }

            if (firstRecipe != null) ShowRecipe(firstRecipe, dishButtons[0]);
        }

        private void ShowRecipe(RecipeDefinition recipe, Button selected)
        {
            foreach (Button button in dishButtons)
                button.interactable = button != selected;

            dishNameLabel.text = recipe.Dish.DisplayName;

            StringBuilder text = new StringBuilder();
            text.AppendLine($"<color=#FFD24D>Score: {recipe.Dish.ScoreValue}</color>");
            text.AppendLine();

            for (int i = 0; i < recipe.StepCount; i++)
            {
                text.AppendLine($"<b>{i + 1}.</b> {DescribeStep(recipe, i)}");
                text.AppendLine();
            }

            text.Append("Pick it up and deliver it to a customer window.");
            instructionsLabel.text = text.ToString();
        }

        private string DescribeStep(RecipeDefinition recipe, int index)
        {
            RecipeStep step = recipe.GetStep(index);
            string station = StationName(step.Action);
            string verb = Verb(step.Action);
            string time = step.Duration > 0f ? $" <color=#9AD1FF>({step.Duration:0.#}s)</color>" : "";
            bool sameStationAsBefore = index > 0 && recipe.GetStep(index - 1).Action == step.Action;

            if (index == 0 && step.Ingredient != null)
                return $"Take <b>{step.Ingredient.DisplayName}</b> to the {station} and {verb} it{time}";

            if (step.Ingredient == null)
            {
                if (sameStationAsBefore) return $"Leave it on the {station} to {verb} some more{time}";
                return $"Carry it to the {station} and {verb} it{time}";
            }

            if (sameStationAsBefore)
                return $"Bring <b>{step.Ingredient.DisplayName}</b> to the {station} and add it{time}";

            return $"Carry it to the {station}, then add <b>{step.Ingredient.DisplayName}</b>{time}";
        }

        private string StationName(PrepType action)
        {
            switch (action)
            {
                case PrepType.Chop: return "Chopping Table";
                case PrepType.Cook: return "Stove";
                default: return action.ToString();
            }
        }

        private string Verb(PrepType action)
        {
            switch (action)
            {
                case PrepType.Chop: return "chop";
                case PrepType.Cook: return "cook";
                default: return action.ToString().ToLower();
            }
        }
    }
}
