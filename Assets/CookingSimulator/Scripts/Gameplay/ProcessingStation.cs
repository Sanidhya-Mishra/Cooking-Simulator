using UnityEngine;
using CookingSimulator.Scripts.Data;
using CookingSimulator.Scripts.UI.World;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class ProcessingStation : StationBase
    {
        [System.Serializable]
        private sealed class SlotBinding
        {
            public Transform anchor;
            public Ingredientview view;
            public StationProgressView progress;
        }

        private sealed class Slot
        {
            public KitchenItem item;
            public bool hasItem;
            public bool isWorking;
            public float timeLeft;
            public float stepTime;
        }

        [SerializeField] private StationConfig config;
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private SlotBinding[] slotBindings;

        private Slot[] slots;

        private void Awake()
        {
            if (config == null || recipeBook == null)
            {
                Debug.LogError($"[{name}] needs a StationConfig and a RecipeBook. Disabling", this);
                enabled = false;
                return;
            }

            int bindingCount = slotBindings != null ? slotBindings.Length : 0;
            int usable = Mathf.Min(config.SlotCount, bindingCount);

            if (usable != config.SlotCount || usable != bindingCount)
            {
                Debug.LogError($"[{name}] has {bindingCount} slot bindings but its config declares " +
                               $"{config.SlotCount}. Running with {usable}. Fix the prefab.", this);
            }

            if (usable == 0)
            {
                enabled = false;
                return;
            }

            slots = new Slot[usable];
            for (int i = 0; i < usable; i++)
            {
                slots[i] = new Slot();
                slotBindings[i].view.Hide();
                slotBindings[i].progress.Hide();
            }
        }

        private void OnValidate()
        {
            if (config == null || slotBindings == null) return;
            if (slotBindings.Length != config.SlotCount)
            {
                Debug.LogWarning($"[{name}] has {slotBindings.Length} slot bindings but its config " +
                                 $"declares {config.SlotCount}. These must match.", this);
            }
        }

        private void Update()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                Slot slot = slots[i];
                if (!slot.isWorking) continue;

                slot.timeLeft -= Time.deltaTime;
                if (slot.timeLeft > 0f)
                {
                    slotBindings[i].progress.ShowProgress(1f - slot.timeLeft / slot.stepTime, slot.timeLeft);
                }
                else
                {
                    FinishStep(i);
                }
            }
        }

        public override bool CanInteract(PlayerHands hands)
        {
            if (!enabled) return false;
            if (hands.IsEmpty) return FindIdleSlot() >= 0;

            KitchenItem held = hands.Held;
            PrepType action = config.Performs;

            if (held.IsRawIngredient)
            {
                if (FindSlotToAddTo(held.Ingredient) >= 0) return true;
                return FindFreeSlot() >= 0 && recipeBook.FindRecipeToStart(action, held.Ingredient) != null;
            }

            return FindFreeSlot() >= 0 && recipeBook.HasNextStepAt(held, action);
        }

        public override void Interact(PlayerHands hands)
        {
            if (!CanInteract(hands)) return;

            if (hands.IsEmpty)
            {
                PickUp(hands);
                return;
            }

            KitchenItem held = hands.Held;
            PrepType action = config.Performs;

            if (held.IsRawIngredient)
            {
                // first try adding it to food that is already on the station
                int addIndex = FindSlotToAddTo(held.Ingredient);
                if (addIndex >= 0)
                {
                    KitchenItem food = slots[addIndex].item;
                    RecipeDefinition nextRecipe = recipeBook.FindNextRecipe(food, action, held.Ingredient);
                    hands.Take();
                    StartStep(addIndex, nextRecipe, food.StepsDone);
                    return;
                }

                RecipeDefinition startRecipe = recipeBook.FindRecipeToStart(action, held.Ingredient);
                hands.Take();
                StartStep(FindFreeSlot(), startRecipe, 0);
                return;
            }

            // half made food: put it down, start right away if nothing needs adding
            int freeIndex = FindFreeSlot();
            hands.Take();
            slots[freeIndex].item = held;
            slots[freeIndex].hasItem = true;
            slotBindings[freeIndex].view.Show(held);

            if (!TryContinue(freeIndex)) ShowIdle(freeIndex);
        }

        private void StartStep(int index, RecipeDefinition recipe, int stepsDone)
        {
            Slot slot = slots[index];
            RecipeStep step = recipe.GetStep(stepsDone);

            slot.item = new KitchenItem(null, recipe, stepsDone);
            slot.hasItem = true;
            slot.isWorking = true;
            slot.stepTime = step.Duration;
            slot.timeLeft = step.Duration;
            slotBindings[index].view.Show(slot.item);

            if (step.Duration <= 0f)
                FinishStep(index);
            else
                slotBindings[index].progress.ShowProgress(0f, step.Duration);
        }

        private void FinishStep(int index)
        {
            Slot slot = slots[index];
            KitchenItem done = new KitchenItem(null, slot.item.Recipe, slot.item.StepsDone + 1);

            slot.item = recipeBook.PreferFinishedDish(done);
            slot.isWorking = false;
            slotBindings[index].view.Show(slot.item);

            // don't auto continue a finished dish, the player might want to pick it up
            if (slot.item.IsDish || !TryContinue(index))
                ShowIdle(index);
        }

        // starts the next step if it happens here and needs no new ingredient
        private bool TryContinue(int index)
        {
            KitchenItem food = slots[index].item;
            RecipeDefinition recipe = recipeBook.FindNextRecipe(food, config.Performs, null);
            if (recipe == null) return false;

            StartStep(index, recipe, food.StepsDone);
            return true;
        }

        private void ShowIdle(int index)
        {
            KitchenItem food = slots[index].item;
            IngredientDefinition needed = recipeBook.FindIngredientNeededAt(food, config.Performs);

            if (food.IsDish)
                slotBindings[index].progress.ShowReady();
            else if (needed != null)
                slotBindings[index].progress.ShowMessage("+" + needed.DisplayName);
            else
                slotBindings[index].progress.ShowMessage("Done");
        }

        private void PickUp(PlayerHands hands)
        {
            int index = FindIdleSlot();
            if (index < 0) return;
            if (!hands.TryGive(slots[index].item)) return;

            slots[index] = new Slot();
            slotBindings[index].view.Hide();
            slotBindings[index].progress.Hide();
        }

        private int FindSlotToAddTo(IngredientDefinition ingredient)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].hasItem || slots[i].isWorking) continue;
                if (recipeBook.FindNextRecipe(slots[i].item, config.Performs, ingredient) != null) return i;
            }
            return -1;
        }

        private int FindFreeSlot()
        {
            for (int i = 0; i < slots.Length; i++)
                if (!slots[i].hasItem) return i;
            return -1;
        }

        private int FindIdleSlot()
        {
            for (int i = 0; i < slots.Length; i++)
                if (slots[i].hasItem && !slots[i].isWorking) return i;
            return -1;
        }
    }
}
