using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draining
{
	public static void Drain (Wall wall)
	{
		for (int layer = 0; layer < wall.LayerCount; layer++) {
			for (int index = 0; index < wall.TileCount; index++) {
				Tile tile = wall.Get (layer, index);
				if (tile == null || !tile.attached) {
					continue;
				}

				foreach (Joint joint in tile.joints) {
					if (tile.kind == Tile.Kind.Source || !tile.attached) {
						joint.nextLiquid = joint.liquid;
					} else {
						joint.nextLiquid = new Liquid ();
					}
				}
			}
		}

		for (int layer = 0; layer < wall.LayerCount; layer++) {
			for (int index = 0; index < wall.TileCount; index++) {
				Tile tile = wall.Get (layer, index);
				if (tile == null) {
					continue;
				}

				DrainTile (tile);

				foreach (Joint joint in tile.joints) {
					joint.liquid = joint.nextLiquid;
				}
			}
		}
	}

	public static void DrainTile (Tile tile)
	{
		Tile left, bottom, right;
		tile.GetDrains (out left, out bottom, out right);

		FlowDown (tile, 4, left, 1);
		FlowDown (tile, 3, bottom, 0);
		FlowDown (tile, 2, right, 5);
	}

	public static void FlowDown (Tile source, byte sourcePort, Tile drain, byte drainPort)
	{
		bool connected = source.ports [sourcePort] != null && drain != null && drain.ports [drainPort] != null &&
		                 source.attached && drain.attached;
		if (connected) {
			Joint sourceJoint = source.ports [sourcePort];
			Joint drainJoint = drain.ports [drainPort];
			drainJoint.nextLiquid = drainJoint.nextLiquid.Mix (sourceJoint.liquid);
			// for immediate flow:
			// drainJoint.nextLiquid = drainJoint.nextLiquid.Mix (sourceJoint.nextLiquid);
		}

		if (source.valve [sourcePort] == null && connected) {
			source.AddValve (sourcePort, drain, drainPort);
		} else if (source.valve [sourcePort] != null && !connected) {
			source.RemoveValve (sourcePort);
		}
	}
}
