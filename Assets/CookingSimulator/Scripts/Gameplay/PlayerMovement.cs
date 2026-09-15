using UnityEngine;
using CookingSimulator.Scripts.Data;

namespace CookingSimulator.Scripts.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private PlayerInputReader input;
        
        private CharacterController controller;
        private bool canMove;
        private void Awake()=> controller = GetComponent<CharacterController>();

        public void SetMovementEnabled(bool value)
        {
            canMove = value;
            input.SetMovementSuppressed(!value);
        }

        private void Update()
        {
            if (!canMove) return;
            Vector2 raw = input.Move;
            Vector3 direction = new Vector3(raw.x, 0f, raw.y);
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }

            Vector3 velocity = direction * config.MoveSpeed;
            velocity.y = config.Gravity;
            controller.Move(velocity* Time.deltaTime);
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion target =  Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target,config.TurnSpeedDegrees*Time.deltaTime);
            }
        }
    }
}