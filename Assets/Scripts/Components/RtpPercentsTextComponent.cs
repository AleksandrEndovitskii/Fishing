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
            FishingManager.Instance.RtpPercentsChanged += FishingManager_RtpPercentsChanged;
            FishingManager_RtpPercentsChanged(FishingManager.Instance.RtpPercents);
        }
        private void OnDestroy()
        {
            FishingManager.Instance.RtpPercentsChanged -= FishingManager_RtpPercentsChanged;
        }

        private void FishingManager_RtpPercentsChanged(int percents)
        {
            _text.text = _initialText + percents;
        }
    }
}
