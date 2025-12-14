using System;
using System.Collections;
using Attacks.FallingAttack;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Attacks.FallingBlock
{
    public class FallingAttack : MonoBehaviour
    {
        [SerializeField] private Vector2 initialHeightRange = new Vector2(10.0f, 15.0f);
        [SerializeField] private float heightToDestroy = -1f;
        [SerializeField] private GameObject parentObject;
        [SerializeField] private AK.Wwise.Event debrisDestroyedEvent;
        [SerializeField] private GameObject debrisVfx;
        [SerializeField] private float timeToReturnToPool = 1f;
        [SerializeField] private GameObject[] objectsToInactivate;
        [SerializeField] private Collider attackCollider;
        
        [Header("Events")]
        [SerializeField] private GameObjectEventChannelSO onFallingBlockDisabledEvent;

        private float _acceleration;
        private float _velocity;
        private Coroutine _deleteCoroutine;
        private void Start()
        {
            _velocity = 0;
            SetHeight();
        }

        private void OnEnable()
        {
            if(_deleteCoroutine != null) StopCoroutine(_deleteCoroutine);
            _deleteCoroutine = null;
        }

        private void Update()
        {
            if (transform.position.y < heightToDestroy)
            {
                SetHeight();
                _velocity = 0;
                onFallingBlockDisabledEvent?.RaiseEvent(parentObject);
                debrisDestroyedEvent?.Post(parentObject);
                debrisVfx.SetActive(true);

                foreach (GameObject objectToInactivate in objectsToInactivate)
                {
                    objectToInactivate?.SetActive(false);
                }

                attackCollider.enabled = false;
                if (_deleteCoroutine != null) return;
                _deleteCoroutine = StartCoroutine(DeleteCoroutine());
            }
            else
            {
                _velocity += _acceleration * Time.deltaTime;
                transform.position += Vector3.down * (_velocity * Time.deltaTime);
            }
        }

        private IEnumerator DeleteCoroutine()
        {
            yield return new WaitForSeconds(timeToReturnToPool);
            
            foreach (GameObject objectToInactivate in objectsToInactivate)
            {
                objectToInactivate?.SetActive(true);
            }

            attackCollider.enabled = true;
            debrisVfx.SetActive(false);
            FallingBlockObjectPool.Instance?.ReturnToPool(parentObject);
        }

        public void SetAcceleration(float newAcceleration)
        {
            _acceleration = newAcceleration;
        }

        private void SetHeight()
        {
            Vector3 newPosition = transform.position;
            newPosition.y = Random.Range(initialHeightRange.x, initialHeightRange.y);
            transform.position = newPosition;
        }
    }
}
