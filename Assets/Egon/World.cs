using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
	public class World
	{
		
	}

	public class Wall
	{
		public Ring[] rings;
		public List<Connection> connections;

		public Wall (int ringCount, int tileCount)
		{
			rings = new Ring[ringCount];
			for (int i = 0; i < rings.Length; i++) {
				rings [i] = new Ring (this, i, tileCount);
			}

			connections = new List<Connection> ();
		}

		// update connections list
		private void updateConnections ()
		{
			connections.Clear ();

			// TODO
		}


		public void Randomize ()
		{
			foreach (Ring ring in rings) {
				ring.Randomize ();
			}

			updateConnections ();
		}

		public void RandomizeColors ()
		{
			foreach (Ring ring in rings) {
				foreach (Tile tile in ring.tiles) {
					foreach (Joint joint in tile.joints) {
						joint.liquid.Randomize ();
					}
				}
			}
		}
	}

	public class Ring
	{
		public Wall wall;
		public int layer;

		public Tile[] tiles;

		public Ring (Wall wall, int layer, int tileCount)
		{
			this.wall = wall;
			this.layer = layer;
			tiles = new Tile[tileCount];
			for (int i = 0; i < tiles.Length; i++) {
				tiles [i] = new Tile (this, layer, i);
			}
		}

		// randomize each tile in ring
		public void Randomize ()
		{
			foreach (Tile tile in tiles) {
				tile.Randomize ();
			}
		}
	}

	public class Tile
	{
		public Ring ring;
		public int index, layer;

		[Range (0, 5)]
		public byte rotation = 0;
		public bool[] used = new bool[6];
		public Joint[] joints = new Joint[0];

		public Tile (Ring ring, int layer, int index)
		{
			this.ring = ring;

			this.layer = layer;
			this.index = index;
		}

		// update used ports list
		private void updateUsed ()
		{
			for (int i = 0; i < this.used.Length; i++) {
				this.used [i] = false;
			}
			for (int i = 0; i < this.joints.Length; i++) {
				for (int k = 0; k < this.joints [i].ports.Length; k++) {
					this.used [this.joints [i].ports [k]] = true;
				}
			}
		}

		// choose random joints
		public void Randomize ()
		{
			int n = Random.Range (1, 3);
			this.joints = new Joint[n];

			List<byte> available = new List<byte> ();

			available.Add (0);
			available.Add (1);
			available.Add (2);
			available.Add (3);
			available.Add (4);
			available.Add (5);

			for (int i = 0; i < this.joints.Length; i++) {
				this.joints [i] = new Joint ();
				this.joints [i].Randomize (available);
			}

			updateUsed ();
		}

		// Angle is the angular wall-space position
		public float Angle ()
		{
			return (float)Mathf.PI * 2.0f * (float)index / (float)ring.tiles.Length;
		}

		// Returns Y height in tile units
		public float Y ()
		{
			return (float)this.layer + ((index & 1) == 1 ? 0.5f : 0.0f);
		}
	}

	public class Joint
	{
		public byte[] ports = new byte[0];
		public Liquid liquid = new Liquid ();

		// pick random ports from available
		public void Randomize (List<byte> available)
		{
			int n = Random.Range (1, Mathf.Min (available.Count, 3));
			ports = new byte[n];
			for (int i = 0; i < ports.Length; i++) {
				int k = Random.Range (0, available.Count);
				ports [i] = available [k];
				available.RemoveAt (k);
			}
		}
	}

	public class Connection
	{
		public Tile a, b;

		public Connection (Tile a, Tile b)
		{
			bool swap = false;
			if (a.layer == b.layer) {
				swap = b.index > a.index;
			} else if (b.layer > a.layer) {
				swap = true;
			}

			this.a = swap ? b : a;
			this.b = swap ? a : b;
		}
	}

	public struct Liquid
	{
		public bool r, g, b;

		public Liquid (bool r, bool g, bool b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public Liquid Mix (Liquid b)
		{
			Liquid a = this;
			return new Liquid (a.r | b.r, a.g | b.g, a.b | b.b);
		}

		public Color Color ()
		{
			Color c = new Color ();
			c.a = 1;
			c.r = this.r ? 1 : 0;
			c.g = this.g ? 1 : 0;
			c.b = this.b ? 1 : 0;
			return c;
		}

		public void Randomize ()
		{
			int v = Random.Range (1, 8);
			this.r = ((v >> 0) & 1) == 1;
			this.g = ((v >> 1) & 1) == 1;
			this.b = ((v >> 2) & 1) == 1;
		}

		public bool IsFilled ()
		{
			return this.r | this.g | this.b;
		}

		public bool IsEmpty ()
		{
			return !this.IsFilled ();
		}
	}
}