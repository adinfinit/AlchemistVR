using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
	World.Wall puzzle;
	List<GameObject> tiles = new List<GameObject> ();
	List<GameObject> connPipes = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		puzzle = new World.Wall (null, 5, 10, 5, 5);
		World.Randomize.Wall (puzzle);

		Generators generators = GetComponent<Generators> ();

		for (int i = 0; i < puzzle.layers.Length; i++) {
			World.Layer layer = puzzle.layers [i];
			foreach (World.Tile tile in layer.tiles) {
				tiles.Add (generators.newTileGameObject (tile));
			}
		}

		foreach (World.Connection conn in puzzle.connections) {
			connPipes.Add (generators.newConnectionGameObject (conn));
		}

	}
}
