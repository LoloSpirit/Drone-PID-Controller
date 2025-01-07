using UnityEngine;

public class PIDController
{
    public PIDParameters parameters;

    private float _integrationStored;

    public PIDController(PIDParameters parameters)
    {
        this.parameters = parameters;
    }

    private float AngleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;   //calculate modular difference, and remap to [-180, 180]
    }

    public float UpdatePIDAngle(float timeStep, float targetAngle, float angle, float angularV)
    {
        var error = AngleDifference(targetAngle, angle);
        var p = -parameters.pGain * error;
        var d = angularV * parameters.dGain;
        _integrationStored = Mathf.Clamp(_integrationStored + (error * timeStep), -parameters.iSaturation, parameters.iSaturation);
        var i = parameters.iGain * _integrationStored;
        
        DataUI.Instance.SetData(angle, targetAngle, angularV);
        return Mathf.Clamp(p + i + d, -1, 1);
    }
    
    public float UpdatePID(float timeStep, float target, float current, float vel, bool log)
    {
        var error = target - current;
        var p = parameters.pGain * error;
        var d = vel * parameters.dGain;
        _integrationStored = Mathf.Clamp(_integrationStored + (error * timeStep), -parameters.iSaturation, parameters.iSaturation);
        var i = parameters.iGain * _integrationStored;
        
        if (log)
            Debug.Log($"p {p}, d {d}, i{i}");
        
        //DataUI.Instance.SetData(current, target, vel);
        return Mathf.Clamp(p + i + d, -1, 1);
    }
}
