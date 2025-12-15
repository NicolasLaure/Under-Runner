using Events;
using Events.ScriptableObjects;
using Input;
using UnityEngine;

namespace Managers.Pause
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private InputHandlerSO inputHandler;
        [SerializeField] private PauseSO pauseData;

        [Header("Events")]
        [SerializeField] private BoolEventChannelSO onHandlePauseEvent;
        [SerializeField] private VoidEventChannelSO onCinematicStarted;
        [SerializeField] private VoidEventChannelSO onPlayerCinematicLock;
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onGameplayReset;
        [SerializeField] private VoidEventChannelSO onCinematicEnded;

        [Header("Music")]
        [SerializeField] private AK.Wwise.Event pauseMusicEvent;
        [SerializeField] private AK.Wwise.Event unpauseMusicEvent;

        private float _lastTimeScale;
        private bool _isPauseBlocked;

        private void OnEnable()
        {
            pauseData.isPaused = false;
            _isPauseBlocked = false;

            onCinematicStarted?.onEvent.AddListener(HandleBlockPause);
            onPlayerCinematicLock?.onEvent.AddListener(HandleBlockPause);
            onPlayerDeath?.onEvent.AddListener(HandleBlockPause);
            onCinematicEnded?.onEvent.AddListener(HandleUnblockPause);
            onGameplayReset?.onEvent.AddListener(HandleUnblockPause);
            onHandlePauseEvent?.onTypedEvent.AddListener(HandlePause);
            inputHandler?.onPauseToggle.AddListener(HandlePause);
        }

        private void OnDisable()
        {
            onCinematicStarted?.onEvent.RemoveListener(HandleBlockPause);
            onPlayerCinematicLock?.onEvent.RemoveListener(HandleBlockPause);
            onPlayerDeath?.onEvent.RemoveListener(HandleBlockPause);
            onGameplayReset?.onEvent.RemoveListener(HandleUnblockPause);
            onCinematicEnded?.onEvent.RemoveListener(HandleUnblockPause);
            onHandlePauseEvent?.onTypedEvent.RemoveListener(HandlePause);
            inputHandler?.onPauseToggle.RemoveListener(HandlePause);
        }

        private void HandleUnblockPause()
        {
            _isPauseBlocked = false;
        }

        private void HandleBlockPause()
        {
            _isPauseBlocked = true;
        }

        private void HandlePause(bool value)
        {
            pauseData.SetIsPaused(value);

            if (pauseData.isPaused)
            {
                AkSoundEngine.PostEvent(pauseMusicEvent.Id, gameObject);
                _lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                AkSoundEngine.PostEvent(unpauseMusicEvent.Id, gameObject);
                Time.timeScale = _lastTimeScale;
            }
        }

        private void HandlePause()
        {
            if (_isPauseBlocked)
                return;

            onHandlePauseEvent.RaiseEvent(!pauseData.isPaused);
        }
    }
}