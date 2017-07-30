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

    public GameObject controllerLeft;
    public GameObject controllerRight;

    private bool leftControllerDragging;
    private bool rightControllerDragging;

    


    Ray selectionStartRay;
    List<Pipe> selection = new List<Pipe>();


    void Start()
    {
        CreateLevel();
    }


    static public GameObject getChildGameObjectTag(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren <Transform>();
        foreach (Transform t in ts) if (t.gameObject.tag == withName) return t.gameObject;
        return null;
    }

    public void HandleControllerPressRight()
    {
        selection.Clear();
        Debug.Log("Handling right controller press in LAB");

        Pipe targetPipe = null;
        selectionStartRay = new Ray(controllerRight.transform.position, controllerRight.transform.forward);
        foreach (RaycastHit hit in Physics.RaycastAll(selectionStartRay))
        {
            targetPipe = hit.collider.GetComponent<Pipe>();
            if (targetPipe != null)
            {
                break;
            }
        }

        if (targetPipe != null)
        {
            rightControllerDragging = true;
            Wall.DisconnectLayer(targetPipe.tile.layer);
            foreach (World.Tile tile in targetPipe.tile.layer.tiles)
            {
                if (tile == null) {
                    continue;
                }

                selection.Add((Pipe)tile.visual);
            }
        }
    }

    public void HandleControllerUnpressRight()
    {
        Debug.Log("Handling right controller unpress in LAB");
        if (Input.GetMouseButtonUp(0))
        { // release
            foreach (Pipe pipe in selection)
            {
                pipe.angleOffset = 0f;
            }
            selection.Clear();
            rightControllerDragging = false;
        }
    }
	void Update ()
	{
        if (controllerLeft == null)
        { controllerLeft = GameObject.FindGameObjectWithTag("LeftController"); }
        
        if (controllerRight == null)
        { controllerRight = GameObject.FindGameObjectWithTag("RightController"); }

		if (rightControllerDragging) { // press
            Ray currentRay = new Ray(controllerRight.transform.position, controllerRight.transform.forward);

            Vector2 start = new Vector2(selectionStartRay.direction.x, selectionStartRay.direction.z);
            Vector2 current = new Vector2(currentRay.direction.x, currentRay.direction.z);

            float rotation = Vector2.SignedAngle(current, start) * Mathf.Deg2Rad;
            foreach (Pipe pipe in selection)
            {
                pipe.angleOffset = rotation;
            }
        }

        if (leftControllerDragging)
        { // press
            Ray currentRay = new Ray(controllerLeft.transform.position, controllerLeft.transform.forward);

            Vector2 start = new Vector2(selectionStartRay.direction.x, selectionStartRay.direction.z);
            Vector2 current = new Vector2(currentRay.direction.x, currentRay.direction.z);

            float rotation = Vector2.SignedAngle(current, start) * Mathf.Deg2Rad;
            foreach (Pipe pipe in selection)
            {
                pipe.angleOffset = rotation;
            }
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
