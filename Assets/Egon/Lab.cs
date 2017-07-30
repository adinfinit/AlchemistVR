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

	void Start ()
	{
		Container = new GameObject ();
		Container.transform.name = "Wall";
		Container.transform.parent = transform;

		float totalHeight = (Layers + 2) * TileRadius * YSpacing;
		Container.transform.position = new Vector3 (0f, totalHeight, 0f);

		Wall = new World.Wall (this, Layers + 2, Tiles, 6, 6);
		World.Randomize.Wall (Wall);
	}


	Ray SelectionStartRay;
	Ray SelectionCurrentRay;
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
			foreach (World.Tile tile in target.tile.layer.tiles) {
				Selection.Add (tile);
			}
			Wall.DisconnectTiles (Selection);
		}
		return Selection.Count > 0;
	}

	void SelectUpdate (Ray ray)
	{
		SelectionCurrentRay = ray;

		Vector2 start = new Vector2 (SelectionStartRay.direction.x, SelectionStartRay.direction.z);
		Vector2 current = new Vector2 (SelectionCurrentRay.direction.x, SelectionCurrentRay.direction.z);

		float rotation = Vector2.SignedAngle (current, start) * Mathf.Deg2Rad;
		foreach (World.Tile tile in Selection) {
			Pipe pipe = (Pipe)tile.visual;
			pipe.SetOffset (rotation);
		}
	}

	void SelectFinish ()
	{
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
		ViveRightDown = true;
		Select (new Ray (ViveRight.transform.position, ViveRight.transform.forward));
	}

	public void HandleControllerUnpressRight ()
	{
		ViveRightDown = false;
		SelectFinish ();
	}

	#endregion

	#region Callbacks

	public void TileCreated (World.Tile tile)
	{
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.name = tile.ToString ();
		sphere.transform.parent = Container.transform;

		sphere.AddComponent<SphereCollider> ();

		Pipe pipe = sphere.AddComponent<Pipe> ();
		tile.visual = pipe;
		pipe.Init (this, Wall, tile);
	}

	public void TileChanged (World.Tile tile)
	{
		
	}

	public void TileDestroyed (World.Tile tile)
	{
		Destroy (tile.visual);
		tile.visual = null;
	}

	public void ConnectionCreated (World.Connection conn)
	{
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.name = conn.ToString ();
		sphere.transform.parent = Container.transform;

		Pipe sourcePipe = (Pipe)conn.source.tile.visual;
		Vector3 source = sourcePipe.transform.position;
		Pipe drainPipe = (Pipe)(conn.drain.tile.visual); 
		Vector3 drain = drainPipe.transform.position;

		sphere.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		sphere.transform.position = (source + drain) * 0.5f;

		conn.visual = sphere;
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
