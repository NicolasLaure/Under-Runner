using System;
using System.Collections;
using Events.ScriptableObjects;
using UnityEngine;

namespace VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class MotorFlamesHandler : MonoBehaviour
    {
        [SerializeField] private float timeToChange;
        [SerializeField] private MotorFlamesChannelSO onNewMotorFlamesEvent;
        [SerializeField] private GameObject smallFlames;
        
        private ParticleSystem _motorParticleSystem;

        private Coroutine _changeMotorParticlesCoroutine;
        void OnEnable()
        {
            _motorParticleSystem ??= GetComponent<ParticleSystem>();
            onNewMotorFlamesEvent?.onTypedEvent.AddListener(HandleMotorFlamesVfx);
        }

        private void OnDisable()
        {
            onNewMotorFlamesEvent?.onTypedEvent.RemoveListener(HandleMotorFlamesVfx);
        }

        private void HandleMotorFlamesVfx(MotorFlamesData motorData)
        {
            if(_changeMotorParticlesCoroutine != null) StopCoroutine(_changeMotorParticlesCoroutine);
            _changeMotorParticlesCoroutine = StartCoroutine(ChangeMotorParticles(motorData));
        }

        private IEnumerator ChangeMotorParticles(MotorFlamesData motorData)
        {
            smallFlames.gameObject.SetActive(motorData.areSmallFlamesActive);
            float timer = 0;
            float startTime = Time.time;

            ParticleSystem.MainModule mainModule = _motorParticleSystem.main;
            Vector2 startLifetimePreviousValue = new Vector2(mainModule.startSpeed.constantMin, mainModule.startSpeed.constantMax);
            Vector2 startSizePreviousValue = new Vector2(mainModule.startSize.constantMin, mainModule.startSize.constantMax);
            
            while (timer < timeToChange)
            {
                timer = Time.time - startTime;

                Vector2 lifetimeValue = Vector2.Lerp(startLifetimePreviousValue, motorData.speedMinMax,
                    timer / timeToChange);
                Vector2 startSizeValue =
                    Vector2.Lerp(startSizePreviousValue, motorData.startSizeMinMax, timer / timeToChange);

                ParticleSystem.MinMaxCurve lifetimeCurveMin = mainModule.startSpeed;
                lifetimeCurveMin.constantMin = lifetimeValue.x;
                mainModule.startSpeed = lifetimeCurveMin;
                
                ParticleSystem.MinMaxCurve lifetimeCurveMax = mainModule.startSpeed;
                lifetimeCurveMax.constantMax = lifetimeValue.y;
                mainModule.startSpeed = lifetimeCurveMax;
                
                ParticleSystem.MinMaxCurve startSizeCurveMin = mainModule.startSize;
                startSizeCurveMin.constantMin = startSizeValue.x;
                mainModule.startSize = startSizeCurveMin;
                
                ParticleSystem.MinMaxCurve startSizeCurveMax = mainModule.startSize;
                startSizeCurveMax.constantMax = startSizeValue.y;
                mainModule.startSize = startSizeCurveMax;
                yield return null;
            }
        }
    }
}
