using System.Collections.Generic;
using UnityEngine;

namespace CookingSimulator.Scripts.Data
{
    [System.Serializable]
    public sealed class RecipeStep
    {
        [Tooltip("Which station does this step.")]
        [SerializeField] private PrepType action = PrepType.Chop;

        [Tooltip("Ingredient the player adds in this step. Leave empty to keep working on the food already on the station.")]
        [SerializeField] private IngredientDefinition ingredient;

        [SerializeField, Min(0f)] private float duration = 2f;

        [Tooltip("Optional. How the food looks once this step is done.")]
        [SerializeField] private Mesh meshAfterStep;
        [SerializeField] private Material materialAfterStep;

        public PrepType Action => action;
        public IngredientDefinition Ingredient => ingredient;
        public float Duration => duration;
        public Mesh MeshAfterStep => meshAfterStep;
        public Material MaterialAfterStep => materialAfterStep;

        public bool Matches(PrepType otherAction, IngredientDefinition otherIngredient)
        {
            return action == otherAction && ingredient == otherIngredient;
        }
    }

    [CreateAssetMenu(fileName = "RCP_NewRecipe", menuName = "CookingSimulator/Recipe Definition", order = 2)]
    public sealed class RecipeDefinition : ScriptableObject
    {
        [SerializeField] private DishDefinition dish;

        [Tooltip("Steps are done in this order. The first step must add an ingredient.")]
        [SerializeField] private List<RecipeStep> steps = new List<RecipeStep>();

        public DishDefinition Dish => dish;
        public int StepCount => steps.Count;
        public RecipeStep GetStep(int index) => steps[index];

        // true if both recipes did the same first few steps
        public bool StartsLike(RecipeDefinition other, int stepCount)
        {
            if (other == this) return true;
            if (other == null) return false;
            if (StepCount < stepCount || other.StepCount < stepCount) return false;

            for (int i = 0; i < stepCount; i++)
            {
                RecipeStep otherStep = other.GetStep(i);
                if (!steps[i].Matches(otherStep.Action, otherStep.Ingredient)) return false;
            }
            return true;
        }

        public Mesh GetMesh(int stepsDone)
        {
            for (int i = Mathf.Min(stepsDone, StepCount) - 1; i >= 0; i--)
            {
                if (steps[i].MeshAfterStep != null) return steps[i].MeshAfterStep;
            }
            return FirstIngredient() != null ? FirstIngredient().Mesh : null;
        }

        public Material GetMaterial(int stepsDone)
        {
            for (int i = Mathf.Min(stepsDone, StepCount) - 1; i >= 0; i--)
            {
                if (steps[i].MaterialAfterStep != null) return steps[i].MaterialAfterStep;
            }
            return FirstIngredient() != null ? FirstIngredient().Material : null;
        }

        private IngredientDefinition FirstIngredient()
        {
            return StepCount > 0 ? steps[0].Ingredient : null;
        }

        private void OnValidate()
        {
            if (dish == null)
                Debug.LogWarning($"[{name}] has no dish.", this);
            if (StepCount == 0 || steps[0].Ingredient == null)
                Debug.LogWarning($"[{name}] the first step needs an ingredient.", this);
        }
    }
}
