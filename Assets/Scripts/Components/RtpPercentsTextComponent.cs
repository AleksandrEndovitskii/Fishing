using Managers;
using TMPro;
using UnityEngine;

namespace Components
{
    public class RtpPercentsTextComponent : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private string _initialText;

        private void Awake()
        {
            _text = this.gameObject.GetComponent<TextMeshProUGUI>();
            _initialText = _text.text;
        }
        private void Start()
        {
            FishingManager.Instance.RtpPercentsChanged += (percents) =>
            {
                _text.text = _initialText + percents;
            };
        }
    }
}
