using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
	World.Wall puzzle;
	List<GameObject> tiles = new List<GameObject> ();

	float WallRadius = 10.25f;

	public GameObject TilePrefab;

	// Use this for initialization
	void Start ()
	{
		puzzle = new World.Wall (5, 10, 5, 5);
		puzzle.Randomize ();

		for (int i = 1; i < puzzle.rings.Length - 1; i++) {
			World.Ring ring = puzzle.rings [i];
			foreach (World.Tile tile in ring.tiles) {
				if (tile.joints.Length == 1) {
					GameObject newTile = (GameObject)Instantiate (TilePrefab);
					tiles.Add (newTile);

					GenerateTile genTile = newTile.GetComponent<GenerateTile> ();
					genTile.Generate (tile.joints [0].ports);

					TileData tileData = newTile.GetComponent<TileData> ();
					tileData.wall = puzzle;
					tileData.index = tile.index;
					tileData.layer = tile.layer;

				} else if (tile.joints.Length == 2) {
					GameObject newTile = (GameObject)Instantiate (TilePrefab);
					tiles.Add (newTile);

					GenerateTile genTile = newTile.GetComponent<GenerateTile> ();
					genTile.Generate (tile.joints [0].ports, tile.joints [1].ports);

					TileData tileData = newTile.GetComponent<TileData> ();
					tileData.wall = puzzle;
					tileData.index = tile.index;
					tileData.layer = tile.layer;

				}
			}
		}

		foreach (GameObject tile in tiles) {
			TileData tileData = tile.GetComponent<TileData> ();

			World.Tile tile2 = tileData.Tile ();
			float angle = tile2.Angle ();
			//tile.transform.position = new Vector3 (angle * (5 - 2.236f), tile2.Y () * 4, 0);
			tile.transform.Rotate (90, Mathf.Rad2Deg * angle + 90, 0);

			Vector3 position = new Vector3 ();
			position.x = Mathf.Cos (angle) * WallRadius;
			position.z = -Mathf.Sin (angle) * WallRadius;
			position.y = tile2.Y () * 4;

			tile.transform.position = position;
		}

	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}
