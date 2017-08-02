using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		c.a = 1f;
		c.r = r ? 1f : 0f;
		c.g = g ? 1f : 0f;
		c.b = b ? 1f : 0f;
		return c;
	}

	public bool IsFilled ()
	{
		return r | g | b;
	}

	public bool IsEmpty ()
	{
		return !IsFilled ();
	}

	public void Randomize ()
	{
		int v = Random.Range (1, 8);
		r = ((v >> 0) & 1) == 1;
		g = ((v >> 1) & 1) == 1;
		b = ((v >> 2) & 1) == 1;
	}

	public void RandomizePrimary ()
	{
		int v = Random.Range (1, 4);

		r = v == 1;
		g = v == 2;
		b = v == 3;
	}
}