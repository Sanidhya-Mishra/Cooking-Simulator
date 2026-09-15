using UnityEngine;
using UnityEngine.Serialization;

namespace CookingSimulator.Scripts.Data
{
    [CreateAssetMenu(fileName = "ING_NewIngredient", menuName = "CookingSimulator/Ingredient Definition", order = 0)]
    public sealed class IngredientDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Ingredient";
        [SerializeField] private Mesh mesh;

        [FormerlySerializedAs("rawMaterial")]
        [SerializeField] private Material material;

        [Tooltip("Colour used for this ingredient in the UI.")]
        [SerializeField] private Color tickColor = Color.white;

        public string DisplayName => displayName;
        public Mesh Mesh => mesh;
        public Material Material => material;
        public Color TickColor => tickColor;
    }
}
