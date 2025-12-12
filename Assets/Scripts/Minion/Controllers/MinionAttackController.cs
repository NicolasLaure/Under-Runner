using System.Collections;
using Health;
using Minion.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Minion.Controllers
{
    public class MinionAttackController : MinionController
    {
        [SerializeField] private MinionSO minionConfig;
        [SerializeField] private UnityEvent onAttack;
        
        private bool _isAttacking;
        private Coroutine _attackCoroutine;
        private Vector3 attackDir;


        public Vector3 AttackDir
        {
            get { return attackDir; }
            set { attackDir = value; }
        }

        public void Enter()
        {
            _attackCoroutine = StartCoroutine(StartCharge());

            _isAttacking = true;
        }

        private void OnDisable()
        {
            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);
        }

        private IEnumerator StartCharge()
        {
            float timer = 0;
            float chargeDuration = minionConfig.chargeAttackData.length / minionConfig.attackData.speed;
            float startTime = Time.time;

            Vector3 destination = transform.position + attackDir.normalized * minionConfig.chargeAttackData.length;
            Vector3 startPosition = transform.position;
            destination.y = startPosition.y;

            _healthPoints.SetCanTakeDamage(false);
            _collider.isTrigger = true;
            
            onAttack?.Invoke();

            while (timer < chargeDuration)
            {
                timer = Time.time - startTime;
                transform.position = Vector3.Lerp(startPosition, destination, timer / chargeDuration);
                yield return null;
            }

            _healthPoints.SetCanTakeDamage(true);

            minionAgent.ChangeStateToFallingBack();
        }

        public void Exit()
        {
            _isAttacking = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && _isAttacking)
            {
                DealDamage(other.gameObject);
            }
        }

        private void DealDamage(GameObject target)
        {
            target.gameObject.TryGetComponent(out ITakeDamage playerHealth);
            playerHealth.TryTakeDamage(minionConfig.attackData.damage);
        }
    }
}