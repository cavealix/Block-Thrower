using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Vive.VR;

public class HandController : MonoBehaviour {
	public SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device;
	public float throwForce = 1.5f;
	public Rigidbody rigidBody;

	//Swipe
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;
	public ObjectMenuManager objectMenuManager;

	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		device = SteamVR_Controller.Input((int)trackedObj.index);

		//set initial touch to 0
		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
		{
			touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
		}

		//detect touch
		if(device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
		{
			touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			//how much did finger move this frame?
			distance = touchCurrent - touchLast;
			touchLast = touchCurrent;
			//total movement
			swipeSum += distance;

			if(!hasSwipedRight)
			{
				if(swipeSum > 0.5f)
				{
					swipeSum = 0;
					SwipeRight();
					hasSwipedLeft = true;
					hasSwipedLeft = false;
				}
			}
			if(!hasSwipedLeft)
			{
				if(swipeSum < -0.5f)
				{
					swipeSum = 0;
					SwipeLeft();
				}
				hasSwipedLeft = true;
				hasSwipedRight = false;
			}
		}

		//reset variable if finger up 
		if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
		{
			swipeSum = 0;
			touchCurrent = 0;
			touchLast = 0;
			hasSwipedLeft = false;
			hasSwipedRight = false;
		}

		//Spawn object currently selected by menu
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
		{
			SpawnObject();
		}
	}

	void SpawnObject()
	{
		objectMenuManager.SpawnCurrentObject();
	}

	//Swipe
	void SwipeLeft()
	{
		objectMenuManager.MenuLeft();
		//Debug.Log("SwipeLeft");
	}
	void SwipeRight()
	{
		objectMenuManager.MenuRight();
		//Debug.Log("SwipeRight");
	}

	//Hold Objects
	void OnTriggerStay(Collider col)
	{
		if(col.gameObject.CompareTag("Throwable"))
		{
			if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
			{
				ThrowObject(col);
			}
			else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
			{
				GrabObject(col);
			}
		}
	}

	//Grab objects with Trigger
	void GrabObject(Collider coli)
	{
		coli.transform.SetParent(gameObject.transform);
		coli.GetComponent<Rigidbody>().isKinematic = true;
		device.TriggerHapticPulse(2000);
		Debug.Log("you are touching down on the trigger on an object");
	}
	void ThrowObject(Collider coli)
	{
		coli.transform.SetParent(null);
		Rigidbody rigidBody = coli.GetComponent<Rigidbody>();
		rigidBody.isKinematic = false;
		rigidBody.velocity = device.velocity * throwForce;
		rigidBody.angularVelocity = device.angularVelocity;
		Debug.Log("You have released the trigger");
	}



}
