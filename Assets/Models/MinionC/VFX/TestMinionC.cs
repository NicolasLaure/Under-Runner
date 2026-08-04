using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TestMinionC : MonoBehaviour
{
    [SerializeField] MinionCVFXController minionOnSideVfxController;
    [SerializeField] MinionCVFXController minionInFrontVfxController;

    private void OnEnable()
    {
        MinionCVFXController.OnFinishedWarning += Release;
    }
    private void OnDisable()
    {
        MinionCVFXController.OnFinishedWarning -= Release;
    }
    public void AimFront()
    {
        minionInFrontVfxController.Aim(true);
    }
    public void ChargeFront()
    {
        minionInFrontVfxController.Charge(2f);
    }
    public void AimSide()
    {
        minionOnSideVfxController.Aim(false);
    }
    public void ChargeSide()
    {
        minionOnSideVfxController.Charge(2f);
    }
    void Release(MinionCVFXController vfxController)
    {
        if(vfxController == minionOnSideVfxController)
        {
            minionOnSideVfxController.Release();
            Invoke("LeaveSide", 3f);
        }
        else if(vfxController == minionInFrontVfxController)
        {
            minionInFrontVfxController.Release();
            Invoke("LeaveFront", 3f);
        }
        

    }
    void LeaveFront()
    {
        minionInFrontVfxController.Leave();
    }
    void LeaveSide()
    {
        minionOnSideVfxController.Leave();
    }
}
