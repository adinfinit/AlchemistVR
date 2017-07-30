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

		//GameObject indicator = transform.Find ("Cube").gameObject;
		//if (tile.joints.Length > 0) {
		//	indicator.GetComponent<MeshRenderer> ().material.color = tile.joints [0].nextLiquid.Color ();
		//}

		foreach (World.Joint joint in tile.joints) {
			GameObject obj = joint.gameObject;

			Color target = joint.liquid.Color ();
			foreach (MeshRenderer mr in obj.GetComponentsInChildren<MeshRenderer>()) {
				mr.material.color = Color.Lerp (mr.material.color, target, 0.1f);
			}
		}

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

