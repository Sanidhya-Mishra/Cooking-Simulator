using System.Collections.Generic;
using UnityEngine;

namespace CookingSimulator.Scripts.Data
{
    [CreateAssetMenu(fileName = "RB_RecipeBook", menuName = "CookingSimulator/Recipe Book", order = 3)]
    public sealed class RecipeBook : ScriptableObject
    {
        [Tooltip("Every recipe in the game. Customers can order any dish made by these.")]
        [SerializeField] private List<RecipeDefinition> recipes = new List<RecipeDefinition>();

        public IReadOnlyList<RecipeDefinition> Recipes => recipes;

        public RecipeDefinition FindRecipeToStart(PrepType action, IngredientDefinition ingredient)
        {
            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe == null || recipe.StepCount == 0) continue;
                if (recipe.GetStep(0).Matches(action, ingredient)) return recipe;
            }
            return null;
        }

        // looks for a recipe made the same way so far whose next step is this action + ingredient
        public RecipeDefinition FindNextRecipe(KitchenItem item, PrepType action, IngredientDefinition ingredient)
        {
            if (item.Recipe == null) return null;

            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe == null || recipe.StepCount <= item.StepsDone) continue;
                if (!recipe.StartsLike(item.Recipe, item.StepsDone)) continue;
                if (recipe.GetStep(item.StepsDone).Matches(action, ingredient)) return recipe;
            }
            return null;
        }

        public bool HasNextStepAt(KitchenItem item, PrepType action)
        {
            if (item.Recipe == null) return false;

            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe == null || recipe.StepCount <= item.StepsDone) continue;
                if (!recipe.StartsLike(item.Recipe, item.StepsDone)) continue;
                if (recipe.GetStep(item.StepsDone).Action == action) return true;
            }
            return false;
        }

        public IngredientDefinition FindIngredientNeededAt(KitchenItem item, PrepType action)
        {
            if (item.Recipe == null) return null;

            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe == null || recipe.StepCount <= item.StepsDone) continue;
                if (!recipe.StartsLike(item.Recipe, item.StepsDone)) continue;

                RecipeStep step = recipe.GetStep(item.StepsDone);
                if (step.Action == action && step.Ingredient != null) return step.Ingredient;
            }
            return null;
        }

        // if the food is already a finished dish in some recipe, use that recipe
        public KitchenItem PreferFinishedDish(KitchenItem item)
        {
            if (item.Recipe == null || item.IsDish) return item;

            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe == null || recipe.StepCount != item.StepsDone) continue;
                if (recipe.StartsLike(item.Recipe, item.StepsDone))
                    return new KitchenItem(null, recipe, item.StepsDone);
            }
            return item;
        }

        public void GetAllDishes(List<DishDefinition> result)
        {
            result.Clear();
            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe == null || recipe.Dish == null) continue;
                if (!result.Contains(recipe.Dish)) result.Add(recipe.Dish);
            }
        }
    }
}
