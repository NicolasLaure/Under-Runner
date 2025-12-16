using System;
using System.Collections;
using System.Linq;
using Camera;
using Events;
using Events.ScriptableObjects;
using LevelManagement.Sequences.Data;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace LevelManagement.Sequences
{
    public class StartLevelSequence : MonoBehaviour
    {
        [SerializeField] private GameObject[] otherPlayers;
        [SerializeField] private GameObject player;
        [SerializeField] private StartLevelSO startLevelConfig;
        
        [Header("Cinematic Events")] 
        [SerializeField] private VoidEventChannelSO onCinematicStartEvent;
        [SerializeField] private VoidEventChannelSO onCinematicPlayerLockStart;
        [SerializeField] private VoidEventChannelSO onCinematicPlayerLockFinished;
        [SerializeField] private VoidEventChannelSO onStartCinematicCanvas;
        [SerializeField] private VoidEventChannelSO onEndCinematicCanvas;
        [SerializeField] private VoidEventChannelSO onCinematicCanvasFinishedAnimation;
        [SerializeField] private BoolEventChannelSO onCinematicUICanvasEvent;
        [SerializeField] private VoidEventChannelSO onCinematicEnded;

        [Header("Gameplay Events")]
        [SerializeField] private BoolEventChannelSO onGameplayUICanvasEvent;

        [SerializeField] private Vector3EventChannelSO onNewRoadsVelocity;
        [SerializeField] private CameraDataChannelSO onCameraMovementEvent;

        [Header("Music")] [SerializeField] private AK.Wwise.State firstPhaseState;
        
        private bool _isCinematicCanvasAnimating;

        private void OnEnable()
        {
            onCinematicCanvasFinishedAnimation?.onEvent.AddListener(HandleFinishedAnimation);
        }

        private void OnDisable()
        {
            onCinematicCanvasFinishedAnimation?.onEvent.RemoveListener(HandleFinishedAnimation);
            onGameplayUICanvasEvent?.RaiseEvent(false);
        }

        public Sequence GetStartSequence(RoadData roadData)
        {
            Sequence startSequence = new Sequence();

            startSequence.AddPreAction(RaiseStartCinematicEvent());
            startSequence.AddPreAction(HandleStartCinematicCanvas());
            startSequence.AddPreAction(StartRoads(roadData));
            startSequence.AddPreAction(MoveOtherPlayers());
            startSequence.SetAction(MovePlayerToMiddle());
            startSequence.AddPostAction(HandleStopCinematicCanvas());
            
            return startSequence;
        }

        private IEnumerator StartRoads(RoadData roadData)
        {
            if (startLevelConfig.isFirstGameplay)
            {
                onCinematicPlayerLockStart?.RaiseEvent();
                onNewRoadsVelocity?.RaiseEvent(roadData.roadVelocity);
                onCameraMovementEvent?.RaiseEvent(startLevelConfig.startCameraData);
                startLevelConfig.isFirstGameplay = false;
                yield return new WaitForSeconds(startLevelConfig.startCameraData.timeToRotate);
            }
            else
            {
                yield return null;
            }
        }

        private void ManageMusic()
        {
            AkSoundEngine.SetState(firstPhaseState.GroupId, firstPhaseState.Id);
        }

        public void SkipCinematic()
        {
            onCinematicPlayerLockFinished?.RaiseEvent();
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y,
                startLevelConfig.playerInitZPosition);
            onGameplayUICanvasEvent?.RaiseEvent(true);
            onCinematicUICanvasEvent?.RaiseEvent(false);
            onCinematicEnded?.RaiseEvent();
            ManageMusic();
            
            foreach (var otherPlayer in otherPlayers)
            {
                otherPlayer.SetActive(false);
            }
        }

        private IEnumerator RaiseStartCinematicEvent()
        {
            onCinematicStartEvent?.RaiseEvent();
            onCinematicPlayerLockStart?.RaiseEvent();
            yield return null;
        }

        private IEnumerator MovePlayerToMiddle()
        {
            player.SetActive(true);
            
            while (player.transform.position.z < startLevelConfig.playerInitZPosition)
            {
                player.transform.position += new Vector3(0, 0, startLevelConfig.playersVelocity) * Time.deltaTime;
                yield return null;
            }

            onCinematicPlayerLockFinished?.RaiseEvent();
        }

        private IEnumerator MoveOtherPlayers()
        {
            foreach (var otherPlayer in otherPlayers)
            {
                otherPlayer.SetActive(true);
            }

            while (otherPlayers.Any(player => player.activeInHierarchy))
            {
                foreach (var otherPlayer in otherPlayers)
                {
                    otherPlayer.transform.position += new Vector3(0, 0, startLevelConfig.playersVelocity) * Time.deltaTime;

                    if (otherPlayer.transform.position.z > startLevelConfig.otherPlayersEndZPosition)
                    {
                        otherPlayer.SetActive(false);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator HandleStopCinematicCanvas()
        {
            onEndCinematicCanvas?.RaiseEvent();
            _isCinematicCanvasAnimating = true;
            ManageMusic();
            yield return new WaitWhile(() => _isCinematicCanvasAnimating);
            onGameplayUICanvasEvent?.RaiseEvent(true);
            onCinematicUICanvasEvent?.RaiseEvent(false);
            onCinematicEnded?.RaiseEvent();
        }

        private IEnumerator HandleStartCinematicCanvas()
        {
            onGameplayUICanvasEvent?.RaiseEvent(false);
            onCinematicUICanvasEvent?.RaiseEvent(true);
            onStartCinematicCanvas?.RaiseEvent();
            _isCinematicCanvasAnimating = true;
            if(!startLevelConfig.isFirstGameplay) yield return new WaitWhile(() => _isCinematicCanvasAnimating);
        }

        private void HandleFinishedAnimation()
        {
            _isCinematicCanvasAnimating = false;
        }
    }
}