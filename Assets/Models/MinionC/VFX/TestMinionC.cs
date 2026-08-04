using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TestMinionC : MonoBehaviour
{
    [SerializeField] MinionCVFXController vfxController;

    private void OnEnable()
    {
        MinionCVFXController.OnFinishedWarning += Release;
    }
    private void OnDisable()
    {
        MinionCVFXController.OnFinishedWarning -= Release;
    }
    public void Aim()
    {
        vfxController.Aim(true);
    }
    public void Charge()
    {
        vfxController.Charge(2f);
    }
    void Release(MinionCVFXController vfxController)
    {
        if(vfxController == this.vfxController)
        {
            vfxController.Release();
        }
        Invoke("Leave", 3f);

    }
    void Leave()
    {
        vfxController.Leave();
    }
}
