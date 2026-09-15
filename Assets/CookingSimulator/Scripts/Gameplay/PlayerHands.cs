using System;
using UnityEngine;
using CookingSimulator.Scripts.Data;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class PlayerHands : MonoBehaviour
    {
        [SerializeField] private Ingredientview view;
        public KitchenItem Held { get; private set; }
        public bool IsEmpty => !Held.IsValid;
        public event Action<KitchenItem> HeldChanged;

        public bool TryGive(in KitchenItem item)
        {
            if(!IsEmpty || !item.IsValid) return false;
            Held = item;
            Refresh();
            return true;
        }

        public KitchenItem Take()
        {
            KitchenItem item = Held;
            Held = KitchenItem.None;
            Refresh();
            return item;
        }

        public void Clear() => Take();

        private void Refresh()
        {
            if (Held.IsValid) view.Show(Held);
            else view.Hide();
            HeldChanged?.Invoke(Held);
        }

    }
}
