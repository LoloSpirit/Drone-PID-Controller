using UnityEngine;

[CreateAssetMenu(menuName = "PIDParameters")]
public class PIDParameters : ScriptableObject
{
	[Range(0, 10)]
	public float pGain;
	[Range(0, 10)]
	public float dGain;
	[Range(0, 5)]
	public float iGain;
	[Range(0, 10)]
	public float iSaturation;
}