using UnityEngine;
using CookingSimulator.Scripts.Data;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        private const int MaxCandidates = 16;
        [SerializeField] private GameConfig config;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerHands hands;
        [SerializeField] private LayerMask interactableMask;
        [SerializeField] private Transform queryOrigin;
        [SerializeField] private InteractionPrompt prompt;
        
        private readonly Collider[] buffer = new Collider[MaxCandidates];
        private IInteractable current;
        private bool active;

        private void OnEnable()
        {
            input.Interacted += OnInteract;
            input.Dropped += OnDrop;
        }

        private void OnDisable()
        {
            input.Interacted -= OnInteract;
            input.Dropped -= OnDrop;
        }

        public void SetInteractionEnabled(bool value)
        {
            active = value;
            if (!value) SetCurrent(null);
        }

        private void Update()
        {
            if (!active) return;
            SetCurrent(FindBest());
        }

        private IInteractable FindBest()
        {
            Vector3 origin = queryOrigin.position;
            int count = Physics.OverlapSphereNonAlloc(origin, config.InteractionRadius, buffer, interactableMask,
                QueryTriggerInteraction.Collide);
            IInteractable best = null;
            float bestScore = float.NegativeInfinity;
            Vector3 forward = transform.forward;
            for (int i = 0;
                 i < count;
                 i++)
            {
                IInteractable candidate = buffer[i].GetComponentInParent<IInteractable>();
                if (candidate == null) continue;
                if (!candidate.CanInteract(hands)) continue;
                Vector3 to = buffer[i].transform.position - origin;
                to.y = 0f;
                float distance = to.magnitude;
                if(distance<0.0001f) return candidate;
                float facing = Vector3.Dot(forward, to / distance);
                if (facing < config.InteractionFacingBias) continue;

                float score = facing - distance * 0.25f;
                if (score <= bestScore) continue;
                
                bestScore = score;
                best = candidate;
            }
            return best;
        }

        private void SetCurrent(IInteractable next)
        {
            if(ReferenceEquals(current,next)) return;
            current = next;
            prompt.SetTarget(current?.HighlightAnchor);
        }

        private void OnInteract()
        {
            if(!active||current == null) return;
            if(!current.CanInteract(hands))return;
            current.Interact(hands);
        }

        /// <summary>
        /// Drops whatever is currently held, regardless of what station (if any)
        /// the player is near. Ingredients are data, not spawned objects, so
        /// "destroying" the held item is just clearing the hand slot - the same
        /// thing TrashStation does, just without needing to walk to the trash.
        /// </summary>
        private void OnDrop()
        {
            if (!active) return;
            hands.Clear();
        }

    }
}