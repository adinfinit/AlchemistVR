using System.Collections;
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

	Wall wall;

	public bool detached;

	// kind determines draining behavior
	public Kind kind;
	// layer at the wall grid; index from the center
	public int layer, index;

	// null | Joint that is in that port
	public Joint[] ports = new Joint[6];
	// list of all joints
	public Joint[] joints = new Joint[0];
	// valve objects only for ports 2, 3, 4
	public GameObject[] valve = new GameObject[6];

	void Start ()
	{
		
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
				joints [0] = CreateJoint (0, new byte[3]{ 2, 3, 4 });
				break;
			case Kind.Drain:
				joints [0] = CreateJoint (0, new byte[1]{ 0 });
				break;
			}

			foreach (Joint joint in joints) {
				foreach (byte port in joint.ports) {
					ports [port] = joint;
				}
			}
		}
	}

	public void GetDrains (out Tile left, out Tile  bottom, out Tile right)
	{
		left = wall.Get (layer + 1, index - 1 + (layer & 1));
		bottom = wall.Get (layer + 2, index);
		right = wall.Get (layer + 1, index + (layer & 1));
	}

	public void AddValve (byte sourcePort, Tile drain, byte drainPort)
	{
		Vector3 sourcePos = ports [sourcePort].GlobalPort (sourcePort);
		Vector3 drainPos = drain.ports [drainPort].GlobalPort (drainPort);

		GameObject obj = Geometry.CylinderBetweenPoints (sourcePos, drainPos, transform.lossyScale.x * Joint.Thickness);

		Vector3 scaled = obj.transform.localScale;
		scaled.Scale (new Vector3 (1.5f, 2f, 1.5f));
		obj.transform.localScale = scaled;

		obj.name = "Valve " + sourcePort; 
		obj.transform.parent = ports [sourcePort].transform;
		valve [sourcePort] = obj;
	}

	public void RemoveValve (byte sourcePort)
	{
		Destroy (valve [sourcePort]);
		valve [sourcePort] = null;
	}

	void Update ()
	{
		Vector3 position;
		wall.GetTilePosition (layer, index, out position);
		transform.localPosition = position;

		Vector3 center = wall.transform.position;
		center.y = transform.position.y;
		transform.LookAt (center);
	}

	public void Randomize ()
	{
		if (kind != Kind.Pipe) {
			return;
		}

		// testing code
		/*
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
