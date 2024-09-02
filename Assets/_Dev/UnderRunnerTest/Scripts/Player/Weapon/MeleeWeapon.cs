using System;
using System.Collections;
using System.Collections.Generic;
using _Dev.UnderRunnerTest.Scripts.Health;
using _Dev.UnderRunnerTest.Scripts.ParryProjectile;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemy;

    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Deflectable"))
        {
            if (other.transform.TryGetComponent<IDeflectable>(out IDeflectable deflectableInterface))
            {
                deflectableInterface.Deflect(enemy);
            }

            transform.parent.gameObject.SetActive(false);
        }

        if (other.transform.CompareTag("Enemy"))
        {
            if (other.transform.TryGetComponent<ITakeDamage>(out ITakeDamage takeDamageInterface))
            {
                takeDamageInterface.TakeDamage(damage);
            }

            transform.parent.gameObject.SetActive(false);
        }
    }
}