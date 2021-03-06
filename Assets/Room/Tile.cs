﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public enum Kind
	{
		Source,
		Pipe,
		Drain
	}

	public Wall wall;

	public bool attached = true;

	// kind determines draining behavior
	public Kind kind;
	// layer at the wall grid; index from the center
	public int layer, index;

	public GameObject Potion;
	Material LiquidMaterial;

	// null | Joint that is in that port
	public Joint[] ports = new Joint[6];
	// list of all joints
	public Joint[] joints = new Joint[0];
	// valve objects only for ports 2, 3, 4
	public GameObject[] fittings = new GameObject[6];

	void Start ()
	{
		if (Potion != null) {
			MeshRenderer renderer = Potion.GetComponent<MeshRenderer> ();
			for (int i = 0; i < renderer.materials.Length; i++) {
				Material mat = renderer.materials [i];
				if (mat.name == "HealthLiquid (Instance)") {
					LiquidMaterial = mat;
					break;
				}
			}
		}
	}

	public void Init (Wall wall, int layer, int index)
	{
		this.wall = wall;
		this.layer = layer;
		this.index = index;

		this.Update ();

		if (kind != Kind.Pipe) {
			joints = new Joint[1];
			switch (kind) {
			case Kind.Source:
				joints [0] = CreateJoint (0, new byte[4]{ 0, 2, 3, 4 });
				break;
			case Kind.Drain:
				joints [0] = CreateJoint (0, new byte[3]{ 0, 1, 5 });
				break;
			}

			foreach (Joint joint in joints) {
				foreach (byte port in joint.ports) {
					ports [port] = joint;
				}
			}
		}
	}

	public void GetSources (out Tile left, out Tile top, out Tile right)
	{
		left = wall.Get (layer - 1, index - 1 + (layer & 1));
		top = wall.Get (layer - 2, index);
		right = wall.Get (layer - 1, index + (layer & 1));
	}

	public void GetDrains (out Tile left, out Tile  bottom, out Tile right)
	{
		left = wall.Get (layer + 1, index - 1 + (layer & 1));
		bottom = wall.Get (layer + 2, index);
		right = wall.Get (layer + 1, index + (layer & 1));
	}

	public void AddFitting (byte sourcePort, Tile drain, byte drainPort)
	{
		GameObject fittingObj = Instantiate (wall.FittingPrefab);
		fittingObj.name = "Fitting " + sourcePort; 
		fittings [sourcePort] = fittingObj;

		Fitting fitting = fittingObj.GetComponent<Fitting> ();
		fittingObj.transform.parent = ports [sourcePort].transform;
		fitting.Init (this, sourcePort, drain, drainPort);
	}

	public void RemoveFitting (byte sourcePort)
	{
		if (fittings [sourcePort] == null) {
			return;
		}
		Destroy (fittings [sourcePort]);
		fittings [sourcePort] = null;
	}

	public void Attach ()
	{
		attached = true;
		SetGrab (0f, 0);
	}

	float angularOffset = 0.0f;
	int controllerIndex = 0;

	public void SetGrab (float angularOffset, int controllerIndex)
	{
		this.angularOffset = angularOffset;
		this.controllerIndex = controllerIndex;
	}

	public void Detach ()
	{
		attached = false;

		Tile left, top, right;
		GetSources (out left, out top, out right);
		if (left != null) {
			left.RemoveFitting (2);
		}
		if (top != null) {
			top.RemoveFitting (3);
		}
		if (right != null) {
			right.RemoveFitting (4);
		}

		RemoveFitting (4);
		RemoveFitting (3);
		RemoveFitting (2);
	}

	void Update ()
	{
		if (wall == null) {
			return;
		}


		Vector3 position;

		if (attached) {
			wall.GetTilePosition (layer, index, out position);
		} else {
			wall.GetDetachedPosition (layer, index, angularOffset, controllerIndex, out position);
		}

		// lerp without collapsing to center
		if (!Vectors.Equals (transform.localPosition, position)) {
			position = Vector3.Lerp (transform.localPosition, position, 0.2f);
			float y = position.y;
			position.y = 0.0f;

			float magnitude = position.magnitude;
			if (magnitude < wall.Radius * 0.7f) {
				position = position * wall.Radius * 0.7f / magnitude;
			}

			position.y = y;
			transform.localPosition = position;

			Vector3 center = wall.transform.position;
			center.y = transform.position.y;
			transform.LookAt (center);
		}
	
		if (LiquidMaterial != null) {
			if (Colors.NeedUpdate (ref firstJointColor, joints [0].liquid.Color ())) {
				LiquidMaterial.color = firstJointColor;
			}
		}
	}

	Color firstJointColor = new Color (0.5f, 0.5f, 0.5f);

	public void Randomize ()
	{
		if (kind != Kind.Pipe) {
			return;
		}

		/* testing code
		if (false) {
			joints = new Joint[1];
			Joint joint = CreateJoint (0, new byte[6]{ 0, 1, 2, 3, 4, 5 }); 
			joints [0] = joint;
			foreach (byte port in joint.ports) {
				ports [port] = joint;
			}
			return;
		}
		*/


		int jointCount = Random.Range (1, 3);

		List<byte> availablePorts = new List<byte> ();
		for (byte k = 0; k < 6; k++) {
			availablePorts.Add (k);
		}

		joints = new Joint[jointCount];
		List<Joint> jointlist = new List<Joint> ();

		for (int i = 0; i < jointCount; i++) {
			int n = Mathf.Min (RandomPortCount (), availablePorts.Count);	
			if (n <= 0) {
				continue;
			}

			byte[] portset = new byte[n];
			for (int k = 0; k < n; k++) {
				int p = Random.Range (0, availablePorts.Count);
				portset [k] = availablePorts [p];
				availablePorts.RemoveAt (p);
			}

			Joint joint = CreateJoint (jointlist.Count, portset);
			foreach (byte port in joint.ports) {
				ports [port] = joint;
			}
			joint.transform.SetParent (transform, false);
			jointlist.Add (joint);
		}

		joints = jointlist.ToArray ();
	}

	Joint CreateJoint (int index, byte[] ports)
	{
		GameObject jointObject = new GameObject ();
		jointObject.name = "Joint";
		Joint joint = jointObject.AddComponent<Joint> ();
		joint.Init (this, index, ports);
		joint.transform.SetParent (transform, false);
		return joint;
	}

	int RandomPortCount ()
	{
		// pick random ports from available
		int n = 0;
		float rand = Random.Range (0, 10f);
		if (rand < 1.5f) {
			n = 1;
		} else if (rand < 8f) {
			n = 2;
		} else if (rand > 8f) {
			n = 3;
		} 
		return n;
	}
}
