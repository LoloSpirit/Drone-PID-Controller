using System;
using UnityEngine;

public class DroneController: MonoBehaviour
{
	private Rigidbody _rigidbody;
	public Transform motor1;
	public Transform motor2;
	public Transform motor3;
	public Transform motor4;

	public PIDParameters rollTargetParameters;
	public PIDParameters pitchTargetParameters;
	
	public PIDParameters rollParameters;
	public PIDParameters pitchParameters;
	public PIDParameters yawParameters;
	public PIDParameters thrustParameters;

	public float maxVelocity;
	public float maxYaw;
	public float maxThrust;
	
	public Vector3 windSpeed;
	private float _targetVelocityX;
	private float _targetVelocityY;
	private float _targetVelocityZ;
	public float targetYaw;
	private float _lastVelocityX;
	private float _lastVelocityY;
	private float _lastVelocityZ;

	private PIDController _rollTargetController;
	private PIDController _pitchTargetController;
	private PIDController _rollController;
	private PIDController _pitchController;
	private PIDController _thrustController;
	private PIDController _yawController;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rollController = new PIDController(rollParameters);
		_thrustController  = new PIDController(thrustParameters);
		_rollTargetController = new PIDController(rollTargetParameters);
		_pitchTargetController = new PIDController(pitchTargetParameters);
		_pitchController = new PIDController(pitchParameters);
		_yawController = new PIDController(yawParameters);
	}

	private void Update()
	{
		//remote control
		_targetVelocityX = -Input.GetAxis("Horizontal") * maxVelocity;
		_targetVelocityZ = -Input.GetAxis("Vertical") * maxVelocity;
		targetYaw += Input.GetKey(KeyCode.E) ? maxYaw : Input.GetKey(KeyCode.Q) ? -maxYaw : 0;
		_targetVelocityY = Input.GetKey(KeyCode.LeftShift) ? maxThrust : Input.GetKey(KeyCode.LeftControl) ? -maxThrust : 0;
	}

	private void FixedUpdate()
	{
		var localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
		var localAngularVelocity = transform.InverseTransformDirection(_rigidbody.angularVelocity);
		
		//pitch target
		var zAccel = (_lastVelocityZ - localVelocity.z) / Time.fixedDeltaTime;
		_lastVelocityZ = localVelocity.z;
		var pitchTarget = _pitchTargetController.UpdatePID(Time.fixedDeltaTime, _targetVelocityZ, _lastVelocityZ, zAccel, false);
		pitchTarget *= 45;
		
		//pitch
		var pitchCmd = _pitchController.UpdatePIDAngle(Time.fixedDeltaTime, pitchTarget, transform.localEulerAngles.x, localAngularVelocity.x);

		//yaw
		var yawCmd = _yawController.UpdatePIDAngle(Time.fixedDeltaTime, targetYaw, transform.localEulerAngles.y, localAngularVelocity.y);
		
		//roll target
		var xAccel = (_lastVelocityX - localVelocity.x) / Time.fixedDeltaTime;
		_lastVelocityX = localVelocity.x;
		var rollTarget = _rollTargetController.UpdatePID(Time.fixedDeltaTime, _targetVelocityX, _lastVelocityX, xAccel, false);
		rollTarget *= -45;
		
		//roll
		var rollCmd = _rollController.UpdatePIDAngle(Time.fixedDeltaTime, rollTarget, transform.localEulerAngles.z, localAngularVelocity.z);
		
		//thrust
		var accelerationY = (_lastVelocityY - _rigidbody.velocity.y) / Time.fixedDeltaTime;
		_lastVelocityY = _rigidbody.velocity.y;
		var thrustCmd = _thrustController.UpdatePID(Time.fixedDeltaTime, _targetVelocityY, _rigidbody.velocity.y, accelerationY, false);
		thrustCmd += _rigidbody.mass * 9.81f / 4;
		var motor1Cmd = thrustCmd + rollCmd - pitchCmd - yawCmd;
		var motor2Cmd = thrustCmd + rollCmd + pitchCmd + yawCmd;
		var motor3Cmd = thrustCmd - rollCmd + pitchCmd - yawCmd;
		var motor4Cmd = thrustCmd - rollCmd - pitchCmd + yawCmd;
		
		//add forces and torques
		_rigidbody.AddForceAtPosition(motor1.up * motor1Cmd, motor1.position);
		_rigidbody.AddForceAtPosition(motor2.up * motor2Cmd, motor2.position);
		_rigidbody.AddForceAtPosition(motor3.up * motor3Cmd, motor3.position);
		_rigidbody.AddForceAtPosition(motor4.up * motor4Cmd, motor4.position);
		var up = transform.up;
		_rigidbody.AddTorque(up * motor1Cmd);
		_rigidbody.AddTorque(-up * motor2Cmd);
		_rigidbody.AddTorque(up * motor3Cmd);
		_rigidbody.AddTorque(-up * motor4Cmd);
		
		//add wind
		_rigidbody.AddForce(windSpeed, ForceMode.Acceleration);
	}
}