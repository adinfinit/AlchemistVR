using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : MonoBehaviour
{
	World.Wall Wall;
	GameObject Container;

	public int Layers = 5;
	public int Tiles = 24;

	public float WallRadius = 1.25f;
	public float TileRadius = 0.2f;
	public float YSpacing = 1.75f;

	Generators generators;

	void Start ()
	{
		generators = GetComponent<Generators> ();
		Container = new GameObject ();
		Container.transform.name = "Wall";
		Container.transform.parent = transform;

		float totalHeight = (Layers + 2) * TileRadius * YSpacing;
		Container.transform.position = new Vector3 (0f, totalHeight, 0f);

		Wall = new World.Wall (this, Layers + 2, Tiles, 6, 6);
		World.Randomize.Wall (Wall);

		InvokeRepeating ("CallDrain", 1f, 0.25f);
	}

	void CallDrain ()
	{
		Wall.Drain ();
	}

	Ray SelectionStartRay;
	Ray SelectionCurrentRay;
	World.Layer SelectedLayer;
	float SelectionOffset;
	List<World.Tile> Selection = new List<World.Tile> ();

	bool Select (Ray ray)
	{
		Selection.Clear ();
		SelectionStartRay = ray;
		SelectionCurrentRay = ray;

		Pipe target = null;
		foreach (RaycastHit hit in Physics.RaycastAll(SelectionStartRay)) {
			target = hit.collider.GetComponent<Pipe> ();
			if (target != null) {
				break;
			}
		}

		if (target != null) {
			SelectedLayer = target.tile.layer;
			if (SelectedLayer.locked) {
				SelectedLayer = null;
				return false;
			}

			foreach (World.Tile tile in SelectedLayer.tiles) {
				Selection.Add (tile);
			}
			Wall.DisconnectTiles (Selection);
		}
		return Selection.Count > 0;
	}

	void SelectUpdate (Ray ray)
	{
		if (SelectedLayer == null) {
			return;
		}

		SelectionCurrentRay = ray;

		Vector2 start = new Vector2 (SelectionStartRay.direction.x, SelectionStartRay.direction.z);
		Vector2 current = new Vector2 (SelectionCurrentRay.direction.x, SelectionCurrentRay.direction.z);

		SelectionOffset = Vector2.SignedAngle (current, start) * Mathf.Deg2Rad;
		foreach (World.Tile tile in Selection) {
			Pipe pipe = (Pipe)tile.visual;
			pipe.SetOffset (SelectionOffset);
		}
	}

	void SelectFinish ()
	{
		if (SelectedLayer == null) {
			return;
		}

		float anglePerTile = 2f * Mathf.PI / (float)Wall.layers [0].tiles.Length;
		int offsetIndex = (int)Mathf.Round (SelectionOffset / anglePerTile);

		if (offsetIndex != 0) {
			SelectedLayer.Rotate (offsetIndex);
		}
		foreach (World.Tile tile in Selection) {
			Pipe pipe = (Pipe)tile.visual;
			pipe.SetOffset (0f);
		}

		Wall.ConnectTiles (Selection);
		Selection.Clear ();
	}

	void Update ()
	{
		UpdateMouseController ();
		UpdateVive ();
	}

	#region Mouse

	void UpdateMouseController ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Select (Camera.main.ScreenPointToRay (Input.mousePosition));
		}
		if (Input.GetMouseButton (0)) {
			SelectUpdate (Camera.main.ScreenPointToRay (Input.mousePosition));
		}
		if (Input.GetMouseButtonUp (0)) {
			SelectFinish ();
		}
	}

	#endregion

	#region Vive

	public bool ViveTriedConnect = false;
	public GameObject ViveLeft = null;
	public GameObject ViveRight = null;

	public bool ViveLeftDown = false;
	public bool ViveRightDown = false;

	void UpdateVive ()
	{
		if (!ViveTriedConnect) {
			ViveTriedConnect = true;
			try {
				ViveLeft = GameObject.FindGameObjectWithTag ("LeftController");
				ViveRight = GameObject.FindGameObjectWithTag ("RightController");
			} catch {
			}
		}

		if (ViveLeft == null && ViveRight == null) {
			return;
		}

		if (ViveRightDown) {
			SelectUpdate (new Ray (ViveRight.transform.position, ViveRight.transform.forward));
		}
		if (ViveLeftDown) {
			SelectUpdate (new Ray (ViveRight.transform.position, ViveRight.transform.forward));
		}
	}

	public void HandleControllerPressRight ()
	{
		if (ViveRight == null) {
			return;
		}

		ViveRightDown = true;
		Select (new Ray (ViveRight.transform.position, ViveRight.transform.forward));
	}

	public void HandleControllerUnpressRight ()
	{
		if (ViveRight == null) {
			return;
		}

		ViveRightDown = false;
		SelectFinish ();
	}

	#endregion

	#region Callbacks

	public void TileCreated (World.Tile tile)
	{
		TileDestroyed (tile); // ensure that there are no left overs

		GameObject tileObject = Generators.CreateTile (tile);
		tileObject.transform.parent = Container.transform;
		tileObject.AddComponent<SphereCollider> ();

		Pipe pipe = tileObject.AddComponent<Pipe> ();
		pipe.Init (this, Wall, tile);
		tile.visual = pipe;
	}

	public void TileChanged (World.Tile tile)
	{
		TileCreated (tile);
	}

	public void TileDestroyed (World.Tile tile)
	{
		if (tile.visual == null) {
			return;
		}

		Pipe pipe = (Pipe)tile.visual;
		Destroy (pipe.gameObject);
		tile.visual = null;
	}

	public void ConnectionCreated (World.Connection conn)
	{
		GameObject connObj = Generators.CreateConnection (conn);
		connObj.transform.parent = Container.transform;
		conn.visual = connObj;
	}

	public void JointChanged (World.Joint joint)
	{
		
	}

	public void ConnectionDestroyed (World.Connection conn)
	{
		if (conn.visual == null) {
			return;
		}

		Destroy (conn.visual);
		conn.visual = null;
	}

	#endregion
}
