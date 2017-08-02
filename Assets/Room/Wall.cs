using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
	public GameObject SourcePrefab;
	public GameObject PipePrefab;
	public GameObject DrainPrefab;

	[Header ("Grid Configuration")]
	public int LayerCount = 7;
	public int TileCount = 11;

	public int SourceCount = 5;
	public int DrainCount = 5;

	[Header ("Size")]
	public float Radius = 1.25f;
	public float TileSize = 0.4f;
	public float YSpacing = 1.0f;

	void Start ()
	{
		CreateGrid ();
		InvokeRepeating ("Drain", 0f, 0.3f);
	}

	void Drain ()
	{
		Draining.Drain (this);
	}

	void Update ()
	{
		
	}

	void CreateGrid ()
	{
		grid = new Tile[LayerCount, TileCount];
		for (int layer = 0; layer < LayerCount; layer++) {
			if (layer == 0) {
				int offset = -SourceCount / 2;
				for (int index = offset; index < offset + SourceCount; index++) {
					CreateTile (layer, index, Tile.Kind.Source);
				}
			} else if (layer == LayerCount - 1) {
				int offset = -DrainCount / 2;
				for (int index = offset; index < offset + DrainCount; index++) {
					CreateTile (layer, index, Tile.Kind.Drain);
				}
			} else {
				for (int index = 0; index < TileCount; index++) {
					CreateTile (layer, index, Tile.Kind.Pipe);
				}
			}
		}
	}

	void CreateTile (int layer, int index, Tile.Kind kind)
	{
		GameObject tileObject = null;

		switch (kind) {
		case Tile.Kind.Source:
			tileObject = Instantiate (SourcePrefab);
			break;
		case Tile.Kind.Pipe:
			tileObject = Instantiate (PipePrefab);
			break;
		case Tile.Kind.Drain:
			tileObject = Instantiate (DrainPrefab);
			break;
		default:
			Debug.LogAssertion ("Tile.Kind unhandled: " + kind.ToString ());
			break;
		}

		Tile tile = tileObject.GetComponent<Tile> ();
		Debug.Assert (tile != null);

		tileObject.name = "Tile";
		tileObject.transform.parent = transform;
		tileObject.transform.localScale = Vector3.one * TileSize;

		tile.Init (this, layer, index);
		tile.Randomize ();

		Set (layer, index, tile);
	}

	#region Grid

	public Tile[,] grid;

	public Tile Get (int layer, int index)
	{
		if (layer < 0 || LayerCount <= layer) {
			return null;
		}
		index = (index + TileCount) % TileCount;
		return grid [layer, index];
	}

	public void Set (int layer, int index, Tile tile)
	{
		Debug.Assert (0 <= layer && layer < LayerCount);
		index = (index + TileCount) % TileCount;
		grid [layer, index] = tile;
	}

	#endregion

	#region TilePositioning

	public float LayerHeight (int y)
	{
		return ((float)(LayerCount - y - 1) + 0.5f) * 0.5f * YSpacing;
	}

	public void GetAngularPosition (int layer, int index, out float angle, out float y)
	{
		y = LayerHeight (layer);
		float p = (float)(2 * index + (layer & 1)) / (float)(2 * TileCount);
		angle = p * 2f * Mathf.PI;
	}

	public void GetTilePosition (int layer, int index, out Vector3 localPosition)
	{
		float angle, y;
		GetAngularPosition (layer, index, out angle, out y);

		localPosition = new Vector3 ();
		localPosition.z = -Radius * Mathf.Cos (angle);
		localPosition.x = -Radius * Mathf.Sin (angle);
		localPosition.y = y;
	}

	public void GetTileWorldPosition (int layer, int index, out Vector3 position, out Vector3 normal)
	{
		GetTilePosition (layer, index, out position);
		position = (Vector3)(transform.localToWorldMatrix * new Vector4 (position.x, position.y, position.z, 1));
		normal = (position - transform.position).normalized;
		normal.y = 0f;
	}

	#endregion

	#region DebugInfo

	void OnDrawGizmos ()
	{
		UnityEditor.Handles.DrawLine (transform.position, transform.position + transform.up * LayerHeight (0));

		Vector3 pos, norm;

		for (int layer = 0; layer < LayerCount; layer++) {
			if (layer == 0) {
				UnityEditor.Handles.color = Color.white;
				int offset = -SourceCount / 2;
				for (int index = offset; index < offset + SourceCount; index++) {
					GetTileWorldPosition (layer, index, out pos, out norm);
					UnityEditor.Handles.DrawWireDisc (pos, norm, TileSize * 0.5f);
				}
			} else if (layer == LayerCount - 1) {
				UnityEditor.Handles.color = Color.white;
				int offset = -DrainCount / 2;
				for (int index = offset; index < offset + DrainCount; index++) {
					GetTileWorldPosition (layer, index, out pos, out norm);
					UnityEditor.Handles.DrawWireDisc (pos, norm, TileSize * 0.5f);
				}
			} else {
				UnityEditor.Handles.color = Color.yellow;
				for (int index = 0; index < TileCount; index++) {
					GetTileWorldPosition (layer, index, out pos, out norm);
					UnityEditor.Handles.DrawWireDisc (pos, norm, TileSize * 0.5f);
				}
			}
			UnityEditor.Handles.DrawWireDisc (transform.position + transform.up * LayerHeight (layer), transform.up, Radius);
		}
	}

	#endregion
}
