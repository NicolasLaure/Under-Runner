using System;
using System.Collections;
using Events;
using UnityEngine;

namespace UI
{
    public class CinematicUIBars : MonoBehaviour
    {
        [Header("Bars")] 
        [SerializeField] private GameObject topBar;
        [SerializeField] private GameObject bottomBar;
        [SerializeField] private float barMovementVelocity;
        [SerializeField] private float waitTime = 0.5f;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onStartCinematicCanvas;
        [SerializeField] private VoidEventChannelSO onEndCinematicCanvas;
        [SerializeField] private VoidEventChannelSO onCinematicCanvasFinishedAnimation;

        private Coroutine _moveIntoScreenCoroutine;
        private Coroutine _moveOutOfScreenCoroutine;
        private Vector3 _initTopLocalPosition;
        private Vector3 _initBottomLocalPosition;
        
        private void OnEnable()
        {
            onStartCinematicCanvas?.onEvent.AddListener(MoveBarsToScreen);
            onEndCinematicCanvas?.onEvent.AddListener(MoveBarsOutOfScreen);

            _initTopLocalPosition = topBar.transform.localPosition;
            _initBottomLocalPosition = bottomBar.transform.localPosition;
        }

        private void OnDisable()
        {
            topBar.transform.localPosition = _initTopLocalPosition;
            bottomBar.transform.localPosition = _initBottomLocalPosition;
        }

        private void MoveBarsOutOfScreen()
        {
            if (_moveOutOfScreenCoroutine != null)
            {
                StopCoroutine(_moveOutOfScreenCoroutine);
            }
            _moveOutOfScreenCoroutine = StartCoroutine(MoveBarsOutOfScreenCoroutine());
        }

        private IEnumerator MoveBarsOutOfScreenCoroutine()
        {
            RectTransform topTransform = topBar.GetComponent<RectTransform>();
            RectTransform bottomTransform = bottomBar.GetComponent<RectTransform>();

            float movedPixels = 0;
            while (movedPixels < bottomTransform.rect.height / 2)
            { 
                topTransform.localPosition += Vector3.up * (barMovementVelocity * Time.deltaTime);
                bottomTransform.localPosition += Vector3.down * (barMovementVelocity * Time.deltaTime);
                
                movedPixels += barMovementVelocity * Time.deltaTime;
                yield return null;
            }

            
            onCinematicCanvasFinishedAnimation?.RaiseEvent();
        }

        private void MoveBarsToScreen()
        {
            if (_moveIntoScreenCoroutine != null)
            {
                StopCoroutine(_moveIntoScreenCoroutine);
            } 
            _moveIntoScreenCoroutine = StartCoroutine(MoveBarsToScreenCoroutine());
        }

        private IEnumerator MoveBarsToScreenCoroutine()
        {
            yield return new WaitForSeconds(waitTime);

            RectTransform topTransform = topBar.GetComponent<RectTransform>();
            RectTransform bottomTransform = bottomBar.GetComponent<RectTransform>();

            float movedPixels = 0;
            while (movedPixels < bottomTransform.rect.height / 2)
            { 
                topTransform.localPosition += Vector3.down * (barMovementVelocity * Time.deltaTime);
                bottomTransform.localPosition += Vector3.up * (barMovementVelocity * Time.deltaTime);
                
                movedPixels += barMovementVelocity * Time.deltaTime;
                yield return null;
            }
            
            onCinematicCanvasFinishedAnimation?.RaiseEvent();
        }
    }
}
