using System.Collections.Generic;
using CookingSimulator.Scripts.Data;
using UnityEngine;

namespace CookingSimulator.Core
{
    public sealed class Order
    {
        private const int MaxDishes = 3;
        private readonly List<DishDefinition> required = new List<DishDefinition>(MaxDishes);
        private readonly List<bool> filled = new List<bool>(MaxDishes);

        public float ActivatedAt { get; private set; }
        public int BaseScore { get; private set; }

        public int Count => required.Count;
        public IReadOnlyList<DishDefinition> Required => required;
        public DishDefinition RequiredAt(int index) => required[index];
        public bool IsFilledAt(int index) => filled[index];

        public bool IsComplete
        {
            get
            {
                for (int i = 0; i<filled.Count; i++)
                    if (!filled[i])
                        return false;
                return filled.Count > 0;
            }
        }

        public void Initialize(List<DishDefinition> dishes, float clockNow)
        {
            required.Clear();
            filled.Clear();
            BaseScore = 0;

            for (int i = 0; i < dishes.Count; i++)
            {
                required.Add(dishes[i]);
                filled.Add(false);
                BaseScore += dishes[i].ScoreValue;
            }
            ActivatedAt = clockNow;
        }

        public bool TryFill(in KitchenItem item)
        {
            DishDefinition dish = item.Dish;
            if (dish == null) return false;

            for (int i = 0; i < required.Count; i++)
            {
                if (filled[i]) continue;
                if (required[i] != dish) continue;
                filled[i] = true;
                return true;
            }
            return false;
        }

        public bool Wants(in KitchenItem item)
        {
            DishDefinition dish = item.Dish;
            if (dish == null) return false;

            for (int i = 0; i < required.Count; i++)
                if (!filled[i] && required[i] == dish) return true;
            return false;
        }
        public float ElapsedAt(float clockNow) => clockNow - ActivatedAt;
        public int ScoreAt(float clockNow)=> BaseScore - Mathf.FloorToInt(clockNow-ActivatedAt);
    }
}
