using System;
using Events;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public class HealthPoints : MonoBehaviour, ITakeDamage
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int initHealth = 100;
        [SerializeField] private bool canTakeDamage = true;

        [Header("events")]
        [SerializeField] private UnityEvent onHit;
        [SerializeField] private VoidEventChannelSO onDeathEvent;
        [SerializeField] private IntEventChannelSO onTakeDamageEvent;
        [SerializeField] private IntEventChannelSO onSumHealthEvent;
        [SerializeField] private IntEventChannelSO onResetPointsEvent;
        [SerializeField] private IntEventChannelSO onInitializeHealthEvent;
        [SerializeField] private IntEventChannelSO onInitializeMaxHealthEvent;
        [SerializeField] private VoidEventChannelSO onDamageAvoidedEvent;

        [Header("Internal events")]
        [SerializeField] private UnityEvent onInternalDeathEvent;
        [SerializeField] private UnityEvent<int> onInternalResetEvent;
        [SerializeField] private UnityEvent<int> onInternalTakeDamageEvent;
        [SerializeField] private UnityEvent<int> onInternalInitializeMaxHealthEvent;
        
        public int MaxHealth
        {
            get { return maxHealth; }
        }

        public VoidEventChannelSO OnDeathEvent
        {
            get { return onDeathEvent; }
        }

        private bool _isInvincible = false;

        public int CurrentHp { get; private set; }

        void Start()
        {
            CurrentHp = initHealth;
            onInitializeHealthEvent?.RaiseEvent(CurrentHp);
            RaiseInitMaxHpEvent();
        }

        private void OnDestroy()
        {
            if (onDeathEvent != null)
                onDeathEvent.onEvent?.RemoveAllListeners();
        }

        public void SetCanTakeDamage(bool value)
        {
            canTakeDamage = value;
        }

        // Is invincible =/= can take damage.
        // isInvincible is used for attacks That can be avoidable.
        // canTakeDamage is used if the entity just cant take damage in any way.
        public void SetIsInvincible(bool value)
        {
            _isInvincible = value;
        }

        public void ResetHitPoints()
        {
            CurrentHp = maxHealth;
            onResetPointsEvent?.RaiseEvent(CurrentHp);
            onInternalResetEvent?.Invoke(CurrentHp);
            RaiseInitMaxHpEvent();
        }

        public bool TryTakeDamage(int damage)
        {
            if (!canTakeDamage)
            {
                onDamageAvoidedEvent?.RaiseEvent();
                return false;
            }

            CurrentHp -= damage;
            
            
            if (IsDead())
            {
                onDeathEvent?.RaiseEvent();
                onInternalDeathEvent?.Invoke();
            }
            else
            {
                onTakeDamageEvent?.RaiseEvent(CurrentHp);
                onHit?.Invoke();
                onInternalTakeDamageEvent?.Invoke(CurrentHp);
            }

            return true;
        }

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }

        public void TryTakeAvoidableDamage(int damage)
        {
            if (_isInvincible) return;
            TryTakeDamage(damage);
        }

        public void RaiseInitMaxHpEvent()
        {
            onInitializeMaxHealthEvent?.RaiseEvent(MaxHealth);
            onInternalInitializeMaxHealthEvent?.Invoke(MaxHealth);
        }

        public void SumHealth(int wonHealth)
        {
            CurrentHp = math.min(maxHealth, wonHealth + CurrentHp);
            onSumHealthEvent?.RaiseEvent(CurrentHp);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void ToggleInvulnerability()
        {
            canTakeDamage = !canTakeDamage;
        }

        public void ToggleInvulnerability(bool value)
        {
            canTakeDamage = value;
        }
#endif
    }
}