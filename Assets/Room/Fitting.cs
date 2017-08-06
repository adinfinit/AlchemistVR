﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitting : MonoBehaviour
{
	public Tile source;
	public byte sourcePort;

	public Tile drain;
	public byte drainPort;

	void Start ()
	{
		
	}

	public void Init (Tile source, byte sourcePort, Tile drain, byte drainPort)
	{
		this.source = source;
		this.sourcePort = sourcePort;
		this.drain = drain;
		this.drainPort = drainPort;

		Update ();
	}

	Vector3 sourcePos, drainPos;

	void Update ()
	{
		Vector3 nextSourcePos = source.ports [sourcePort].GlobalPort (sourcePort);
		Vector3 nextDrainPos = drain.ports [drainPort].GlobalPort (drainPort);

		if (Vectors.Equal (sourcePos, nextSourcePos) && Vectors.Equal (drainPos, nextDrainPos)) {
			return;
		}
		sourcePos = nextSourcePos;
		drainPos = nextDrainPos;

		MoveBetweenPoints (sourcePos, drainPos, Joint.Thickness);
	}

	void MoveBetweenPoints (Vector3 p1, Vector3 p2, float radius)
	{
		Vector3 offset = p2 - p1;
		Vector3 pos = p1 + (offset / 2);

		transform.position = pos;

		Vector3 scale = new Vector3 (radius, offset.magnitude, radius);
		transform.up = offset;
		transform.localScale = scale;
	}
}
