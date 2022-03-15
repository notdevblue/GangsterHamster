using Tween;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Effects.Global
{
    public class GlobalChromaticAberration : MonoSingleton<GlobalChromaticAberration>
    {
        private Volume _volume;
        private ChromaticAberration _chromaticAberration;

        // 코루틴 저장 용
        private Coroutine _decreaseCoroutine = null;
        private Coroutine _increaseCoroutine = null;

        private void Awake() {

            _volume = GetComponent<Volume>();

#if UNITY_EDITOR
            NULL.Check(_volume, () => {
                this.enabled = false;
            });
#endif
            if(!_volume.profile.TryGet<ChromaticAberration>(out var chromaticAberration)) {
                _chromaticAberration = _volume.profile.Add<ChromaticAberration>(false);
            } else { // 이렇게 안하면 안 들어가져서
                _chromaticAberration = chromaticAberration;
            }
        }

        public void Increase(float startValue, float endValue, float duration, Action callback = null) {

            if(_increaseCoroutine != null) { // 이미 재생중인 경우
                ValueTween.Stop(this, _increaseCoroutine);
            }

            float step = (endValue - startValue) / duration; // 계산

            _chromaticAberration.intensity.value = startValue;

            callback += () => { // 효과 종료 처리
                _increaseCoroutine = null;
            };

            _increaseCoroutine = ValueTween.To(this, () => {
                _chromaticAberration.intensity.value += step * Time.deltaTime;
            }, () => {
                return _chromaticAberration.intensity.value >= endValue;
            }, callback);
        }

        public void Decrease(float startValue, float endValue, float duration, Action callback = null) {

            if (_decreaseCoroutine != null) { // 이미 재생중인 경우
                ValueTween.Stop(this, _decreaseCoroutine);
            }

            float step = (startValue - endValue) / duration; // 계산

            _chromaticAberration.intensity.value = startValue;

            callback += () => { // 효과 종료 처리
                _decreaseCoroutine = null;
            };

            _decreaseCoroutine = ValueTween.To(this, () => {
                _chromaticAberration.intensity.value -= step * Time.deltaTime;
            }, () => {
                return _chromaticAberration.intensity.value <= endValue;
            }, callback);
        }
    }
}