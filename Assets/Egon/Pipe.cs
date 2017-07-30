using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
	public Lab lab;

	public World.Wall wall;
	public World.Tile tile;

	public float angleOffset = 0.0f;
	public float snapOffset = 0.0f;

	void Start ()
	{
		
	}

	void Update ()
	{
		// TODO: fix localRotation
		transform.localScale = new Vector3 (lab.TileRadius, lab.TileRadius, lab.TileRadius);
		float angle = TargetAngle ();
		transform.localRotation = Quaternion.Euler (0f, angle * Mathf.Rad2Deg, 0f);
		transform.localPosition = TargetPosition ();

		/*
		foreach (World.Joint joint in tile.joints) {
			GameObject obj = joint.gameObject;

			GenerateJoint genJoint = obj.GetComponent<GenerateJoint> ();
			//TODO: set color to obj

			foreach (GameObject sphere in genJoint.spheres) {
				MeshRenderer renderer = sphere.GetComponent<MeshRenderer> ();
				renderer.material.color = joint.liquid.Color ();
			}
			foreach (GameObject cylinder in genJoint.cylinders) {
				MeshRenderer renderer = cylinder.GetComponent<MeshRenderer> ();
				renderer.material.color = joint.liquid.Color ();
			}
		}
		*/
	}

	public void Init (Lab lab, World.Wall wall, World.Tile tile)
	{
		this.lab = lab;
		this.wall = wall;
		this.tile = tile;

		Update ();
	}

	public void SetOffset (float offset)
	{
		this.angleOffset = offset;
		Init (lab, wall, tile);
	}

	float FixedAngle ()
	{
		return Mathf.PI * 0.25f + tile.index * 2f * Mathf.PI / tile.layer.tiles.Length;
	}

	float TargetAngle ()
	{
		return FixedAngle () + angleOffset;
	}

	Vector3 TargetPosition ()
	{
		Vector3 position = new Vector3 ();
		float angle = TargetAngle ();

		position.z = -lab.WallRadius * Mathf.Cos (angle);
		position.x = -lab.WallRadius * Mathf.Sin (angle);

		float dy = Mathf.Cos (angle * tile.layer.tiles.Length * 0.5f) * 0.5f * 0.5f;
		position.y = -((float)tile.layer.index + dy) * lab.TileRadius * lab.YSpacing;

		return position;
	}
}

