using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace VFX
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Motor Flames Data", fileName = "MotorFlamesData")]
    public class MotorFlamesData : ScriptableObject
    { 
        public Vector2 speedMinMax;
        public Vector2 startSizeMinMax;
        public bool areSmallFlamesActive;
    }
}