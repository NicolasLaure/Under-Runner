using Health;
using UnityEngine;
using UnityEngine.Serialization;

public class LaserCollider : MonoBehaviour
{
    [SerializeField] private LaserBotConfigSO minionConfig;
    [FormerlySerializedAs("_collider")]
    [SerializeField] private Collider collider;

    public void SetCollision(bool value)
    {
        collider.enabled = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DealDamage(other.gameObject);
        }
    }

    private void DealDamage(GameObject target)
    {
        target.gameObject.TryGetComponent(out ITakeDamage playerHealth);
        playerHealth.TryTakeDamage(minionConfig.damage);
    }
}