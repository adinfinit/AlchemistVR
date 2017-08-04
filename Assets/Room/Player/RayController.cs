using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{
	public Wall Wall;

	Ray StartRay;
	Ray CurrentRay;

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
		for (int index = 0; index < Wall.grid.GetLength (1); index++) {
			Tile tile = Wall.grid [layer, index];
			if (selectable (tile)) {
				Selection.Add (tile);
			}
		}

		foreach (Tile tile in Selection) {
			tile.Detach ();
		}

		return Selection.Count > 0;
	}

	public void Drag ()
	{
		Vector2 start = new Vector2 (StartRay.direction.x, StartRay.direction.z);
		Vector2 current = new Vector2 (CurrentRay.direction.x, CurrentRay.direction.z);

		float angularOffset = Vector2.SignedAngle (current, start) * Mathf.Deg2Rad;
		foreach (Tile tile in Selection) {
			tile.SetGrab (angularOffset, 0);
		}
	}

	public void Release ()
	{
		foreach (Tile tile in Selection) {
			tile.Attach ();
		}
	}
}
