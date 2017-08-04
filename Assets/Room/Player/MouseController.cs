using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController: RayController
{
	void Start ()
	{
	}

	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Point (ray);

		if (Input.GetMouseButtonDown (0)) {
			Select ();
		}
		if (Input.GetMouseButton (0)) {
			Drag ();
		}
		if (Input.GetMouseButtonUp (0)) {
			Release ();
		}
	}
}
