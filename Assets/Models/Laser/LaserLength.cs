using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLength : MonoBehaviour
{
    [SerializeField] private float laserLength = 0;
    [SerializeField] private float laserWidth = 2;
    [SerializeField] LineRenderer beamLineRenderer;
    [SerializeField] ParticleSystem startParticleSystem;
    
    private float _currentLaserLength;

    public void UpdateBeamWidth()
    {
        beamLineRenderer.widthMultiplier = laserWidth;
    }

    public void UpdateBeamLength(float time)
    {
        _currentLaserLength = Mathf.Lerp(0, laserLength, time);
        beamLineRenderer.SetPosition(1, new Vector3(0, 0, _currentLaserLength));
    }

    public void UpdateStartParticleEmission()
    {
        var emission = startParticleSystem.emission;
        emission.rateOverTime = Mathf.Lerp(0, 60, Mathf.Clamp(laserLength, 0, 1));
    }

    public void SetLaserGradient(Gradient newColor)
    {
        beamLineRenderer.colorGradient = newColor;
    }

    public void SetLineParticles(bool hasParticles)
    {
        startParticleSystem.gameObject.SetActive(hasParticles);
    }
}
