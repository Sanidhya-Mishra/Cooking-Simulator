using UnityEngine;

namespace CookingSimulator.Scripts.Data
{
    [CreateAssetMenu(fileName = "DISH_NewDish", menuName = "CookingSimulator/Dish Definition", order = 1)]
    public sealed class DishDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Dish";
        [SerializeField, Min(0)] private int scoreValue = 20;

        [Tooltip("Colour of this dish on the order tickets.")]
        [SerializeField] private Color tickColor = Color.white;

        [Header("Optional - leave empty to use the look from the last recipe step")]
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        public string DisplayName => displayName;
        public int ScoreValue => scoreValue;
        public Color TickColor => tickColor;
        public Mesh Mesh => mesh;
        public Material Material => material;
    }
}
