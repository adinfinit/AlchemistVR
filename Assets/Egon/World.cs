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
		public Layer[] layers;
		public List<Connection> connections;

		public Wall (int layerCount, int tileCount, int sources, int drains)
		{
			layers = new Layer[layerCount];
			for (int i = 0; i < layers.Length; i++) {
				layers [i] = new Layer (this, i, tileCount);
			}

			MakeLayerSpecial (layers [0], Tile.Kind.Source, sources);
			MakeLayerSpecial (layers [layers.Length - 1], Tile.Kind.Drain, drains);
				
			connections = new List<Connection> ();
		}

		void MakeLayerSpecial (Layer layer, Tile.Kind kind, int count)
		{
			layer.locked = true;

			for (int i = 0; i < layer.tiles.Length; i++) {
				layer.tiles [i] = null;
			}

			byte port = kind == Tile.Kind.Drain ? (byte)0 : (byte)3;

			for (int k = 0; k < count; k++) {
				int index = (k * 2 + layer.tiles.Length) % layer.tiles.Length;
				Tile tile = new Tile (layer, index);
				tile.kind = kind;
				layer.tiles [index] = tile;

				Joint joint = new Joint (tile);
				tile.joints = new Joint[1]{ joint };
				joint.ports = new byte[1]{ port };
			}
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
			foreach (Layer layer in layers) {
				for (int i = 0; i < layer.tiles.Length; i += 2) {
					Tile tile = layer.tiles [i];
					if (tile == null) {
						continue;
					}

					foreach (Joint joint in tile.joints) {
						changed = joint.Update () || changed;
					}
				}
				for (int i = 1; i < layer.tiles.Length; i += 2) {
					Tile tile = layer.tiles [i];
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
						joint.sources.Clear ();
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
				connections = conns;
				// TODO: notify UI
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
			drain.sources.Add (source);
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
			tiles = new Tile[tileCount];
			for (int i = 0; i < tiles.Length; i++) {
				tiles [i] = new Tile (this, i);
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
			for (int i = 0; i < this.ports.Length; i++) {
				this.ports [i] = null;
			}
			for (int i = 0; i < this.joints.Length; i++) {
				for (int k = 0; k < this.joints [i].ports.Length; k++) {
					this.ports [this.joints [i].ports [k]] = this.joints [i];
				}
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

		public List<Joint> sources = new List<Joint> ();
		public List<Joint> drains = new List<Joint> ();

		public Liquid liquid = new Liquid ();

		public Joint (Tile tile)
		{
			this.tile = tile;
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