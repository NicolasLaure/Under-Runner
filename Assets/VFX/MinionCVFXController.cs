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
    [SerializeField] Transform frontBeamTransform;
    [SerializeField] Transform sideBeamTransform;
    bool isFlickering = false;

    public delegate void AnimationEvent(MinionCVFXController vfxController);
    public static event AnimationEvent OnFinishedWarning;
    public static event AnimationEvent OnFinishedAiming;
    bool aimingDown = false;

    /// <summary>
    /// Use positive values for them to go on the same direction as the minion or negatives when going backwards.
    /// </summary>
    public void UpdateWheelSpinningSpeed(float speed)
    {
        minionAnim.SetFloat("WheelSpeed", speed);
    }
    /// <summary>
    /// Changes direction of the wheels, were 0 is left, 1 is right, and 0.5 is centered.
    /// </summary>
    public void UpdateWheelSteering(float steer)
    {
        minionAnim.SetFloat("DirectionWheels", Mathf.Clamp01(steer));

    }
    /// <summary>
    /// In case the range of the red beam is too short, it can be changed here.
    /// </summary>
    public void SetBeamMaxLength(float length)
    {
        beamMaxLength = length;
    }
    /// <summary>
    /// Plays the Aim animation, and fires "OnFinishedAiming" when the red beam is on.
    /// </summary>
    /// <param name="aimDown"> Decides which animations should take place for this current Minion. </param>
    public void Aim(bool aimDown)
    {
        aimingDown = aimDown;
        minionAnim.SetBool("isAimingDown", aimDown);
        minionAnim.SetTrigger("Aim");
        StartCoroutine(TurnOnRedBeamCoroutine());
    }

    /// <summary>
    /// Gives a warning for the desired duration and then fires "OnFinishedWarning" once its ready to Release the charged shot.
    /// </summary>
    public void Charge(float warningDuration)
    {
        isFlickering = true;
        this.flickerDuration = warningDuration;
        StartCoroutine(FlickerCoroutine());
    }


    /// <summary>
    /// Fires the beam and goes back to IDLE.
    /// </summary>
    public void Release()
    {
        Transform transformToMatch;
        if (aimingDown) transformToMatch = frontBeamTransform;
        else transformToMatch = sideBeamTransform;

        damagingBeamGO.transform.position = transformToMatch.position;
        damagingBeamGO.transform.rotation = transformToMatch.rotation;

        redBeam.SetActive(false);
        minionAnim.SetTrigger("Release");
        damagingBeamGO.SetActive(false);
        damagingBeamGO.SetActive(true);
    }
    /// <summary>
    /// Enters the "leaving" animation.
    /// </summary>
    public void Leave()
    {
        minionAnim.SetTrigger("Leave");
    }
    
    IEnumerator TurnOnRedBeamCoroutine()
    {
        yield return new WaitForSeconds(timeForBeamToAppear);
        if(!aimingDown)aimTransform.localPosition = new Vector3(beamMaxLength, aimTransform.localPosition.y, 0);
        else aimTransform.localPosition = new Vector3(0, aimTransform.localPosition.y, beamMaxLength);
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
