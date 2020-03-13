using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTop90 : MonoBehaviour
{
    public Transform player;
	Vector3 target, mousePos, refVel, shakeOffset;
	public float cameraDist = 3.5f;
	public float smoothTime = 0.2f;
	private float yStart;
	//shake
	float shakeMag, shakeTimeEnd;
	Vector3 shakeVector;
	bool shaking;

	private bool activated;
	public bool clamped;
	public bool followMouse;

	void Start () {
		target = player.position; //set default target
		yStart = transform.position.y; //capture current y position
	}
	void FixedUpdate () 
	{
		if (activated)
		{
			smoothTime = 0.1f;
			if (followMouse)
			{
				mousePos = CaptureMousePos();
			}
		}
		else
		{
			smoothTime = 0.2f;
		}
		
		if (Input.GetMouseButton(0) && !followMouse)
		{
			activated = true;
			mousePos = CaptureMousePos(); //find out where the mouse is
		}

		if (!Input.GetMouseButton(0))
		{
			if (followMouse)
			{
				activated = true;
			}
			else
			{
				activated = false;
			}
		}
		
		shakeOffset = UpdateShake(); //account for screen shake
		target = UpdateTargetPos(); //find out where the camera ought to be
		UpdateCameraPosition(); //smoothly move the camera closer to it's target location
	}
	Vector3 CaptureMousePos(){
		Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition); //raw mouse pos
		ret *= 2; 
		ret -= Vector2.one; //set (0,0) of mouse to middle of screen
		float max = 0.9f;
		if (Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max){
			ret = ret.normalized; //helps smooth near edges of screen
		}
		Vector3 rete = new Vector3(ret.x, 0, ret.y);
		return rete;
	}
	Vector3 UpdateTargetPos(){
		Vector3 mouseOffset = mousePos * cameraDist; //mult mouse vector by distance scalar 
		Vector3 ret = new Vector3();
		
		if (activated)
		{
			ret = player.position + mouseOffset;
		} //find position as it relates to the player
		else
		{
			ret = player.position;
		}
		ret += shakeOffset; //add the screen shake vector to the target
		ret.y = yStart; //make sure camera stays at same y coord
		return ret;
	}
	Vector3 UpdateShake(){
		if (!shaking || Time.time > shakeTimeEnd){
			shaking = false; //set shaking false when the shake time is up
			return Vector3.zero; //return zero so that it won't effect the target
		}
		Vector3 tempOffset = shakeVector; 
		tempOffset *= shakeMag; //find out how far to shake, in what direction
		return tempOffset;
	}
	void UpdateCameraPosition(){
		Vector3 tempPos;
		tempPos = Vector3.SmoothDamp(transform.position, target, ref refVel, smoothTime); //smoothly move towards the target
		if (clamped)
		{
			float clampedX = Mathf.Clamp(tempPos.x, -0.1f, 3.8f);
			float clampedZ = Mathf.Clamp(tempPos.z, -15.4f, 13.2f);
			tempPos = new Vector3(clampedX,tempPos.y,clampedZ);
		}
		transform.position = tempPos; //update the position
	}

	public void Shake(Vector3 direction, float magnitude, float length){ //capture values set for where it's called
		shaking = true; //to know whether it's shaking
		shakeVector = direction; //direction to shake towards
		shakeMag = magnitude; //how far in that direction
		shakeTimeEnd = Time.time + length; //how long to shake
	}
}
