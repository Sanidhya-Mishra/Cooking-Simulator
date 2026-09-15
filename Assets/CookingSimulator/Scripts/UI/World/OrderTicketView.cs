using System;
using CookingSimulator.Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CookingSimulator.Scripts.Core;

namespace CookingSimulator.Scripts.UI.World
{
    public sealed class OrderTicketView : MonoBehaviour
    {
        [System.Serializable]
        private sealed class Chip
        {
            public GameObject root;
            public Image icon;
            public GameObject filledOverlap;
            public TMP_Text nameLabel;
        }

        [Tooltip("Must be a CHILD object, never this one.")] 
        [SerializeField] private GameObject root;
        [SerializeField] private Chip[] chips = new Chip[3];
        [SerializeField] private TMP_Text timerLabel;
        [SerializeField] private TMP_Text valueLabel;
        [SerializeField] private Image urgencyBar;
        [SerializeField] private Color safeColor = new Color(0.53f, 0.85f, 0.4f);
        [SerializeField] private Color urgentColor = new Color(0.9f,0.3f,0.3f);

        private Order order;

        private void Awake()
        {
            if (root == gameObject)
            {
                Debug.LogError($"[{name}] root must be a child object.", this); 
            }

            Hide();
        }

        public void Bind(Order value)
        {
            order = value;
            root.SetActive(true);
            if (order.Count > chips.Length)
            {
             Debug.LogError($"[{name}] order needs {order.Count} chips but only {chips.Length} " +
                             "are authored on this prefab.", this);    
            }

            for (int i = 0; i < chips.Length; i++)
            {
                bool used = i< order.Count;
                chips[i].root.SetActive(used);
                if (!used) continue;
                chips[i].icon.color = order.RequiredAt(i).TickColor;
                chips[i].filledOverlap.SetActive(false);
                if (chips[i].nameLabel != null)
                    chips[i].nameLabel.text = order.RequiredAt(i).DisplayName;
            }
        }

        public void RefreshFillState()
        {
            if (order == null) return;
            int shown = Mathf.Min(order.Count, chips.Length);
            for(int i =0; i< shown; i++)
                chips[i].filledOverlap.SetActive(order.IsFilledAt(i));
        }

        public void RefreshTimer(float elapsedSeconds, int projectedScore)
        {
            timerLabel.text = Mathf.FloorToInt(elapsedSeconds).ToString();
            valueLabel.text = projectedScore.ToString();

            float fraction = order == null || order.BaseScore <= 0
                ? 0f
                : Mathf.Clamp01(projectedScore / (float)order.BaseScore);
            urgencyBar.fillAmount = fraction;
            urgencyBar.color = Color.Lerp(urgentColor, safeColor, fraction);
        }

        public void Hide()
        {
            order = null;
            root.SetActive(false);
        }

    }
    
}