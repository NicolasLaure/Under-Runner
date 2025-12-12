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
            float delta = 0;
            float prevTime = Time.time;

            onAttack?.Invoke();

            laser.SetActive(true);
            laser.GetComponent<Renderer>().material = laserChargeMat;

            Vector3 dir = Vector3.zero;
            while (timer < minionConfig.chargeDuration)
            {
                timer = Time.time - startTime;
                delta = Time.time - prevTime;

                if ((botAgent.dir == Directions.Left || botAgent.dir == Directions.Right) && Mathf.Abs(transform.position.z - botAgent.Player.transform.position.z) > minionConfig.minDistance)
                {
                    if (botAgent.Player.transform.position.z < transform.position.z)
                        dir = Vector3.back;
                    else
                        dir = Vector3.forward;
                }
                else if (botAgent.dir == Directions.Down && Mathf.Abs(transform.position.x - botAgent.Player.transform.position.x) > minionConfig.minDistance)
                {
                    if (botAgent.Player.transform.position.x < transform.position.x)
                        dir = Vector3.left;
                    else
                        dir = Vector3.right;
                }

                if (dir != Vector3.zero)
                    transform.Translate(dir * (minionConfig.chargeMovSpeed * delta), Space.World);

                prevTime = Time.time;
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