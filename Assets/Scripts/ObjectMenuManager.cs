﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {
    public List<GameObject> objectList;
    public List<GameObject> objectPrefabList;
    public List<GameObject> objectMenuUI;
    public int currentObject = 0;

	// Use this for initialization
	void Start () {
        //foreach (Transform child in transform) {
        //    objectList.Add(child.gameObject);
        //}
        objectList[currentObject].transform.rotation = Quaternion.identity;
	}

    private void LateUpdate() {
        if (objectMenuUI[currentObject].gameObject.name == "Plank") {
            objectList[currentObject].transform.rotation = Quaternion.identity;
        } else if (objectMenuUI[currentObject].gameObject.name == "Goal" ||
            objectMenuUI[currentObject].gameObject.name == "Trampoline") {
            objectList[currentObject].transform.rotation = Quaternion.Euler(-90, 0, 0);
        } else if (objectMenuUI[currentObject].gameObject.name == "Funnel") {
            objectList[currentObject].transform.rotation = Quaternion.Euler(90, 0, 0);
        }

    }

    public void MenuLeft() {
        objectMenuUI[currentObject].SetActive(false);
        currentObject--;
        if (currentObject < 0) {
            currentObject = objectList.Count - 1;
        }
        objectMenuUI[currentObject].SetActive(true);
    }

    public void MenuRight() {
        objectMenuUI[currentObject].SetActive(false);
        currentObject++;
        if (currentObject > objectList.Count - 1) {
            currentObject = 0;
        }
        objectMenuUI[currentObject].SetActive(true);
    }

    public void SpawnCurrenObject() {
        Instantiate(objectPrefabList[currentObject], objectList[currentObject].transform.position, objectList[currentObject].transform.rotation);
    }
}
