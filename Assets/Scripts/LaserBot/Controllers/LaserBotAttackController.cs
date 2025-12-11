using System.Collections;
using System.Collections.Generic;
using Health;
using UnityEngine;
using UnityEngine.Events;

namespace LaserBot.Controllers
{
    public class LaserBotAttackController : LaserBotController
    {
        [SerializeField] private LaserBotConfigSO minionConfig;
        [SerializeField] private UnityEvent onAttack;
        [SerializeField] private GameObject laser;
        [SerializeField] private Material laserChargeMat;
        [SerializeField] private Material laserMat;
        [SerializeField] private LaserCollider laserCollider;

        private bool _isAttacking;
        private Coroutine _attackCoroutine;

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
            float startTime = Time.time;

            onAttack?.Invoke();

            laser.SetActive(true);
            laser.GetComponent<Renderer>().material = laserChargeMat;

            while (timer < minionConfig.chargeDuration)
            {
                timer = Time.time - startTime;
                yield return null;
            }

            laserCollider.SetCollision(true);
            laser.GetComponent<Renderer>().material = laserMat;

            yield return new WaitForSeconds(minionConfig.attackDuration);
            laserCollider.SetCollision(false);
            laser.SetActive(false);

            botAgent.ChangeStateToExit();
        }
    }
}