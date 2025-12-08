using DEBUG.Input;
using Events.ScriptableObjects;
using Health;
using UnityEngine;

namespace DEBUG.Cheats
{
    [RequireComponent(typeof(DebugInputReader))]
    public class CheatsManager : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || ENABLE_CHEATS

        [SerializeField] private CheatsConfigSO config;
        [SerializeField] private HealthPoints playerHealth;

        private bool _isOverlayActive;

        void Start()
        {
            config.playerHealth = playerHealth;
        }

        public void ToggleInvincibility(bool value)
        {
            playerHealth.ToggleInvulnerability(value);
        }
#endif
    }
}