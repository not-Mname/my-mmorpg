using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Common
{
    public class ProgressBar : MonoBehaviour
    {
        public Slider Progress;
        public TextMeshProUGUI ProgressText;
        public delegate void OnValueChangedHander(bool isCrease, float value);
        private OnValueChangedHander _onValueChanged;

        private float _maxValue;
        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = Mathf.Max(value, 1f);
                UpdateUI();
            }
        }

        private float _currentValue;
        public float CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                value = Mathf.Clamp(value, 0, _maxValue);
                this._onValueChanged?.Invoke(value > _currentValue, value);
                _currentValue = value;
                UpdateUI();
            }
        }

        private float _currentPercent;
        private float _target;
        private bool _isRunnig = false;
        public void UpdateUI()
        {
            ProgressText.text = $"{_currentValue:0}/{_maxValue:0}";
            this._target = this._currentValue / this._maxValue;
            if (!_isRunnig)
            {
                this.StartCoroutine(ValueChanger());
            }
        }

        //初始化使用
        public void SetData(float maxValue, float currentValue)
        {
            this._maxValue = maxValue;
            this._currentValue = currentValue;
            this._currentPercent = currentValue / maxValue;
            UpdateUI();
        }
        
        private IEnumerator ValueChanger()
        {
            this._isRunnig = true;
            while (!Mathf.Approximately(_currentPercent, _target))//Approximately比较两个浮点数是否近似相等
            {
                this._currentPercent = Mathf.Lerp(this._currentPercent, this._target, Time.deltaTime * 5f);
                this.Progress.value = this._currentPercent;
                yield return null;
            }
            this._currentPercent = _target;
            Progress.value = this._currentPercent;
            this._isRunnig = false;
        }

        public void AddListener(OnValueChangedHander onValueChanged)
        {
            this._onValueChanged += onValueChanged;
        }

    }
}
