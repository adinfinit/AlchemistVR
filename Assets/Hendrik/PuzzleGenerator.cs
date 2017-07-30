using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
	World.Wall puzzle;
	List<GameObject> tiles = new List<GameObject> ();

	public GameObject TilePrefab;

	// Use this for initialization
	void Start ()
	{
		puzzle = new World.Wall (null, 3, 5, 5, 5);
		World.Randomize.Wall (puzzle);

		for (int i = 1; i < puzzle.layers.Length; i++) {
			World.Layer layer = puzzle.layers [i];
			foreach (World.Tile tile in layer.tiles) {
				print (tile.joints);
				if (tile.joints.Length == 1) {
					GameObject newTile = (GameObject)Instantiate (TilePrefab);
					tiles.Add (newTile);

					GenerateTile genTile = newTile.GetComponent<GenerateTile> ();
					genTile.Generate (tile.joints [0].ports);

					TileData tileData = newTile.GetComponent<TileData> ();
					tileData.wall = puzzle;
					tileData.index = tile.index;
					tileData.layer = tile.layer.index;

				} else if (tile.joints.Length == 2) {
					GameObject newTile = (GameObject)Instantiate (TilePrefab);
					tiles.Add (newTile);

					GenerateTile genTile = newTile.GetComponent<GenerateTile> ();
					genTile.Generate (tile.joints [0].ports, tile.joints [1].ports);

					TileData tileData = newTile.GetComponent<TileData> ();
					tileData.wall = puzzle;
					tileData.index = tile.index;
					tileData.layer = tile.layer.index;

				}
			}
		}

		foreach (GameObject tile in tiles) {
			TileData tileData = tile.GetComponent<TileData> ();

			World.Tile tile2 = tileData.Tile ();
			float angle = 2f * Mathf.PI / (float)tile2.layer.tiles.Length;
			float y = (float)tile2.layer.index + (tile2.IsOffset () ? 0.5f : 0.0f);
			tile.transform.position = new Vector3 (angle * (5 - 2.236f), y * 4, 0);
			tile.transform.Rotate (90, 0, 0);
		}

	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}
