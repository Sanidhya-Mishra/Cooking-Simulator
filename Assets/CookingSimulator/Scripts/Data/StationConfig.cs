using UnityEngine;

namespace CookingSimulator.Scripts.Data
{
    [CreateAssetMenu(fileName = "STC_NewStation", menuName = "CookingSimulator/Station Config", order = 4)]
    public sealed class StationConfig : ScriptableObject
    {
        [SerializeField] private string displayName = "Station";

        [Tooltip("Recipe steps with this action are done at this station. Step times come from the recipe.")]
        [SerializeField] private PrepType performs = PrepType.Chop;

        [SerializeField, Min(1)] private int slotCount = 1;

        public string DisplayName => displayName;
        public PrepType Performs => performs;
        public int SlotCount => slotCount;
    }
}
