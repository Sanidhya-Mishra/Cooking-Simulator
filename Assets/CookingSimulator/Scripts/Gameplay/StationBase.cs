using UnityEngine;

namespace CookingSimulator.Scripts.Gameplay
{
    public abstract class StationBase : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform highlightAnchor;
        
        public Transform HighlightAnchor => highlightAnchor != null
        ? highlightAnchor
        :transform;
        
        public abstract bool CanInteract(PlayerHands hands);
        public abstract void Interact(PlayerHands hands);
        
    }
}