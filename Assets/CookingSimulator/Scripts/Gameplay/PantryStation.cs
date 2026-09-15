using UnityEngine;
using CookingSimulator.Scripts.Data;
using CookingSimulator.Scripts.UI.Screens;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class PantryStation : StationBase
    {
        [Tooltip("The ingredients this station offers, in display order.")]
        [SerializeField] private IngredientDefinition[] offerings;

        [Tooltip("Purely decorative - one view per entry in Offerings, shown at Start and never changed again.")]
        [SerializeField] private Ingredientview[] displayViews;

        [SerializeField] private IngredientPickerView picker;

        private void Start()
        {
            if (offerings == null || displayViews == null) return;

            int count = Mathf.Min(offerings.Length, displayViews.Length);
            for (int i = 0; i < count; i++)
            {
                if (displayViews[i] == null || offerings[i] == null) continue;
                displayViews[i].Show(KitchenItem.FromIngredient(offerings[i]));
            }
        }

        public override bool CanInteract(PlayerHands hands) =>
            hands.IsEmpty && offerings != null && offerings.Length > 0 && picker != null && !picker.IsOpen;

        public override void Interact(PlayerHands hands) => picker.Open(offerings, hands);
    }
}
