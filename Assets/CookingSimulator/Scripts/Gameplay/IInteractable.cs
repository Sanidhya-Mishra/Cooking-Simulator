namespace CookingSimulator.Scripts.Gameplay
{
    public interface IInteractable
    {
        bool CanInteract(PlayerHands hands);
        void Interact(PlayerHands hands);
        UnityEngine.Transform HighlightAnchor { get; }
    }
}