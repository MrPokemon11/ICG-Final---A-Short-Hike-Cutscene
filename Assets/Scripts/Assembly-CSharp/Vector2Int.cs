using System;
using System.Text;
using UnityEngine;

public struct Vector2Int
{
	public static readonly Vector2Int[] MAIN_DIRECTIONS = new Vector2Int[4]
	{
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1)
	};

	public static readonly Vector2Int[] ALL_DIRECTIONS = new Vector2Int[8]
	{
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
		new Vector2Int(1, 1),
		new Vector2Int(-1, 1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, -1)
	};

	public static readonly Vector2Int[] CORNER_DIRECTIONS = new Vector2Int[4]
	{
		new Vector2Int(1, 1),
		new Vector2Int(-1, 1),
		new Vector2Int(-1, -1),
		new Vector2Int(1, -1)
	};

	public static readonly Vector2Int zero = new Vector2Int(0, 0);

	public static readonly Vector2Int unitX = new Vector2Int(1, 0);

	public static readonly Vector2Int unitY = new Vector2Int(0, 1);

	public int x;

	public int y;

	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public bool IsNeighbors(Vector2Int other)
	{
		if (Math.Abs(other.x - x) <= 1)
		{
			return Math.Abs(other.y - y) <= 1;
		}
		return false;
	}

	public Vector3 ToV3()
	{
		return new Vector3(x, y, 0f);
	}

	public Vector2 ToV2()
	{
		return new Vector2(x, y);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Vector2Int))
		{
			return false;
		}
		return this == (Vector2Int)obj;
	}

	public bool Equals(Vector2Int other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() + y.GetHashCode();
	}

	public Vector2Int TurnClockwise()
	{
		return new Vector2Int(-y, x);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(24);
		stringBuilder.Append("{X:");
		stringBuilder.Append(x);
		stringBuilder.Append(" Y:");
		stringBuilder.Append(y);
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public static Vector2Int operator -(Vector2Int value)
	{
		value.x = -value.x;
		value.y = -value.y;
		return value;
	}

	public static bool operator ==(Vector2Int value1, Vector2Int value2)
	{
		if (value1.x == value2.x)
		{
			return value1.y == value2.y;
		}
		return false;
	}

	public static bool operator !=(Vector2Int value1, Vector2Int value2)
	{
		if (value1.x == value2.x)
		{
			return value1.y != value2.y;
		}
		return true;
	}

	public static Vector2Int operator +(Vector2Int value1, Vector2Int value2)
	{
		value1.x += value2.x;
		value1.y += value2.y;
		return value1;
	}

	public static Vector2Int operator -(Vector2Int value1, Vector2Int value2)
	{
		value1.x -= value2.x;
		value1.y -= value2.y;
		return value1;
	}

	public static Vector2Int operator *(Vector2Int value1, Vector2Int value2)
	{
		value1.x *= value2.x;
		value1.y *= value2.y;
		return value1;
	}

	public static Vector2Int operator *(Vector2Int value, int scaleFactor)
	{
		value.x *= scaleFactor;
		value.y *= scaleFactor;
		return value;
	}

	public static Vector2Int operator *(int scaleFactor, Vector2Int value)
	{
		value.x *= scaleFactor;
		value.y *= scaleFactor;
		return value;
	}

	public static Vector2Int operator /(Vector2Int value1, Vector2Int value2)
	{
		value1.x /= value2.x;
		value1.y /= value2.y;
		return value1;
	}

	public static Vector2Int operator /(Vector2Int value1, int divider)
	{
		float num = 1 / divider;
		value1.x = (int)((float)value1.x * num);
		value1.y = (int)((float)value1.y * num);
		return value1;
	}

	public Vector2Int SetX(int x)
	{
		Vector2Int result = this;
		result.x = x;
		return result;
	}

	public Vector2Int SetY(int y)
	{
		Vector2Int result = this;
		result.y = y;
		return result;
	}
}
