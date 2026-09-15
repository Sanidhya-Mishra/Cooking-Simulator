using UnityEngine;
using CookingSimulator.Scripts.Data;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class Ingredientview : MonoBehaviour
    {
        [Tooltip("Must be a CHILD object, never this one - deactivating the" +
                 "object this script lives on would stop it responding.")]
        [SerializeField]
        private GameObject visualRoot;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        private void Awake()
        {
            if (visualRoot == gameObject)
            {
                Debug.LogError($"[{name} visualRoot must be child object)", this);
            }

            Hide();
        }

        public void Show(in KitchenItem item)
        {
            if (!item.IsValid)
            {
                Hide();
                return;
            }

            if (item.IsRawIngredient)
            {
                meshFilter.sharedMesh = item.Ingredient.Mesh;
                meshRenderer.sharedMaterial = item.Ingredient.Material;
            }
            else
            {
                meshFilter.sharedMesh = item.Recipe.GetMesh(item.StepsDone);
                meshRenderer.sharedMaterial = item.Recipe.GetMaterial(item.StepsDone);

                DishDefinition dish = item.Dish;
                if (dish != null && dish.Mesh != null) meshFilter.sharedMesh = dish.Mesh;
                if (dish != null && dish.Material != null) meshRenderer.sharedMaterial = dish.Material;
            }
            visualRoot.SetActive(true);
        }
        public void Hide() => visualRoot.SetActive(false);
    }
}
