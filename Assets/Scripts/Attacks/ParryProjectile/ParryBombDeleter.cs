using System;
using System.Collections;
using UnityEngine;

namespace Attacks.ParryProjectile
{
    public class ParryBombDeleter : MonoBehaviour
    {
        [SerializeField] private GameObject destroyParticleObject;
        [SerializeField] private float destroyParticlesSeconds;
        [SerializeField] private GameObject model;
        [SerializeField] private ParryBomb parryBomb;
        [SerializeField] private AK.Wwise.Event explosionSound;
        
        private Coroutine _destroyCoroutine;
        
        private void OnEnable()
        {
            model.gameObject.SetActive(true);
        }

        public void HandleDelete()
        {
            if(_destroyCoroutine != null)
                StopCoroutine(_destroyCoroutine);

            parryBomb.IsActive = false;
            _destroyCoroutine = StartCoroutine(DestroyObstacleCoroutine());
        }

        private IEnumerator DestroyObstacleCoroutine()
        {
            explosionSound.Post(gameObject);
            model.gameObject.SetActive(false);
            destroyParticleObject.gameObject.SetActive(true);
            yield return new WaitForSeconds(destroyParticlesSeconds);
            gameObject.SetActive(false);
        }
    }
}
