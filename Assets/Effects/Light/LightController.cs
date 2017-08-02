using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
	public float circleSpeed = 0.2f;
	public float circleSize = 7f;
	public float TimeOffset = 7f;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		float xPos = Mathf.Sin (Time.time * circleSpeed + TimeOffset) * circleSize;
		float yPos = Mathf.Cos (Time.time * circleSpeed + TimeOffset) * circleSize;

		this.transform.position = new Vector3 (xPos, this.transform.position.y, yPos);
	}
}
