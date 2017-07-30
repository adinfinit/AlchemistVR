using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : MonoBehaviour
{
	World.Wall wall;

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
	List<Pipe> selection = new List<Pipe> ();

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

					selection.Add (tile.visual);
				}
			}
		}

		if (Input.GetMouseButton (0)) { // press
			Ray currentRay = Camera.main.ScreenPointToRay (Input.mousePosition);

			Vector2 start = new Vector2 (selectionStartRay.direction.x, selectionStartRay.direction.z);
			Vector2 current = new Vector2 (currentRay.direction.x, currentRay.direction.z);
			 
			float rotation = Vector2.SignedAngle (current, start) * Mathf.Deg2Rad;
			foreach (Pipe pipe in selection) {
				pipe.angleOffset = rotation;
			}
		}

		if (Input.GetMouseButtonUp (0)) { // release
			foreach (Pipe pipe in selection) {
				pipe.angleOffset = 0f;
			}
			selection.Clear ();
		}
	}

	void CreateLevel ()
	{
		wall = new World.Wall (Layers + 2, Tiles, 6, 6);
		World.Randomize.Wall (wall);

		GameObject container = new GameObject ();
		container.transform.name = "Wall";
		container.transform.parent = transform;

		float totalHeight = wall.layers.Length * TileRadius * YSpacing;
		container.transform.position = new Vector3 (0f, totalHeight, 0f);

		foreach (World.Layer ring in wall.layers) {
			foreach (World.Tile tile in ring.tiles) {
				if (tile == null) {
					continue;
				}

				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere.transform.name = tile.ToString ();
				sphere.transform.parent = container.transform;

				sphere.AddComponent<SphereCollider> ();

				Pipe pipe = sphere.AddComponent<Pipe> ();
				tile.visual = pipe;
				pipe.Init (this, wall, tile);

				MeshRenderer renderer = sphere.GetComponent<MeshRenderer> ();
				if (tile.joints.Length > 0) {
					renderer.material.color = tile.joints [0].liquid.Color ();
				}
			}
		}

		foreach (World.Connection conn in wall.connections) {
			GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphere.transform.name = conn.ToString ();
			sphere.transform.parent = container.transform;

			Vector3 source = conn.source.tile.visual.transform.position;
			Vector3 drain = conn.drain.tile.visual.transform.position;

			sphere.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
			sphere.transform.position = (source + drain) * 0.5f;
		}
	}

	public void TileCreated (World.Tile tile)
	{

	}

	public void TileDestroyed (World.Tile tile)
	{

	}

	public void ConnectionCreated (World.Connection conn)
	{

	}

	public void ConnectionDestroyed (World.Connection conn)
	{

	}
}
