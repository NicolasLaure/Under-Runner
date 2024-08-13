using UnityEngine;
using UnityEngine.Events;

namespace _Dev.GolfTest.Scripts.Events
{
    [CreateAssetMenu(menuName = "Bool Event Channel")]
    public class BoolEventChannelSO : VoidEventChannelSO
    {
        public UnityEvent<bool> onBoolEvent;

        public void RaiseEvent(bool value)
        {
            if (onBoolEvent != null)
            {
                onBoolEvent.Invoke(value);
            }
            else
            {
                LogNullEventError();
            }
        }
    }
}