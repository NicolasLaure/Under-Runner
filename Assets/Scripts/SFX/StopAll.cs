using UnityEngine;

namespace SFX
{
    public class StopAll : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event stopAllEvent;
        
        public void HandleStopSfx()
        {
            stopAllEvent.Post(gameObject);
        }
    }
}
