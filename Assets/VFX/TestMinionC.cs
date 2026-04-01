using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMinionC : MonoBehaviour
{
    [SerializeField] Animator minionAnim;
    [SerializeField] Animator redBeamAnim;
    public void Charge()
    {
        minionAnim.SetTrigger("Charge");
        redBeamAnim.SetTrigger("Charge");
    }
    public void Release()
    {
        minionAnim.SetTrigger("Release");
        redBeamAnim.SetTrigger("Release");
    }
}
