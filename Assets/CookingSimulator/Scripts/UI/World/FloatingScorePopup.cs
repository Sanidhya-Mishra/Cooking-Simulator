using UnityEngine;
using TMPro;
using CookingSimulator.Scripts.Data;

namespace CookingSimulator.Scripts.UI.World
{
    public sealed class FloatingScorePopup : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text label;
        [SerializeField] private float riseDistance = 0.8f;
        [SerializeField] private Color positiveColor = new Color(0.4f, 0.9f, 0.45f);
        [SerializeField] private Color negativeColor = new Color(0.95f, 0.35f, 0.35f);

        private Vector3 basePosition;
        private float timer = -1f;

        private float Duration => config != null
            ? config.ScorePopupDuration
            : 2f; 
        private void Awake()
        {
            if (root == gameObject)
                Debug.LogError($"[{name}] root must be a child object.", this);
            basePosition = root.transform.localPosition;
            root.SetActive(false);
        }

        public void Play(int score)
        {
            label.text = score >= 0 ? $"+{score}" : score.ToString();
            label.color = score >= 0 ? positiveColor : negativeColor;
            timer = 0f;
            root.transform.localPosition = basePosition;
            root.SetActive(true);
        }

        private void Update()
        {
            if (timer < 0f) return;
            timer += Time.unscaledDeltaTime;
            float t = timer / Duration;
            if (t >= 1f)
            {
                timer = -1f;
                root.SetActive(false);
                return;
            }
            root.transform.localPosition = basePosition + Vector3.up * (riseDistance * t);
            Color c = label.color;
            c.a = 1f - t;
            label.color = c;
        }
    }
}