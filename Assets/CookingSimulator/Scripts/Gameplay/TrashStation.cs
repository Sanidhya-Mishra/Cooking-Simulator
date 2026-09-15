using UnityEngine;

namespace CookingSimulator.Scripts.Gameplay
{
    public class TrashStation : StationBase
    {
        public override bool CanInteract(PlayerHands hands)=>!hands.IsEmpty;
        public override void Interact(PlayerHands hands) => hands.Clear();

    }
}