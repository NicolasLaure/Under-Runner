using Events.ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [Header("events")] 
        [SerializeField] private BoolEventChannelSO onHandleCanvas;

        private void Awake()
        {
            onHandleCanvas?.onTypedEvent.AddListener(HandleCanvas);
            canvas.SetActive(false);
        }

        private void HandleCanvas(bool value)
        {
            canvas.SetActive(value);
        }
    }
}
