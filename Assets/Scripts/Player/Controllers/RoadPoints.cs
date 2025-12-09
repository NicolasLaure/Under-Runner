using UnityEngine;

namespace Player.Controllers
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Road Points", fileName = "RoadPoints")]
    public class RoadPoints : ScriptableObject
    {
        public float[] xPoints;
        public float zPosition;
    }
}