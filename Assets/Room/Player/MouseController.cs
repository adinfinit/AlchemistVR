using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController: RayController
{
	void Start ()
	{
	}

	bool GetMouseRay (out Ray ray)
	{
		foreach (Camera camera in Object.FindObjectsOfType<Camera>()) {
			Vector3 point = camera.ScreenToViewportPoint (Input.mousePosition);
			if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) {
				ray = camera.ScreenPointToRay (Input.mousePosition);
				return true;
			}
		}
		ray = new Ray ();
		return false;
	}

	void Update ()
	{
		Ray ray;
		if (!GetMouseRay (out ray)) {
			return;
		}

		Point (ray);

		if (Input.GetMouseButtonDown (0)) {
			Select ();
		}
		if (Input.GetMouseButton (0)) {
			Drag ();
		} else {
			Release ();
		}
	}
}
