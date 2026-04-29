using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCVFXController : MonoBehaviour
{
    [SerializeField] Animator minionAnim;
    [SerializeField] Animator redBeamAnim;
    [SerializeField] GameObject redBeam;
    [SerializeField] GameObject goForScale;
    [SerializeField] Transform aimTransform;
    [SerializeField] float timeForBeamToAppear;
    [SerializeField] float beamMaxLength;
    [SerializeField] float timeForBeamToTurnOn;
    [SerializeField] float vortexAnimationDuration = 0.28f;
    float flickerDuration = 1;
    [SerializeField] GameObject damagingBeamGO;
    bool isFlickering = false;

    public delegate void AnimationEvent(MinionCVFXController vfxController);
    public static event AnimationEvent OnFinishedWarning;
    public static event AnimationEvent OnFinishedAiming;

    public void SetBeamMaxLength(float length)
    {
        beamMaxLength = length;
    }
    public void Aim(bool aimDown)
    {
        minionAnim.SetBool("isAimingDown", aimDown);
        minionAnim.SetTrigger("Aim");
        StartCoroutine(TurnOnRedBeamCoroutine());
    }
    

    public void Charge(float warningDuration)
    {
        isFlickering = true;
        this.flickerDuration = warningDuration;
        StartCoroutine(FlickerCoroutine());
    }

    public void Release()
    {
        redBeam.SetActive(false);
        minionAnim.SetTrigger("Release");
        damagingBeamGO.SetActive(false);
        damagingBeamGO.SetActive(true);
    }
    public void Leave()
    {
        minionAnim.SetTrigger("Leave");
    }
    
    IEnumerator TurnOnRedBeamCoroutine()
    {
        yield return new WaitForSeconds(timeForBeamToAppear);
        aimTransform.localPosition = new Vector3(beamMaxLength, aimTransform.localPosition.y, aimTransform.localPosition.z);
        float currentLength = 0;
        redBeam.SetActive(true);
        float timer = 0;
        while (timer < timeForBeamToTurnOn)
        {
            timer += Time.deltaTime;
            currentLength = Mathf.Clamp((timer / timeForBeamToTurnOn) * beamMaxLength, 0, beamMaxLength);
            goForScale.transform.localScale = new Vector3(goForScale.transform.localScale.x, goForScale.transform.localScale.y, currentLength);
            yield return null;
        }
        goForScale.transform.localScale = new Vector3(goForScale.transform.localScale.x, goForScale.transform.localScale.y, beamMaxLength);
        if (OnFinishedAiming != null) OnFinishedAiming(this);
    }
    IEnumerator FlickerCoroutine()
    {
        minionAnim.SetTrigger("Charge");
        bool hasStartedCharging = false;
        float timer = 0;
        flickerDuration -= vortexAnimationDuration;
        while (timer < flickerDuration)
        {
            timer += Time.deltaTime;
            float x = timer / flickerDuration;
            float curveValue = Mathf.Sin(Mathf.Pow(x, 2) * 100) * -1;
            redBeam.SetActive(curveValue > 0);
            if (!hasStartedCharging && timer > flickerDuration * 0.5f)
            {
                hasStartedCharging = true;
                redBeamAnim.SetTrigger("Charge");
            }
            yield return null;
        }
        redBeam.SetActive(false);
        redBeamAnim.SetTrigger("Release");
        yield return new WaitForSeconds(vortexAnimationDuration);
        if(OnFinishedWarning != null)
        {
            OnFinishedWarning(this);
        }
    }
}
