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
		public Lab lab;

		public Layer[] layers;
		public List<Connection> connections;

		public Wall (Lab lab, int layerCount, int tileCount, int sources, int drains)
		{
			layers = new Layer[layerCount];

			for (int i = 0; i < layers.Length; i++) {
				layers [i] = new Layer (this, i, tileCount);

				if (i == 0) {
					layers [i].InitLocked (Tile.Kind.Source, sources);
				} else if (i == layers.Length - 1) {
					layers [i].InitLocked (Tile.Kind.Drain, drains);
				} else {
					layers [i].InitTiles ();
				}
			}

			connections = new List<Connection> ();
		}

		List<Tile> Tiles ()
		{
			List<Tile> tiles = new List<Tile> ();

			foreach (Layer layer in layers) {
				for (int i = 0; i < layer.tiles.Length; i += 2) {
					if (layer.tiles [i] != null) {
						tiles.Add (layer.tiles [i]);
					}
				}
				for (int i = 1; i < layer.tiles.Length; i += 2) {
					if (layer.tiles [i] != null) {
						tiles.Add (layer.tiles [i]);
					}
				}
			}

			return tiles;
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

			List<Tile> tiles = Tiles ();

			foreach (Tile tile in tiles) {
				foreach (Joint joint in tile.joints) {
					joint.ResetStep ();
				}
			}

			foreach (Tile tile in tiles) {
				foreach (Joint joint in tile.joints) {
					joint.Drain ();
				}
			}

			foreach (Tile tile in tiles) {
				foreach (Joint joint in tile.joints) {
					if (joint.Step ()) {
						if (lab != null) {
							lab.JointChanged (joint);
						}
					}
				}
			}
		}

		// update connections and joints
		public void UpdateConnections ()
		{
			List<Connection> conns = new List<Connection> ();

			foreach (Layer layer in layers) {
				foreach (Tile tile in layer.tiles) {
					if (tile == null) {
						continue;
					}

					foreach (Joint joint in tile.joints) {
						joint.drains.Clear ();
					}
				}
			}

			foreach (Layer layer in layers) {
				Layer layerBottom = null;
				if (layer.index + 1 < layers.Length) {
					layerBottom = layers [layer.index + 1];
				}

				List<Connection> tops = new List<Connection> ();
				List<Connection> bottoms = new List<Connection> ();

				foreach (Tile tile in layer.tiles) {
					if (tile == null) {
						continue;
					}

					Tile left = null;
					Tile bottom = null;
					Tile right = null;

					if (layerBottom != null) {
						bottom = layerBottom.At (tile.index);
					}

					Layer layerSide = tile.IsOffset () ? layerBottom : layer;
					if (layerSide != null) {
						left = layerSide.At (tile.index - 1);
						right = layerSide.At (tile.index + 1);
					}

					List<Connection> target = tile.IsOffset () ? bottoms : tops;
					TryConnect (target, tile, 3, bottom, 0);
					TryConnect (target, tile, 4, left, 1);
					TryConnect (target, tile, 2, right, 5);
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
				if (lab != null) {
					// TODO: don't destroy everything when some connection changes
					foreach (Connection conn in connections) {
						lab.ConnectionDestroyed (conn);	
					}
				}

				connections = conns;

				if (lab != null) {
					foreach (Connection conn in connections) {
						lab.ConnectionCreated (conn);	
					}
				}
			}
		}

		private static void TryConnect (List<Connection> conns, Tile sourceTile, byte sourcePort, Tile drainTile, byte drainPort)
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
		}
	}

	public class Layer
	{
		public Wall wall;
		public int index;
		public bool locked = false;

		public Tile[] tiles;

		public Layer (Wall wall, int index, int tileCount)
		{
			this.wall = wall;
			this.index = index;
			this.tiles = new Tile[tileCount];
		}

		public void InitTiles ()
		{
			for (int i = 0; i < tiles.Length; i++) {
				tiles [i] = new Tile (this, i);
				if (wall.lab != null) {
					wall.lab.TileCreated (tiles [i]);
				}
			}
		}

		public void InitLocked (Tile.Kind kind, int count)
		{
			locked = true;

			byte port = kind == Tile.Kind.Drain ? (byte)0 : (byte)3;
			for (int k = 0; k < count; k++) {
				int index = (k * 2 + tiles.Length) % tiles.Length;
				Tile tile = new Tile (this, index);
				tile.kind = kind;

				Joint joint = new Joint (tile);
				tile.joints = new Joint[1]{ joint };
				joint.ports = new byte[1]{ port };

				tiles [index] = tile;

				if (wall.lab != null) {
					wall.lab.TileCreated (tile);
				}
			}
		}

		public void Connect ()
		{


		}

		public void Disconnect ()
		{

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

			wall.UpdateConnections ();
		}
	}

	public class Tile
	{
		public enum Kind
		{
			Pipe,
			Source,
			Drain
		}

		public Object visual = null;

		public Layer layer;
		public Kind kind = Kind.Pipe;
		public int index;

		// null | Joint that is in that port
		public Joint[] ports = new Joint[6];
		// list of all joints
		public Joint[] joints = new Joint[0];

		public Tile (Layer layer, int index)
		{
			this.layer = layer;
			this.index = index;
		}

		// update used ports list
		public void UpdateCrossReference ()
		{
			for (int i = 0; i < ports.Length; i++) {
				ports [i] = null;
			}

			for (int i = 0; i < joints.Length; i++) {
				for (int k = 0; k < joints [i].ports.Length; k++) {
					ports [joints [i].ports [k]] = joints [i];
				}
			}

			if (layer.wall.lab != null) {
				layer.wall.lab.TileChanged (this);
			}
		}

		public bool IsOffset ()
		{
			return (index & 1) == 1;
		}

		override public string ToString ()
		{
			return "L" + layer.index + "T" + index;
		}
	}

	public class Joint
	{
		public GameObject gameObject = null;

		public Tile tile;
		public byte[] ports = new byte[0];

		public List<Joint> drains = new List<Joint> ();

		public Liquid liquid = new Liquid ();
		public Liquid nextLiquid = new Liquid ();

		public Joint (Tile tile)
		{
			this.tile = tile;
		}

		public void ResetStep ()
		{
			this.nextLiquid = new Liquid ();
		}

		public void Drain ()
		{
			foreach (Joint drain in drains) {
				// use nextLiquid to immediate flow
				drain.nextLiquid = drain.nextLiquid.Mix (this.liquid);
			}
		}

		public bool Step ()
		{
			if (!liquid.Equals (nextLiquid)) {
				liquid = nextLiquid;
				return true;
			}

			return false;
		}

		override public string ToString ()
		{
			return tile.ToString () + "P" + ports;
		}
	}

	public class Connection
	{
		public GameObject gameObject = null;

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

		override public string ToString ()
		{
			return source.tile.ToString () + "P" + sourcePort + "_" + drain.tile.ToString () + "P" + drainPort;
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

		public bool IsFilled ()
		{
			return this.r | this.g | this.b;
		}

		public bool IsEmpty ()
		{
			return !this.IsFilled ();
		}
	}

	public static class Randomize
	{
		public static void Wall (Wall wall)
		{
			foreach (Layer layer in wall.layers) {
				Layer (layer);
			}

			wall.UpdateConnections ();
		}

		public static void Layer (Layer layer)
		{
			if (layer.locked) {
				return;
			}

			foreach (Tile tile in layer.tiles) {
				if (tile == null) {
					continue;
				}
				Tile (tile);
			}
		}

		public static void Tile (Tile tile)
		{
			int n = Random.Range (1, 3);
			tile.joints = new Joint[n];

			List<byte> available = new List<byte> ();
			for (byte k = 0; k < 6; k++) {
				available.Add (k);
			}

			for (int i = 0; i < tile.joints.Length; i++) {
				tile.joints [i] = new Joint (tile);
				Joint (tile.joints [i], available);
			}

			tile.UpdateCrossReference ();
		}

		public static void Joint (Joint joint, List<byte> available)
		{
			// pick random ports from available
			int n = Random.Range (1, Mathf.Min (available.Count, 3));
			joint.ports = new byte[n];
			for (int i = 0; i < joint.ports.Length; i++) {
				int k = Random.Range (0, available.Count);
				joint.ports [i] = available [k];
				available.RemoveAt (k);
			}

			Liquid (ref joint.liquid);
		}

		public static void Liquid (ref Liquid liquid)
		{
			int v = Random.Range (1, 8);
			liquid.r = ((v >> 0) & 1) == 1;
			liquid.g = ((v >> 1) & 1) == 1;
			liquid.b = ((v >> 2) & 1) == 1;
		}
	}
}