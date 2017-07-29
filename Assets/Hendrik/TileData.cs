using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
	public World.Wall wall;
	public int layer, index;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*
		Tile tile = wall.rings [layer].tiles [index];
		if (tile != lastTile) {
			// regenerate
		}*/
	}

	public World.Tile Tile ()
	{
		return wall.layers [layer].tiles [index];
	}

	/*
	void OnDrawGizmos ()
	{
		Gizmos.DrawSphere (transform.position, 2);
	}*/
}
