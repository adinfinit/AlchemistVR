using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generators : MonoBehaviour
{

	public GameObject TilePrefab;
	public GameObject PipePrefab;

	public GameObject newTileGameObject (World.Tile tile)
	{
		GameObject newTile = Instantiate (TilePrefab);

		if (tile.joints.Length == 1) {
			newTile.GetComponent<GenerateTile> ().Generate (tile.joints [0].ports);
			newTile.name = tile.ToStringWithPorts ();

			tile.visual = newTile;

		}

		newTile.GetComponent<GenerateTile>.Generate ();

	}
}
