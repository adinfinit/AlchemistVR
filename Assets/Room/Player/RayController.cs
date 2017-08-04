using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{
	public Wall Wall;

	Ray StartRay;
	Ray CurrentRay;

	int selectedLayer = 0;
	List<Tile> Selection = new List<Tile> ();

	public void Point (Ray ray)
	{
		CurrentRay = ray;
	}

	bool selectable (Tile tile)
	{
		return tile != null &&
		tile.attached &&
		tile.kind == Tile.Kind.Pipe &&
		tile.transform.IsChildOf (Wall.transform);
	}

	public bool Select ()
	{
		StartRay = CurrentRay;
		Selection.Clear ();

		Tile target;
		foreach (RaycastHit hit in Physics.RaycastAll(StartRay)) {
			target = hit.collider.GetComponent<Tile> ();
			if (selectable (target)) {
				SelectLayer (target.layer);
				return Selection.Count > 0;
			}
		}

		return false;
	}

	public bool SelectLayer (int layer)
	{
		selectedLayer = layer;
		for (int index = 0; index < Wall.TileCount; index++) {
			Tile tile = Wall.grid [selectedLayer, index];
			if (selectable (tile)) {
				Selection.Add (tile);
			}
		}

		foreach (Tile tile in Selection) {
			tile.Detach ();
		}

		return Selection.Count > 0;
	}

	float GetAngularOffset ()
	{
		Vector2 start = new Vector2 (StartRay.direction.x, StartRay.direction.z);
		Vector2 current = new Vector2 (CurrentRay.direction.x, CurrentRay.direction.z);

		return Vector2.SignedAngle (current, start) * Mathf.Deg2Rad * 2f;
	}

	public void Drag ()
	{
		float angularOffset = GetAngularOffset ();
		foreach (Tile tile in Selection) {
			tile.SetGrab (angularOffset, 0);
		}
	}

	public void Release ()
	{
		if (Selection.Count == 0) {
			return;
		}

		float angularOffset = GetAngularOffset ();
		int angularDelta = (int)Mathf.Round (Wall.TileCount + angularOffset * Wall.TileCount / (Mathf.PI * 2));

		Tile[] tiles = new Tile[Wall.TileCount];
		for (int index = 0; index < Wall.TileCount; index++) {
			tiles [index] = Wall.grid [selectedLayer, index];
		}

		for (int index = 0; index < Wall.TileCount; index++) {
			Tile tile = Selection [index];
			int targetIndex = (index + angularDelta) % Wall.TileCount;
			Wall.grid [selectedLayer, targetIndex] = tiles [index];
			if (tile != null) {
				tile.index = targetIndex;
			}
		}

		foreach (Tile tile in Selection) {
			tile.Attach ();
		}

		Selection.Clear ();
	}
}
