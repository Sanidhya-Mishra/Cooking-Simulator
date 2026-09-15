using System;
using UnityEngine;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class InteractionPrompt  : MonoBehaviour
    {
        [Tooltip("Must be a CHILD Object.")] [SerializeField]
        private GameObject visual;

        [SerializeField] private Vector3 offset = new Vector3(0, 1.4f, 0);
        private Transform target;

        private void Awake()
        {
            if (visual == gameObject)
            {
                Debug.LogError($"[{name} visual must be child object.", this);
            }
            visual.SetActive(false);
        }

        public void SetTarget(Transform value)
        {
            target = value;
            visual.SetActive(target != null);
        }

        private void LateUpdate()
        {
            if(target == null) return;
            visual.transform.position = target.position + offset;
        }
    }
}