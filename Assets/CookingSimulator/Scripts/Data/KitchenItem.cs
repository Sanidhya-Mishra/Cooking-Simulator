namespace CookingSimulator.Scripts.Data
{
    // Whatever is in the player's hands or on a station: a raw ingredient or food partway through a recipe.
    public readonly struct KitchenItem
    {
        public readonly IngredientDefinition Ingredient;
        public readonly RecipeDefinition Recipe;
        public readonly int StepsDone;

        public KitchenItem(IngredientDefinition ingredient, RecipeDefinition recipe, int stepsDone)
        {
            Ingredient = ingredient;
            Recipe = recipe;
            StepsDone = stepsDone;
        }

        public static readonly KitchenItem None = default;

        public static KitchenItem FromIngredient(IngredientDefinition ingredient)
        {
            return new KitchenItem(ingredient, null, 0);
        }

        public bool IsValid => Ingredient != null || Recipe != null;
        public bool IsRawIngredient => Recipe == null && Ingredient != null;
        public bool IsDish => Recipe != null && StepsDone >= Recipe.StepCount;
        public DishDefinition Dish => IsDish ? Recipe.Dish : null;
    }
}
