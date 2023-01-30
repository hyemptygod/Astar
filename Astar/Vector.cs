namespace Astar
{
	using System;
    using System.Collections;
    using System.Collections.Generic;

	/// <summary>
	///   <para>Representation of 2D vectors and points using integers.</para>
	/// </summary>
	public struct Vector : IEquatable<Vector>
	{
		private const float D1 = 1f;
		private const float D2 = 1.414213f;

		/// <summary>
		/// zero
		/// </summary>
		public static readonly Vector zero = new Vector(0, 0);
		/// <summary>
		/// one
		/// </summary>
		public static readonly Vector one = new Vector(1, 1);
		/// <summary>
		/// 上
		/// </summary>
		public static readonly Vector up = new Vector(0, 1);
		/// <summary>
		/// 左
		/// </summary>
		public static readonly Vector left = new Vector(-1, 0);
		/// <summary>
		/// 右
		/// </summary>
		public static readonly Vector right = new Vector(1, 0);
		/// <summary>
		/// 下
		/// </summary>
		public static readonly Vector down = new Vector(0, -1);

		/// <summary>
		/// 左上
		/// </summary>
		public static readonly Vector left_up = new Vector(-1, 1);
		/// <summary>
		/// 左下
		/// </summary>
		public static readonly Vector left_down = new Vector(-1, -1);
		/// <summary>
		/// 右上
		/// </summary>
		public static readonly Vector right_up = new Vector(1, 1);
		/// <summary>
		/// 右下
		/// </summary>
		public static readonly Vector right_down = new Vector(1, -1);

		public static readonly Vector[] four =
		{
			up,
			right,
			down,
			left,
		};

		public static readonly Vector[] eight =
		{
			up,
			right_up,
			right,
			right_down,
			down,
			left_down,
			left,
			left_up,
		};

		public static readonly Dictionary<Vector, Vector[]> dicExcept = new Dictionary<Vector, Vector[]>()
		{
            {up, new Vector[]{right_up,left_up} },
            {down, new Vector[]{right_down,left_down} },
            {left, new Vector[]{left_down,left_up} },
            {right,new Vector[]{right_down,right_up} }
        };

		public static Vector[] Neighbours(NeighbourMode mode)
        {
			switch(mode)
            {
				case NeighbourMode.Four:
					return four;
				case NeighbourMode.Eight:
					return eight;
				default:
					return four;
            }
        }

		/// <summary>
		///   <para>X component of the vector.</para>
		/// </summary>
		public int x;

		/// <summary>
		///   <para>Y component of the vector.</para>
		/// </summary>
		public int y;

		public int this[int index]
		{
			get
			{
				if(index == 0)
                {
					return x;
                }
				else if(index == 1)
                {
					return y;
                }
                else
                {
					throw new IndexOutOfRangeException($"Invalid Vector2Int index addressed: {index}!");
                }
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					default:
						throw new IndexOutOfRangeException($"Invalid Vector2Int index addressed: {index}!");
				}
			}
		}

		/// <summary>
		///   <para>Returns the length of this vector (Read Only).</para>
		/// </summary>
		public float magnitude => (float)Math.Sqrt(x * x + y * y);

		/// <summary>
		///   <para>Returns the squared length of this vector (Read Only).</para>
		/// </summary>
		public int sqrMagnitude => x * x + y * y;

		public Vector(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		///   <para>Set x and y components of an existing Vector2Int.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Set(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public bool Normal()
        {
			if(x != 0 && y != 0)
            {
				return false;
            }

			if(x == 0 && y == 0)
            {
				return false;
            }

			if(x == 0)
            {
				y /= Math.Abs(y);
            }
            else if(y == 0)
            {
				x /= Math.Abs(x);
            }
			return true;
        }

		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Distance(Vector a, Vector b)
		{
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt(num * num + num2 * num2);
		}

		/// <summary>
		/// 曼哈顿距离
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float Manhattan(Vector a, Vector b)
		{
			float dx = Math.Abs(a.x - b.x);
			float dy = Math.Abs(a.y - b.y);
			return D1 * (dx + dy);
		}

		/// <summary>
		/// 契比雪夫(对角线)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float Chebyshev(Vector a, Vector b)
		{
			float dx = Math.Abs(a.x - b.x);
			float dy = Math.Abs(a.y - b.y);
			if (dx > dy)
			{
				return D1 * (dx - dy) + D2 * dy;
			}
			else
			{
				return D1 * (dy - dx) + D2 * dx;
			}
		}

		/// <summary>
		///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector Min(Vector lhs, Vector rhs)
		{
			return new Vector(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
		}

		/// <summary>
		///   <para>Returns a vector that is made from the largest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector Max(Vector lhs, Vector rhs)
		{
			return new Vector(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
		}

		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector Scale(Vector a, Vector b)
		{
			return new Vector(a.x * b.x, a.y * b.y);
		}

		/// <summary>
		///   <para>Multiplies every component of this vector by the same component of scale.</para>
		/// </summary>
		/// <param name="scale"></param>
		public void Scale(Vector scale)
		{
			x *= scale.x;
			y *= scale.y;
		}

		/// <summary>
		///   <para>Clamps the Vector2Int to the bounds given by min and max.</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void Clamp(Vector min, Vector max)
		{
			x = Math.Max(min.x, x);
			x = Math.Min(max.x, x);
			y = Math.Max(min.y, y);
			y = Math.Min(max.y, y);
		}

		public Vector Abs()
        {
			return new Vector(Math.Abs(x), Math.Abs(y));
        }

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.x + b.x, a.y + b.y);
		}

		public static Vector operator -(Vector a)
        {
			return new Vector(-a.x, -a.y);
		}

		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.x - b.x, a.y - b.y);
		}

		public static Vector operator *(Vector a, Vector b)
		{
			return new Vector(a.x * b.x, a.y * b.y);
		}

		public static Vector operator *(Vector a, int b)
		{
			return new Vector(a.x * b, a.y * b);
		}

		public static Vector operator /(Vector a, int b)
		{
			return new Vector(a.x / b, a.y / b);
		}

		public static bool operator ==(Vector lhs, Vector rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}

		public static bool operator !=(Vector lhs, Vector rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator >=(Vector lhs, Vector rhs)
		{
			return lhs.x >= rhs.x && lhs.y >= rhs.y;
		}

		public static bool operator <=(Vector lhs, Vector rhs)
		{
			return lhs.x <= rhs.x && lhs.y <= rhs.y;
		}

		/// <summary>
		///   <para>Returns true if the objects are equal.</para>
		/// </summary>
		/// <param name="other"></param>
		public override bool Equals(object other)
		{
			if (!(other is Vector))
			{
				return false;
			}
			return Equals((Vector)other);
		}

		public bool Equals(Vector other)
		{
			return x.Equals(other.x) && y.Equals(other.y);
		}

		/// <summary>
		///   <para>Gets the hash code for the Vector2Int.</para>
		/// </summary>
		/// <returns>
		///   <para>The hash code of the Vector2Int.</para>
		/// </returns>
		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2);
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		public override string ToString()
		{
			return string.Format("({0}, {1})", x, y);
		}
	}

	public struct Bound: IEnumerable<Vector>
	{
		private Vector m_RangeX;
		private Vector m_RangeY;

		public Vector this[int index]
        {
            get
            {
				if(index == 0)
                {
					return m_RangeX;
                }
				else if(index == 1)
                {
					return m_RangeY;
                }
                else
                {
					throw new IndexOutOfRangeException($"Invalid Rect index addressed: {index}!");
				}
            }
            set
            {
				if(index == 0)
                {
					m_RangeX = value;
                }
				else if(index == 1)
                {
					m_RangeY = value;
                }
                else
                {
					throw new IndexOutOfRangeException($"Invalid Rect index addressed: {index}!");
				}
            }
        }

		public int this[int index1, int index2]
		{
			get
			{
				return this[index1][index2];
			}
			set
			{
				Vector a = this[index1];
				a[index2] = value;
				this[index1] = a;
			}
		}

		public Bound(Vector start, Vector end)
		{
			m_RangeX = new Vector();
			m_RangeY = new Vector();
			for (int i = 0; i <= 1; i++)
			{
				this[i] = new Vector(start[i], end[i]);
			}
		}

		public void Foreach(Action<Vector> callback)
        {
			Vector item;
            for (int i = m_RangeX.x; i <= m_RangeX.y; i++)
            {
				for (int j = m_RangeY.x; j <= m_RangeY.y; j++)
                {
					item.x = i;
					item.y = j;
					callback(item);
                }
			}
        }

		public bool Contains(Vector v)
        {
			return v.x >= m_RangeX.x && v.x <= m_RangeX.y &&
				   v.y >= m_RangeY.x && v.y <= m_RangeY.y;
		}

        public IEnumerator<Vector> GetEnumerator()
        {
			Vector item;
			for (int i = m_RangeX.x; i <= m_RangeX.y; i++)
			{
				for (int j = m_RangeY.x; j <= m_RangeY.y; j++)
				{
					item.x = i;
					item.y = j;
					yield return item;
				}
			}
		}

        IEnumerator IEnumerable.GetEnumerator()
        {
			Vector item;
			for (int i = m_RangeX.x; i <= m_RangeX.y; i++)
			{
				for (int j = m_RangeY.x; j <= m_RangeY.y; j++)
				{
					item.x = i;
					item.y = j;
					yield return item;
				}
			}
		}
    }

}
