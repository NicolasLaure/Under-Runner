using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using Input;
using Managers;
using Player.Weapon;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class AttackInputAction
    {
        public float inputTime;
    }
    
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private PauseSO pauseData;
        [SerializeField] private InputHandlerSO inputHandler;

        [Header("Animation Handler")]
        [SerializeField] private PlayerAnimationHandler animationHandler;

        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onTransitionToMovementEnded;

        [Header("Initial Sequence Events")]
        [SerializeField] private VoidEventChannelSO onCinematicStarted;
        [SerializeField] private VoidEventChannelSO onCinematicFinished;

        [Header("Attack Configuration")]
        [SerializeField] private MeleeWeapon meleeWeapon;
        [SerializeField] private float attackBufferSeconds;
        [SerializeField] private AnimationCurve attackCurve;
        [SerializeField] private LayerMask layers;
        [SerializeField] private float attackDuration;
        [SerializeField] private float attackExitPercentage;
        [SerializeField] private float doubleAttackExitPercentage = 0.81f;
        
        private bool _canAttack = true;
        private bool _hasTransitionToMovementEnded;
        private List<AttackInputAction> _attackInputActions;
        private Coroutine _attackCoroutine = null;

        private void OnEnable()
        {
            _attackInputActions = new List<AttackInputAction>();
            _hasTransitionToMovementEnded = true;
            _canAttack = true;
            inputHandler.onPlayerAttack.AddListener(HandleAttack);

            onCinematicStarted.onEvent.AddListener(DisableAttack);
            onCinematicFinished.onEvent.AddListener(EnableAttack);
            onTransitionToMovementEnded.onEvent.AddListener(HandleTransitionToMovementEnded);
            onPlayerDeath.onEvent.AddListener(EnableAttack);
        }

        private void OnDisable()
        {
            inputHandler.onPlayerAttack.RemoveListener(HandleAttack);

            onTransitionToMovementEnded.onEvent.RemoveListener(HandleTransitionToMovementEnded);
            onCinematicStarted.onEvent.RemoveListener(DisableAttack);
            onCinematicFinished.onEvent.RemoveListener(EnableAttack);
            onPlayerDeath.onEvent.RemoveListener(EnableAttack);
        }
        private void HandleTransitionToMovementEnded()
        {
            _hasTransitionToMovementEnded = true;
        }

        private void Update()
        {
            if (_canAttack)
            {
                CheckAndDoAttack();
            }
        }


        public void HandleAttack()
        {
            _attackInputActions.Add(new AttackInputAction()
            {
                inputTime = Time.time
            });
        }

        private void DoAttack()
        {
            if (!_canAttack || pauseData.isPaused)
                return;

            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            _canAttack = false;
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }
        
        private void CheckAndDoAttack()
        {
            bool attackDone = false;
            foreach (var attackInputAction in _attackInputActions.ToList())
            {
                _attackInputActions.Remove(attackInputAction);
                if (attackInputAction.inputTime + attackBufferSeconds >= Time.time && !attackDone)
                {
                    DoAttack();
                    attackDone = true;
                }
            }
        }

        private void ClearAttackBuffer()
        {
            foreach (var attackInputAction in _attackInputActions.ToList())
            {
                _attackInputActions.Remove(attackInputAction);
            }
        }

        private bool HasBufferedAttack()
        {
            return _attackInputActions.Any(attack => attack.inputTime + attackBufferSeconds >= Time.time);
        }
        
        private IEnumerator AttackCoroutine()
        {
            meleeWeapon.enabled = true;
            float startTime = Time.time;
            animationHandler.StartAttackAnimation();
            _hasTransitionToMovementEnded = false;

            bool isExiting = false;
            bool isEndingAttack = false;
            float timer = 0;
            while (!isExiting)
            {
                timer = Time.time - startTime;
                float percentage = timer / attackDuration;
                animationHandler.SetAttackProgress(attackCurve.Evaluate(percentage));
                
                if (meleeWeapon.enabled && timer / attackDuration > 0.5f)
                    meleeWeapon.enabled = false;
                
                if(percentage > attackExitPercentage && !HasBufferedAttack())
                {
                    animationHandler.EndAttackAnimation();
                    isEndingAttack = true;
                }

                if (!isEndingAttack && percentage > doubleAttackExitPercentage && HasBufferedAttack())
                {
                    ClearAttackBuffer();
                    animationHandler.RestartAttackAnimation();
                    meleeWeapon.enabled = true;
                    startTime = Time.time;
                }
                
                if (percentage > 1)
                {
                    isExiting = true;
                }
                
                yield return null;
            }
            
            meleeWeapon.enabled = false;
            yield return new WaitUntil(() => _hasTransitionToMovementEnded);
            _canAttack = true;
        }

        private void EnableAttack()
        {
            _canAttack = true;
        }

        private void DisableAttack()
        {
            _canAttack = false;
        }
    }
}