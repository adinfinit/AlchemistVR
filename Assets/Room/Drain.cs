using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ParticleSystem))]
[RequireComponent (typeof(Tile))]
public class Drain : MonoBehaviour
{
	ParticleSystem particles;
	Tile tile;

	void Start ()
	{
		particles = GetComponent<ParticleSystem> ();
		tile = GetComponent<Tile> ();	
	}

	void Update ()
	{
		if (tile.joints.Length == 0 || tile.joints [0].liquid.IsEmpty ()) {
			if (particles.isPlaying) {
				particles.Stop ();
			}
		} else {
			if (!particles.isPlaying) {
				particles.Play ();
			}
			Color color = tile.joints [0].liquid.Color ();
			ParticleSystem.MainModule main = particles.main;
			main.startColor = color;
		}
	}
}
