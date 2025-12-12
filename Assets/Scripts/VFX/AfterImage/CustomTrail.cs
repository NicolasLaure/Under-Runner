using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace VFX.AfterImage
{
    public class CustomTrail : MonoBehaviour
    {
        [SerializeField] private float cloneDuration = 1f;
        [SerializeField] private float timeBetweenInstances = 0.1f;
        [SerializeField] private Vector3 offset;

        private bool _shouldSpawnImages;
        private Coroutine _startCoroutine;
        private List<GameObject> _afterImageObjects;

        public void OnEnable()
        {
            _afterImageObjects = new List<GameObject>();
        }

        public void OnDisable()
        {
            foreach (var afterImageObject in _afterImageObjects.ToList())
            {
                if (afterImageObject == null)
                    continue;

                afterImageObject.SetActive(false);
                _afterImageObjects.Remove(afterImageObject);
            }
        }

        public void StartSpawn()
        {
            if (_startCoroutine != null)
            {
                StopCoroutine(_startCoroutine);
            }

            _shouldSpawnImages = true;
            _startCoroutine = StartCoroutine(SpawnAfterImage());
        }

        public void StopSpawn()
        {
            _shouldSpawnImages = false;
        }

        IEnumerator SpawnAfterImage()
        {
            while (_shouldSpawnImages)
            {
                yield return new WaitForSeconds(timeBetweenInstances);

                GameObject afterImage = AfterImagePool.Instance.GetPooledObject();
                afterImage.transform.SetPositionAndRotation(transform.position + offset, transform.rotation);
                afterImage.SetActive(true);
                _afterImageObjects.Add(afterImage);
                StartCoroutine(FadeOut(afterImage));
            }
        }

        IEnumerator FadeOut(GameObject afterImage)
        {
            Material oldMat = afterImage.GetComponentInChildren<SkinnedMeshRenderer>().material;
            Material diffuseMaterial = Instantiate(oldMat);
            float elapsedTime = 0f;
            Color initialColor = diffuseMaterial.color;
            SkinnedMeshRenderer[] meshRenderers = afterImage.GetComponentsInChildren<SkinnedMeshRenderer>();
            while (elapsedTime < cloneDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(initialColor.a, 0f, elapsedTime / cloneDuration);
                diffuseMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.material = diffuseMaterial;
                }

                yield return null;
            }

            foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = oldMat;
            }

            Destroy(diffuseMaterial);
            _afterImageObjects.Remove(afterImage);
            afterImage.SetActive(false);
        }
    }
}