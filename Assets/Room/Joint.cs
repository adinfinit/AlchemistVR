using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
	public const float Thickness = 0.15f;

	public Tile tile;

	public int index;
	public byte[] ports = new byte[0];

	public Liquid liquid;
	public Liquid nextLiquid;

	void Start ()
	{
	}

	public void Init (Tile tile, int index, byte[] ports)
	{
		this.tile = tile;
		this.index = index;
		this.ports = ports;

		RecreatePipes ();

		if (tile.kind == Tile.Kind.Source) {
			liquid.SetPrimary (tile.index);
		} else {
			liquid.Randomize ();
		}
	}

	void Update ()
	{
		if (tile == null) {
			return;
		}

		Color color = liquid.Color ();
		foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer> ()) {
			renderer.material.color = Color.Lerp (renderer.material.color, color, 0.1f);
		}
	}

	public Vector3 LocalPort (byte port)
	{	
		float p = (float)port / 6f;
		float angle = p * Mathf.PI * 2f;

		Vector3 pos = Vector3.zero;
		pos.x = -Mathf.Sin (angle) * 0.5f;
		pos.y = Mathf.Cos (angle) * 0.5f;

		return pos;
	}

	public Vector3 GlobalPort (byte port)
	{
		Vector3 local = LocalPort (port);
		return (Vector3)(transform.localToWorldMatrix * new Vector4 (local.x, local.y, local.z, 1f));
	}

	void RecreatePipes ()
	{
		for (int i = transform.childCount - 1; i >= 0; i--) {
			Destroy (transform.GetChild (i).gameObject);
		}

		Vector3 center = Vector3.zero;
		if (tile.joints.Length > 1) {
			if ((index & 1) == 1) {
				center = new Vector3 (0f, 0f, -2f * Thickness);
			} else {
				center = new Vector3 (0f, 0f, 2f * Thickness);
			}
		}

		foreach (byte port in ports) {
			Vector3 location = GlobalPort (port);
			GameObject cyl = Geometry.CylinderBetweenPoints (location, center, Thickness);
			cyl.name = "Port " + port;
			cyl.transform.SetParent (transform, false);
		}

		if (tile.kind == Tile.Kind.Pipe) {
			GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphere.transform.position = center;
			sphere.transform.localScale = Vector3.one * Thickness * 2f;
			sphere.transform.SetParent (transform, false);
		}
	}
}
