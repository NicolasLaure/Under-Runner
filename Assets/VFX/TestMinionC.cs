using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TestMinionC : MonoBehaviour
{
    [SerializeField] Animator minionAnim;
    [SerializeField] Animator redBeamAnim;
    [SerializeField] GameObject redBeam;
    [SerializeField] GameObject goForScale;
    [SerializeField] Transform aimTransform;
    [SerializeField] float delayForBeamToAppear;
    [SerializeField] float beamMaxLength;
    [SerializeField] float timeForBeamToTurnOn;
    [SerializeField] float timeForEachFlicker;
    [SerializeField] float flickerDuration;
    [SerializeField] float delayForFlickering;
    [SerializeField] GameObject damagingBeamGO;
    
    public void Charge()
    {
        minionAnim.SetTrigger("Charge");
        
        Invoke("TurnOnRedBeam", delayForBeamToAppear);
        Invoke("StartFlickering", delayForFlickering);
    }
    public void Release()
    {
        
    }
    public void TurnOnRedBeam()
    {
        StartCoroutine(TurnOnRedBeamCoroutine());
    }
    void StartFlickering()
    {
       
        StartCoroutine(FlickerCoroutine());
    }
    IEnumerator TurnOnRedBeamCoroutine()
    {
        aimTransform.position = new Vector3(beamMaxLength, aimTransform.position.y, aimTransform.position.z);
        float currentLength = 0;
        redBeam.SetActive(true);
        float timer = 0;
        while(timer < timeForBeamToTurnOn) 
        {
            timer += Time.deltaTime;
            currentLength = Mathf.Clamp((timer / timeForBeamToTurnOn) * beamMaxLength,0,beamMaxLength);
            goForScale.transform.localScale = new Vector3(goForScale.transform.localScale.x, goForScale.transform.localScale.y, currentLength);
            yield return null;
        }
        goForScale.transform.localScale = new Vector3(goForScale.transform.localScale.x, goForScale.transform.localScale.y, beamMaxLength);
        
    }
    IEnumerator FlickerCoroutine()
    {
        bool hasStartedCharging = false;
        float timer = 0;
        while(timer < flickerDuration)
        {
            timer += Time.deltaTime;
            float x = timer / flickerDuration;
            float curveValue = Mathf.Sin(Mathf.Pow(x, 2) * 100) * -1;
            redBeam.SetActive(curveValue > 0);
            if(!hasStartedCharging && timer > flickerDuration * (2f/3f))
            {
                hasStartedCharging = true;
                redBeamAnim.SetTrigger("Charge");
            }
            yield return null;
        }
        

        

        float currentLength = 0;
        redBeam.SetActive(true);
        timer = 0;
        while (timer < timeForBeamToTurnOn)
        {
            timer += Time.deltaTime;
            currentLength = Mathf.Clamp((timer / timeForBeamToTurnOn) * beamMaxLength, beamMaxLength,0);
            goForScale.transform.localScale = new Vector3(goForScale.transform.localScale.x, goForScale.transform.localScale.y, currentLength);
            yield return null;
        }
        goForScale.transform.localScale = new Vector3(goForScale.transform.localScale.x, goForScale.transform.localScale.y, 0);
        redBeamAnim.SetTrigger("Release");
        yield return new WaitForSeconds(0.4f);
        redBeam.SetActive(false);
        minionAnim.SetTrigger("Release");
        damagingBeamGO.SetActive(false);
        damagingBeamGO.SetActive(true);


    }
}
