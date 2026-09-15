using UnityEngine;
using UnityEngine.Serialization;

namespace CookingSimulator.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "CookingSimulator/GameConfig", order = 5)]

    public sealed class GameConfig : ScriptableObject
    {
        [Header("Round")]
        [SerializeField, Min(1f)]
        private float roundDuration = 180f;

        [SerializeField, Min(0f)] private float orderRespawnDelay = 5f;
        [SerializeField, Min(1)] private int windowCount = 4;

        [Header("Order Generation")]
        [FormerlySerializedAs("threeIngredientChance")]
        [SerializeField, Range(0f,1f)] private float threeDishChance = 0.5f;

        [Tooltip("Customers order the dishes made by the recipes in this book.")]
        [SerializeField] private RecipeBook recipeBook;
        [Tooltip("0 = seed from the clock at StartRound. Any other value is used " + "verbatim, which makes a round reproducible for debugging.")]
        [SerializeField] private int orderSeed = 0;

        [Header("Player")]
        [SerializeField, Min(0.1f)]
        private float moveSpeed = 6f;

        [SerializeField, Min(1f)] private float turnSpeedDegrees = 900f;
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.9f;

        [Tooltip("How much the station must be in front of the player to be" +
                 "elected. 1 = dead ahead only, -1 = any direction.")]
        [SerializeField, Range(-1f, 1f)]
        private float interactionFacingBias = -0.15f;

        [Tooltip("Downward velocity applied every frame to keep the " +
                 "CharacterController pinned to the floor collider.")]
        [SerializeField]
        private float gravity = -9.81f;

        [Header("Presentation")] [SerializeField, Min(0.1f)]
        private float scorePopupDuration = 2f;

        public float RoundDuration => roundDuration;
        public float OrderRespawnDelay => orderRespawnDelay;
        public int WindowCount => windowCount;
        public float ThreeDishChance => threeDishChance;
        public RecipeBook RecipeBook => recipeBook;
        public int OrderSeed => orderSeed;
        public float MoveSpeed => moveSpeed;
        public float TurnSpeedDegrees => turnSpeedDegrees;
        public float InteractionRadius => interactionRadius;
        public float InteractionFacingBias => interactionFacingBias;
        public float Gravity => gravity;
        public float ScorePopupDuration => scorePopupDuration;
    }
}
