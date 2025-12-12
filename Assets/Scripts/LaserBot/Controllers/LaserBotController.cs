using UnityEngine;

namespace LaserBot.Controllers
{
    [RequireComponent(typeof(LaserBotAgent))]
    public class LaserBotController : MonoBehaviour
    {
        [SerializeField] protected LaserBotAgent botAgent;
    }
}