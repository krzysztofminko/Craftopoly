using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Distance {

	public enum Axis { x, y, z}

	public static float Manhattan2D(float startx, float starty, float endx, float endy)
	{
		return Math.Abs(startx - endx) + Math.Abs(starty - endy);
	}

	public static float Manhattan2D(Vector3 start, Vector3 end, Axis axisUp = Axis.y)
	{		
		if (axisUp == Axis.y) return Manhattan2D(start.x, start.z, end.x, end.z);
		else if (axisUp == Axis.z) return Manhattan2D(start.x, start.y, end.x, end.y);
		else return Manhattan2D(start.y, start.z, end.y, end.z);
	}
}
