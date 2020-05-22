using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraTop90 : MonoBehaviour
{
	private Camera cam;
	private Vector2 viewPortSize;
	
    public Transform player;
	Vector3 target, mousePos, refVel, shakeOffset;
	public float cameraDist = 3.5f;
	public float smoothTime = 0.2f;

	public float yStart;

	private bool activated;
	public bool clamped;
	public bool followMouse;
	// [SerializeField] private float offsetX;
	[SerializeField] private float offsetZ = -4f;
	
	//Shake2
	Vector3 cameraInitialPosition;
	public float shakeMagnetude = 0.05f, shakeTime = 0.5f;
	
	void Start () 
	{
		cam = Camera.main;
		viewPortSize = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)) - cam.ScreenToWorldPoint(Vector2.zero);

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
		//ret.x = ret.x /*+ offsetX*/;
		ret.z = ret.z + offsetZ;
		ret.y = yStart; //make sure camera stays at same y coord
		return ret;
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
	
	public void ShakeIt()
	{
		cameraInitialPosition = UpdateTargetPos();
		InvokeRepeating ("StartCameraShaking", 0f, 0.005f);
		Invoke ("StopCameraShaking", shakeTime);
	}

	void StartCameraShaking()
	{
		float cameraShakingOffsetX = Random.value * shakeMagnetude * 2 - shakeMagnetude;
		float cameraShakingOffsetY = Random.value * shakeMagnetude * 2 - shakeMagnetude;
		Vector3 cameraIntermadiatePosition = cam.transform.position;
		cameraIntermadiatePosition.x += cameraShakingOffsetX;
		cameraIntermadiatePosition.y += cameraShakingOffsetY;
		cam.transform.position = cameraIntermadiatePosition;
	}

	void StopCameraShaking()
	{
		CancelInvoke ("StartCameraShaking");
		cam.transform.position = cameraInitialPosition;
	}

	private void OnDrawGizmosSelected()
	{
		Color c = Color.red;
		c.a = 0.3f;
		Gizmos.color = c;
		
		Gizmos.DrawCube(transform.position, viewPortSize);
	}
}
