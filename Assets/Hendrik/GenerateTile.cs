using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTile : MonoBehaviour
{
	GameObject[] spheres;
	GameObject[] cylinders;

	public GameObject[] joints;

	public World.Tile tile;

	public GameObject jointPrefab;

	// when two overlaping pipes are generated,
	// the middle of the pipe is this much above the middle
	float heightDifference = 0.6f;

	public void Generate (byte[] connections)
	{
		GameObject newJoint = Instantiate (jointPrefab);
		newJoint.GetComponent<GenerateJoint> ().Generate (connections, 0f);
		newJoint.transform.parent = this.transform;
		joints = new GameObject [1];
		joints [0] = newJoint;
	}

	public void Generate (byte[] connection1, byte[] connection2)
	{
		GameObject newJoint1 = Instantiate (jointPrefab);
		newJoint1.GetComponent<GenerateJoint> ().Generate (connection1, heightDifference);
		newJoint1.transform.parent = this.transform;

		joints = new GameObject [2];
		joints [0] = newJoint1;

		GameObject newJoint2 = Instantiate (jointPrefab);
		newJoint2.GetComponent<GenerateJoint> ().Generate (connection2, -heightDifference);
		newJoint2.transform.parent = this.transform;

		joints [1] = newJoint2;
	}
}
