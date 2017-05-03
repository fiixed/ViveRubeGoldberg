using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager: MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;
    public ObjectMenuManager objectMenuManager;

    // Teleporter
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation;
    public GameObject player;
    public LayerMask laserMask;
    public float yNudgeAmount = 1f;

    // Grab & Throw
    private float throwforce = 1.5f;

    // Swipe
    public float swipeSum;
    public float touchLast;
    public float touchCurrent;
    public float distance;
    public bool hasSwipedLeft;
    public bool hasSwipedRight;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponentInChildren<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device == SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost))) {
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) {
                touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            }
            if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
                touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                distance = touchCurrent - touchLast;
                touchLast = touchCurrent;
                swipeSum += distance;
                if (!hasSwipedRight) {
                    if (swipeSum > 0.5f) {
                        swipeSum = 0;
                        SwipeRight();
                        hasSwipedRight = true;
                        hasSwipedLeft = false;
                    }
                }
                if (!hasSwipedLeft) {
                    if (swipeSum < -0.5f) {
                        swipeSum = 0;
                        SwipeLeft();
                        hasSwipedRight = false;
                        hasSwipedLeft = true;
                    }
                }
                
            }
            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)) {
                swipeSum = 0;
                touchCurrent = 0;
                touchLast = 0;
                hasSwipedRight = false;
                hasSwipedLeft = false;
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
                SpawnObject();
            }
        }
        if (device == SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost))) {
            if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
                laser.gameObject.SetActive(true);
                teleportAimerObject.gameObject.SetActive(true);

                laser.SetPosition(0, transform.position);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask)) {
                    teleportLocation = hit.point;
                    laser.SetPosition(1, teleportLocation);
                    // aimer position
                    teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
                } else {
                    teleportLocation = transform.position + transform.forward * 15;
                    RaycastHit groundRay;
                    if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask)) {
                        teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);
                    }
                    laser.SetPosition(1, transform.forward * 15 + transform.position);
                    // aimer position
                    teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
                }
            }

            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
                laser.gameObject.SetActive(false);
                teleportAimerObject.gameObject.SetActive(false);
                player.transform.position = teleportLocation;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Throwable")  || other.gameObject.CompareTag("Structure")) {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
                ThowObject(other);
            } else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
                GrabObject(other);
            }
        }
    }

    private void GrabObject(Collider other) {
        other.transform.SetParent(transform);
        other.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(2000);
    }

    private void ThowObject(Collider other) {
        other.transform.SetParent(null);
        Rigidbody rb = other.GetComponent<Rigidbody>();
        
        if (other.gameObject.CompareTag("Throwable")) {
            rb.isKinematic = false;
            rb.velocity = device.velocity * throwforce;
            rb.angularVelocity = device.angularVelocity;
        }
        
    }

    private void SwipeLeft() {
        objectMenuManager.MenuLeft();
    }

    private void SwipeRight() {
        objectMenuManager.MenuRight();
    }

    private void SpawnObject() {
        objectMenuManager.SpawnCurrenObject();
    }

}
