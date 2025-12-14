using System;
using Camera;
using Events;
using Events.ScriptableObjects;
using LevelManagement;
using UnityEngine;

namespace Credits
{
    public class CreditsHandler : MonoBehaviour
    {
        [SerializeField] private CameraSO cameraData;
        [SerializeField] private LevelLoopSO firstLevelConfig;

        [Header("Events")] 
        [SerializeField] private CameraDataChannelSO onCameraMovement;
        [SerializeField] private Vector3EventChannelSO onRoadManagerVelocity;
        
        private void OnEnable()
        {
            onCameraMovement?.RaiseEvent(cameraData);
            onRoadManagerVelocity?.RaiseEvent(firstLevelConfig.minionsData.roadData.roadVelocity);
        }
    }
}
