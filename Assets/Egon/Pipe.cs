using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
	public Lab lab;
	public World.Wall wall;
	public World.Tile tile;

	void Start ()
	{
		
	}

	void Update ()
	{
		
	}

	public void Init (Lab lab, World.Wall wall, World.Tile tile)
	{
		this.lab = lab;
		this.wall = wall;
		this.tile = tile;

		transform.localScale = new Vector3 (lab.TileRadius, lab.TileRadius, lab.TileRadius);
		transform.localPosition = TargetPosition ();
	}

	Vector3 TargetPosition ()
	{
		Vector3 position = new Vector3 ();
		float angle = tile.Angle () - Mathf.PI / 2f;
		position.x = lab.WallRadius * Mathf.Cos (angle);
		position.z = lab.WallRadius * Mathf.Sin (angle);
		position.y = -tile.Y () * lab.TileRadius * lab.YSpacing;
		return position;
	}
}

