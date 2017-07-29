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

		public Wall (int ringCount, int tileCount, int sources, int drains)
		{
			rings = new Ring[ringCount];
			for (int i = 0; i < rings.Length; i++) {
				rings [i] = new Ring (this, i, tileCount);
			}
			rings [0].MakeSpecial (sources, 2);
			rings [rings.Length - 1].MakeSpecial (sources, 0);

			connections = new List<Connection> ();
		}

		public void Drain ()
		{
			// scan from top to bottom
			// 
			//  0,0     0,2     0,4
			//      0,1     0,3
			//  1,0     1,2     1,4
			//      1,1     1,3
			//
			bool changed = false;
			foreach (Ring ring in rings) {
				for (int i = 0; i < ring.tiles.Length; i += 2) {
					Tile tile = ring.tiles [i];
					if (tile == null) {
						continue;
					}

					foreach (Joint joint in tile.joints) {
						changed = joint.Update () || changed;
					}
				}
				for (int i = 1; i < ring.tiles.Length; i += 2) {
					Tile tile = ring.tiles [i];
					if (tile == null) {
						continue;
					}

					foreach (Joint joint in tile.joints) {
						changed = joint.Update () || changed;
					}
				}
			}

			if (changed) {
				// TODO: cache if no changes
			}
		}

		// update connections and joints
		public void updateConnections ()
		{
			List<Connection> conns = new List<Connection> ();

			foreach (Ring ring in rings) {
				foreach (Tile tile in ring.tiles) {
					if (tile == null) {
						continue;
					}

					foreach (Joint joint in tile.joints) {
						joint.drains.Clear ();
						joint.sources.Clear ();
					}
				}
			}

			foreach (Ring ring in rings) {
				Ring ringb = null;
				if (ring.layer + 1 < rings.Length) {
					ringb = rings [ring.layer + 1];
				}

				List<Connection> tops = new List<Connection> ();
				List<Connection> bottoms = new List<Connection> ();

				foreach (Tile tile in ring.tiles) {
					if (tile == null) {
						continue;
					}

					Tile left = null;
					Tile bottom = null;
					Tile right = null;

					if (ringb != null) {
						bottom = ringb.At (tile.index);
					}

					Ring ringlr = tile.IsOffset () ? ringb : ring;
					if (ringlr != null) {
						left = ringlr.At (tile.index - 1);
						right = ringlr.At (tile.index + 1);
					}

					List<Connection> target = tile.IsOffset () ? bottoms : tops;
					tryConnect (target, tile, 3, bottom, 0);
					tryConnect (target, tile, 4, left, 1);
					tryConnect (target, tile, 2, right, 5);
				}

				conns.AddRange (tops);
				conns.AddRange (bottoms);
			}

			bool changed = false;
			if (conns.Count != connections.Count) {
				changed = true;
			} else {
				for (int i = 0; i < conns.Count; i++) {
					if (!conns [i].Equals (connections [i])) {
						changed = true;
						break;
					}
				}
			}

			if (changed) {
				connections = conns;
				// TODO: notify UI
			}
		}

		private static void tryConnect (List<Connection> conns, Tile sourceTile, byte sourcePort, Tile drainTile, byte drainPort)
		{
			if (sourceTile == null || drainTile == null) {
				return;
			}

			Joint source = sourceTile.ports [sourcePort];
			Joint drain = drainTile.ports [drainPort];
			if (source == null || drain == null) {
				return;
			}

			conns.Add (new Connection (source, sourcePort, drain, drainPort));
			source.drains.Add (drain);
			drain.sources.Add (source);
		}

		public void Randomize ()
		{
			for (int i = 0; i < rings.Length; i++) {
				Ring ring = rings [i];
				ring.Randomize ();
			}

			updateConnections ();
		}

		public void RandomizeColors ()
		{
			foreach (Ring ring in rings) {
				ring.RandomizeColors ();
			}
		}
	}

	public class Ring
	{
		public Wall wall;
		public int layer;
		//TODO: use enum instead of flag
		public bool special = false;

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

		public void MakeSpecial (int count, byte port)
		{
			special = true;
			for (int i = 0; i < tiles.Length; i++) {
				tiles [i] = null;
			}

			for (int k = 0; k < count; k++) {
				int index = (k * 2 + tiles.Length) % tiles.Length;
				Tile tile = new Tile (this, layer, index);
				tiles [index] = tile;

				Joint joint = new Joint (tile);
				tile.joints = new Joint[1]{ joint };
				joint.ports = new byte[1]{ port };
			}
		}

		public Tile At (int i)
		{
			return tiles [(i + tiles.Length) % tiles.Length];
		}


		public void Rotate (int offset)
		{
			Tile[] next = new Tile[tiles.Length];
			for (int i = 0; i < tiles.Length; i++) {
				next [(i + offset + tiles.Length) % tiles.Length] = tiles [i];
			}
			for (int i = 0; i < next.Length; i++) {
				next [i].index = i;
			}
			tiles = next;

			wall.updateConnections ();
		}

		// randomize each tile in ring
		public void Randomize ()
		{
			if (special) {
				return;
			}

			foreach (Tile tile in tiles) {
				if (tile == null) {
					continue;
				}

				tile.Randomize ();
			}
		}

		public void RandomizeColors ()
		{
			foreach (Tile tile in tiles) {
				if (tile == null) {
					continue;
				}
				foreach (Joint joint in tile.joints) {
					joint.liquid.Randomize ();
				}
			}
		}
	}

	public class Tile
	{
		public enum Kind
		{
			Pipe,
			Source,
			Potion
		}

		public Ring ring;
		public Kind kind = Kind.Pipe;
		public int index, layer;

		// null | Joint that is in that port
		public Joint[] ports = new Joint[6];
		// list of all joints
		public Joint[] joints = new Joint[0];

		public Tile (Ring ring, int layer, int index)
		{
			this.ring = ring;

			this.layer = layer;
			this.index = index;
		}

		// update used ports list
		private void updateCrossReference ()
		{
			for (int i = 0; i < this.ports.Length; i++) {
				this.ports [i] = null;
			}
			for (int i = 0; i < this.joints.Length; i++) {
				for (int k = 0; k < this.joints [i].ports.Length; k++) {
					this.ports [this.joints [i].ports [k]] = this.joints [i];
				}
			}
		}

		// choose random joints
		public void Randomize ()
		{
			if (kind == Kind.Potion) {
				
			} else if (kind == Kind.Source) {
				
			}

			int n = Random.Range (1, 3);
			joints = new Joint[n];

			List<byte> available = new List<byte> ();

			available.Add (0);
			available.Add (1);
			available.Add (2);
			available.Add (3);
			available.Add (4);
			available.Add (5);

			for (int i = 0; i < joints.Length; i++) {
				joints [i] = new Joint (this);
				joints [i].Randomize (available);
			}

			updateCrossReference ();
		}

		public bool IsOffset ()
		{
			return (index & 1) == 1;
		}

		// Angle is the angular wall-space position
		public float Angle ()
		{
			return (float)Mathf.PI * 2.0f * (float)index / (float)ring.tiles.Length;
		}

		// Returns Y height in tile units
		public float Y ()
		{
			return (float)layer + (IsOffset () ? 0.5f : 0.0f);
		}
	}

	public class Joint
	{
		public Tile tile;
		public byte[] ports = new byte[0];

		public List<Joint> sources = new List<Joint> ();
		public List<Joint> drains = new List<Joint> ();

		public Liquid liquid = new Liquid ();

		public Joint (Tile tile)
		{
			this.tile = tile;
		}

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

		public bool Update ()
		{
			if (sources.Count == 0) {
				return false;
			}

			Liquid next = new Liquid ();
			foreach (Joint source in sources) {
				next = next.Mix (source.liquid);
			}

			if (next.Equals (liquid)) {
				return false;
			}

			liquid = next;

			// TODO: notify changed

			return true;
		}
	}

	public class Connection
	{
		public Joint source, drain;
		public byte sourcePort, drainPort;

		public Connection (Joint source, byte sourcePort, Joint drain, byte drainPort)
		{
			this.source = source;
			this.sourcePort = sourcePort;
			this.drain = drain;
			this.drainPort = drainPort;
		}

		public bool Equals (Connection b)
		{
			Connection a = this;
			return (a.source == b.source) && (a.drain == b.drain) &&
			(a.sourcePort == b.sourcePort) && (a.drainPort == b.drainPort);
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

		public bool Equals (Liquid b)
		{
			Liquid a = this;
			return a.r == b.r && a.g == b.g && a.b == b.b;
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