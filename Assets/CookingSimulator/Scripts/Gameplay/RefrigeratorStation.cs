using UnityEngine;
using CookingSimulator.Scripts.Data;
using CookingSimulator.Scripts.Core;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class RefrigeratorStation: StationBase
    {
        [SerializeField] private IngredientDefinition dispenses;
        [SerializeField] private Ingredientview displayview;

        private void Start()
        {
            if (displayview != null)
                displayview.Show(KitchenItem.FromIngredient(dispenses));
        }

        public override bool CanInteract(PlayerHands hands)=>
            hands.IsEmpty && dispenses != null;

        public override void Interact(PlayerHands hands)
        {
            hands.TryGive(KitchenItem.FromIngredient(dispenses));
        }
    }
}