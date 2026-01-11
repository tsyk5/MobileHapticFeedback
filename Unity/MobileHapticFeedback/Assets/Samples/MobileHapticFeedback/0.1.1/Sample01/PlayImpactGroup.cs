using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace tsyk5.MobileHapticFeedback.Sample01
{
    public class PlayImpactGroup : MonoBehaviour
    {
        [Header("CoreHaptics - impact")]
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Slider _intensitySlider;
        [SerializeField] private Slider _sharpnessSlider;
        [SerializeField] private Slider _durationSlider;
        [SerializeField] private Button _playImpactButton;
        [SerializeField] private Button _stopImpactButton;
        
        [Header("CoreHaptics - patterns")]
        [SerializeField] private Button _sosButton;
        [SerializeField] private Button _stepUpButton;
        [SerializeField] private Button _heartBeatButton;
        [SerializeField] private Button _stopPatternButton;
        
        [Header("UIKit")]
        [SerializeField] private Button _lightButton;
        [SerializeField] private Button _mediumButton;
        [SerializeField] private Button _heavyButton;
        [SerializeField] private Button _rigidButton;
        [SerializeField] private Button _softButton;
        [SerializeField] private Button _successButton;
        [SerializeField] private Button _warningButton;
        [SerializeField] private Button _errorButton;
        [SerializeField] private Button _selectButton;

        private IVibrationExecutor _vibrationExecutor;
        private void Start()
        {
            if (!MobileHapticFeedback.IsSupported)
            {
                Debug.Log("Haptics not supported"); 
                return;
            }
            
            MobileHapticFeedback.Prepare();
            _vibrationExecutor = new VibrationExecutor();

            ConfigureSlider(_intensitySlider, 0f, 1f, 0.1f, 0.5f);
            ConfigureSlider(_sharpnessSlider, 0f, 1f, 0.1f, 0.5f);
            ConfigureSlider(_durationSlider, 0.05f, 1.0f, 0.05f, 0.1f);

            _intensitySlider.onValueChanged.AddListener(_ => UpdateText());
            _sharpnessSlider.onValueChanged.AddListener(_ => UpdateText());
            _durationSlider.onValueChanged.AddListener(_ => UpdateText());
            
#if UNITY_ANDROID
            // NOTE: sharpness is not supported on Android
            _sharpnessSlider.interactable = false;
#endif

            _playImpactButton.onClick.AddListener(OnPlayButtonClicked);
            _lightButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Light));
            _mediumButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Medium));
            _heavyButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Heavy));
            _rigidButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Rigid));
            _softButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Soft));
            _successButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Success));
            _warningButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Warning));
            _errorButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Error));
            _selectButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Selection));
            _sosButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Sos));
            _stepUpButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.StepUp));
            _heartBeatButton.onClick.AddListener(() => _vibrationExecutor.Vibrate(VibrationType.Heartbeat));
            _stopImpactButton.onClick.AddListener(MobileHapticFeedback.Stop);
            _stopPatternButton.onClick.AddListener(MobileHapticFeedback.Stop);
        }

        private void ConfigureSlider(Slider slider, float min, float max, float step, float defaultValue)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = false;
            slider.value = defaultValue;

            slider.onValueChanged.AddListener(value =>
            {
                float rounded = Mathf.Round(value / step) * step;
                if (Mathf.Abs(rounded - value) > Mathf.Epsilon)
                    slider.SetValueWithoutNotify(rounded);
            });
        }

        private void UpdateText()
        {
            _text.text = $"Intensity: {_intensitySlider.value:F1}  " +
                         $"Sharpness: {_sharpnessSlider.value:F1}  " +
                         $"Duration: {_durationSlider.value:F2}s";
        }

        private void OnPlayButtonClicked()
        {
            _vibrationExecutor.Vibrate(
                _intensitySlider.value,
                _sharpnessSlider.value,
                _durationSlider.value
            );
        }
    }
}
