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
		CreateLevel ();
	}

	bool inputWasDown = false;
	bool inputIsDown = false;

	Ray selectionStartRay;
	List<World.Tile> selection = new List<World.Tile> ();

	void Update ()
	{
		inputWasDown = inputIsDown;
		inputIsDown = Input.GetMouseButton (0);

		if (Input.GetMouseButtonDown (0)) { // down
			selection.Clear ();

			Pipe targetPipe = null;
			selectionStartRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			foreach (RaycastHit hit in Physics.RaycastAll(selectionStartRay)) {
				targetPipe = hit.collider.GetComponent<Pipe> ();
				if (targetPipe != null) {
					break;
				}
			}

			if (targetPipe != null) {
				foreach (World.Tile tile in targetPipe.tile.layer.tiles) {
					if (tile == null) {
						continue;
					}
					selection.Add (tile);
				}
				Wall.DisconnectTiles (selection);
			}
		}

		if (Input.GetMouseButton (0)) { // press
			Ray currentRay = Camera.main.ScreenPointToRay (Input.mousePosition);

			Vector2 start = new Vector2 (selectionStartRay.direction.x, selectionStartRay.direction.z);
			Vector2 current = new Vector2 (currentRay.direction.x, currentRay.direction.z);
			 
			float rotation = Vector2.SignedAngle (current, start) * Mathf.Deg2Rad;
			foreach (World.Tile tile in selection) {
				Pipe pipe = (Pipe)tile.visual;
				pipe.SetOffset (rotation);
			}
		}

		if (Input.GetMouseButtonUp (0)) { // release
			foreach (World.Tile tile in selection) {
				Pipe pipe = (Pipe)tile.visual;
				pipe.SetOffset (0f);
			}

			Wall.ConnectTiles (selection);
			selection.Clear ();
		}
	}

	void CreateLevel ()
	{
		Container = new GameObject ();
		Container.transform.name = "Wall";
		Container.transform.parent = transform;

		float totalHeight = (Layers + 2) * TileRadius * YSpacing;
		Container.transform.position = new Vector3 (0f, totalHeight, 0f);

		Wall = new World.Wall (this, Layers + 2, Tiles, 6, 6);
		World.Randomize.Wall (Wall);
	}

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
}
