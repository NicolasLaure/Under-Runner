using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    bool flickering = false;
    [SerializeField] float timeForEachFlicker;
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
        minionAnim.SetTrigger("Release");
        redBeamAnim.SetTrigger("Release");
        flickering = false;
        damagingBeamGO.SetActive(false);
        damagingBeamGO.SetActive(true);
    }
    public void TurnOnRedBeam()
    {
        StartCoroutine(TurnOnRedBeamCoroutine());
    }
    void StartFlickering()
    {
        redBeamAnim.SetTrigger("Charge");
        flickering = true;
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
        bool isOn = true;
        float timer = 0;
        while (flickering)
        {
            timer += Time.deltaTime;
            if(timer >= timeForEachFlicker)
            {
                isOn = !isOn;
                timer = 0;
            }
            redBeam.SetActive(isOn);
            yield return null;
        }
        redBeam.SetActive(false);
    }
}
