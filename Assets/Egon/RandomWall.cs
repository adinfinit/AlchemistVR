using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWall : MonoBehaviour
{
	public World.Wall wall;

	public int Layers = 5;
	public int Tiles = 24;
	public float WallRadius = 1.25f;
	public float TileRadius = 0.5f;
	public float YSpacing = 1.5f;

	// Use this for initialization
	void Start ()
	{
		wall = new World.Wall (Layers + 2, Tiles, 6, 6);
		wall.Randomize ();
		wall.RandomizeColors ();

		float totalHeight = wall.rings.Length * TileRadius * YSpacing;
		foreach (World.Ring ring in wall.rings) {
			foreach (World.Tile tile in ring.tiles) {
				if (tile == null) {
					continue;
				}

				Vector3 position = new Vector3 ();
				float angle = tile.Angle () - Mathf.PI / 2f;
				position.x = WallRadius * Mathf.Cos (angle);
				position.z = WallRadius * Mathf.Sin (angle);
				position.y = totalHeight - tile.Y () * TileRadius * YSpacing;

				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere.transform.name = "Tile_" + tile.layer + "_" + tile.index;
				sphere.transform.localScale = new Vector3 (TileRadius, TileRadius, TileRadius);
				sphere.transform.position = position;
				sphere.transform.parent = transform;

				MeshRenderer renderer = sphere.GetComponent<MeshRenderer> ();
				if (tile.joints.Length > 0) {
					renderer.material.color = tile.joints [0].liquid.Color ();
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
