using Player.Animation;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerAgent))]
    public class PlayerController : MonoBehaviour
    {
        protected PlayerAgent playerAgent;
        protected Collider playerCollider;

        [Header("Animation Handler")]
        [SerializeField] protected PlayerAnimationHandler animatorHandler;

        public virtual void OnEnable()
        {
            playerAgent ??= GetComponent<PlayerAgent>();
            playerCollider ??= GetComponent<Collider>();
        }
    }
}