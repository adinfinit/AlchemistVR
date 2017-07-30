using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
	public Lab lab;

	public World.Wall wall;
	public World.Tile tile;

	//TODO: probably need some list like this
	public GameObject[] joints;

	public float angleOffset = 0.0f;
	public float snapOffset = 0.0f;

	void Start ()
	{
		
	}

	void Update ()
	{
		// TODO: fix localRotation
		transform.localPosition = TargetPosition ();
		for (int i = 0; i < tile.joints.Length; i++) {
			SetJointColor (i, tile.joints [i].liquid.Color ());
		}
	}

	void SetJointColor (int joint, Color color)
	{
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		renderer.material.color = color;

		// TODO: color joints
	}

	public void Init (Lab lab, World.Wall wall, World.Tile tile)
	{
		this.lab = lab;
		this.wall = wall;
		this.tile = tile;

		transform.localScale = new Vector3 (lab.TileRadius, lab.TileRadius, lab.TileRadius);
		// TODO: fix localRotation
		transform.localRotation = Quaternion.Euler (90, Mathf.Rad2Deg * Angle () + 90f, 0);
		transform.localPosition = TargetPosition ();
	}

	float Angle ()
	{
		return tile.index * 2f * Mathf.PI / tile.layer.tiles.Length;
	}

	Vector3 TargetPosition ()
	{
		Vector3 position = new Vector3 ();

		int n = tile.layer.tiles.Length;

		float anglePerTile = 2f * Mathf.PI / (float)n;

		float fixedAngle = (float)tile.index * anglePerTile;
		float smoothAngle = fixedAngle + angleOffset;

		// int offsetIndex = (int)Mathf.Round (angleOffset / anglePerTile);
		// float snapAngle = (float)(tile.index + offsetIndex) * anglePerTile;

		float angle = smoothAngle + Mathf.PI * 0.25f;

		position.x = lab.WallRadius * Mathf.Cos (angle);
		position.z = -lab.WallRadius * Mathf.Sin (angle);

		float dy = Mathf.Cos (angle * tile.layer.tiles.Length * 0.5f) * 0.5f * 0.5f;
		position.y = -((float)tile.layer.index + dy) * lab.TileRadius * lab.YSpacing;

		return position;
	}
}

