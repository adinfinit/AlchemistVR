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
		puzzle = new World.Wall (3, 5, 5, 5);
		puzzle.Randomize ();

		for (int i = 1; i < puzzle.rings.Length; i++) {
			World.Ring ring = puzzle.rings [i];
			foreach (World.Tile tile in ring.tiles) {
				print (tile.joints);
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
			tile.transform.position = new Vector3 (tile2.Angle () * (5 - 2.236f), tile2.Y () * 4, 0);
			tile.transform.Rotate (90, 0, 0);
		}

	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}
