using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CookingSimulator.Scripts.UI.World
{
    public sealed class StationProgressView : MonoBehaviour
    {
        [Tooltip("Must be a child object.")] 
        [SerializeField] private GameObject root;

        [SerializeField] private Image fill;
        [SerializeField] private TMP_Text secondsLabel;
        [SerializeField] private Color inProgressColor = new Color(1f, 0.75f, 0.2f);
        [SerializeField] private Color readyColor = new Color(0.35f, 0.85f, 0.4f);

        private void Awake()
        {
            if (root == gameObject)
            {
                Debug.LogError($"[{name}] root must be a child object.", this);
            }
            Hide();
        }

        public void ShowProgress(float normalised, float secondsRemaining)
        {
            root.SetActive(true);
            fill.color = inProgressColor;
            fill.fillAmount = Mathf.Clamp01(normalised);
            secondsLabel.text = secondsRemaining.ToString("0.0");
        }

        public void ShowReady()
        {
            root.SetActive(true);
            fill.color = readyColor;
            fill.fillAmount = 1f;
            secondsLabel.text = "Ready";
            
        }
        public void ShowMessage(string message)
        {
            root.SetActive(true);
            fill.color = readyColor;
            fill.fillAmount = 1f;
            secondsLabel.text = message;
        }

        public void Hide() => root.SetActive(false);
    }
}