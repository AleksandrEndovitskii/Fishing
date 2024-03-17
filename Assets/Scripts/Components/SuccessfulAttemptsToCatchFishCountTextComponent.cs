using Managers;
using TMPro;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SuccessfulAttemptsToCatchFishCountTextComponent : MonoBehaviour
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
            FishingManager.Instance.SuccessfulAttemptsToCatchFishCountChanged += FishingManager_SuccessfulAttemptsToCatchFishCountChanged;
            FishingManager_SuccessfulAttemptsToCatchFishCountChanged(FishingManager.Instance.SuccessfulAttemptsToCatchFishCount);
        }
        private void OnDestroy()
        {
            FishingManager.Instance.SuccessfulAttemptsToCatchFishCountChanged -= FishingManager_SuccessfulAttemptsToCatchFishCountChanged;
        }

        private void FishingManager_SuccessfulAttemptsToCatchFishCountChanged(int count)
        {
            _text.text = _initialText + count;
        }
    }
}
