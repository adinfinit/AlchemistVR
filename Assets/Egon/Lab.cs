using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : MonoBehaviour
{
	World.Wall wall;

	public int Layers = 5;
	public int Tiles = 24;

	public float WallRadius = 1.25f;
	public float TileRadius = 0.2f;
	public float YSpacing = 1.75f;

	void Start ()
	{
		CreateLevel ();
	}

	void Update ()
	{
		
	}

	void CreateLevel ()
	{
		wall = new World.Wall (Layers + 2, Tiles, 6, 6);
		wall.Randomize ();
		wall.RandomizeColors ();

		GameObject objects = new GameObject ();
		objects.transform.name = "Wall";
		objects.transform.parent = transform;

		float totalHeight = wall.rings.Length * TileRadius * YSpacing;
		objects.transform.position = new Vector3 (0f, totalHeight, 0f);

		foreach (World.Ring ring in wall.rings) {
			foreach (World.Tile tile in ring.tiles) {
				if (tile == null) {
					continue;
				}

				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere.transform.name = "Pipe_" + tile.layer + "_" + tile.index;
				sphere.transform.parent = objects.transform;

				Pipe pipe = sphere.AddComponent<Pipe> ();
				pipe.Init (this, wall, tile);

				MeshRenderer renderer = sphere.GetComponent<MeshRenderer> ();
				if (tile.joints.Length > 0) {
					renderer.material.color = tile.joints [0].liquid.Color ();
				}
			}
		}
	}
}
