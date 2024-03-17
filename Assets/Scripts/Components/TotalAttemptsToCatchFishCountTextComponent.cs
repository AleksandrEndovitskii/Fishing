using Managers;
using TMPro;
using UnityEngine;

namespace Components
{
    public class TotalAttemptsToCatchFishCountTextComponent : MonoBehaviour
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
            FishingManager.Instance.TotalAttemptsToCatchFishCountChanged += FishingManager_TotalAttemptsToCatchFishCountChanged;
            FishingManager_TotalAttemptsToCatchFishCountChanged(FishingManager.Instance.TotalAttemptsToCatchFishCount);
        }
        private void OnDestroy()
        {
            FishingManager.Instance.TotalAttemptsToCatchFishCountChanged -= FishingManager_TotalAttemptsToCatchFishCountChanged;
        }

        private void FishingManager_TotalAttemptsToCatchFishCountChanged(int count)
        {
            _text.text = _initialText + count;
        }
    }
}
