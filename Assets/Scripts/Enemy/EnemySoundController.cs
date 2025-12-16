using UnityEngine;

namespace Enemy
{
    public class EnemySoundController : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.State creditsState;
        [SerializeField] private AK.Wwise.Event hitSound;
        
        public void WonSound()
        {
            AkSoundEngine.SetState(creditsState.GroupId, creditsState.Id);
        }

        public void Hit()
        {
            hitSound.Post(gameObject);
        }
    }
}
