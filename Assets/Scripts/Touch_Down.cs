using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch_Down : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnPointerDown(PointerEventData pointerEventData)
	{
		Spawner.stop_rotating_i_touched = true;
	}

	public void OnPointerUp(PointerEventData pointerEventData)
	{
		Spawner.stop_rotating_i_touched = false;
	}	

}