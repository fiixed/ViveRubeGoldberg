using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private float throwforce= 1.5f;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Throwable")) {
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
        rb.isKinematic = false;
        rb.velocity = device.velocity * throwforce;
        rb.angularVelocity = device.angularVelocity;
    }
}
