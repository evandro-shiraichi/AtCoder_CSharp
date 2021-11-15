using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {
	class MultiBinaryTree<T> : IEnumerable<T>
		where T : IComparable<T> {
		readonly SortedSet<(T val, long id)> sortedSet_ = new SortedSet<(T, long)>();
		private long id_ = int.MinValue;

		public void Add(T val) {
			sortedSet_.Add((val, id_));
			id_++;
		}

		public void Remove(T val) {
			var min = GetViewBetween(val, val).Min.id;
			sortedSet_.Remove((val, min));
		}

		public bool Contains(T val) {
			return GetViewBetween(val, val).Count > 0;
		}

		public void Clear() {
			sortedSet_.Clear();
		}

		public bool Any() {
			return sortedSet_.Count > 0;
		}

		public SortedSet<(T val, long id)> GetViewBetween(T min, T max) {
			return sortedSet_.GetViewBetween((min, long.MinValue), (max, long.MaxValue));
		}

		public T MaxBetween(T min, T max) {
			return GetViewBetween(min, max).Max.val;
		}

		public T MinBetween(T min, T max) {
			return GetViewBetween(min, max).Min.val;
		}

		public int CountBetween(T min, T max) {
			return GetViewBetween(min, max).Count;
		}

		public T Max => sortedSet_.Max.val;

		public T Min => sortedSet_.Min.val;

		public int Count => sortedSet_.Count;

		public IEnumerator<T> GetEnumerator() {
			return sortedSet_.Select(x => x.val).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return sortedSet_.Select(x => x.val).GetEnumerator();
		}
	}


	class MultiSetBinarySearchTree<T> : SetBinarySearchTree<T> where T : IComparable
	{
		public override void Insert(T v)
		{
			if (_root is null)
			{
				_root = new RandomizedBinarySearchTree<T>.Node(v);
			}
			else
			{
				_root = RandomizedBinarySearchTree<T>.Insert(_root, v);
			}
		}
	}

	class SetBinarySearchTree<T> where T : IComparable
	{
		protected RandomizedBinarySearchTree<T>.Node _root;

		public T this[int index]
		{
			get { return ElementAt(index); }
		}

		public int Count()
		{
			return RandomizedBinarySearchTree<T>.Count(_root);
		}

		public virtual void Insert(T v)
		{
			if (_root is null)
			{
				_root = new RandomizedBinarySearchTree<T>.Node(v);
			}
			else if (RandomizedBinarySearchTree<T>.Contains(_root, v) == false)
			{
				_root = RandomizedBinarySearchTree<T>.Insert(_root, v);
			}
		}

		public void Clear()
		{
			_root = null;
		}

		public void Remove(T v)
		{
			_root = RandomizedBinarySearchTree<T>.Remove(_root, v);
		}

		public bool Contains(T v)
		{
			return RandomizedBinarySearchTree<T>.Contains(_root, v);
		}

		public T ElementAt(int k)
		{
			var node = RandomizedBinarySearchTree<T>.FindByIndex(_root, k);

			if (node is null)
			{
				throw new IndexOutOfRangeException();
			}

			return node.Value;
		}

		public int Count(T v)
		{
			return UpperBound(v) - LowerBound(v);
		}

		public int LowerBound(T v)
		{
			return RandomizedBinarySearchTree<T>.LowerBound(_root, v);
		}

		public int UpperBound(T v)
		{
			return RandomizedBinarySearchTree<T>.UpperBound(_root, v);
		}

		public (int, int) EqualRange(T v)
		{
			if (Contains(v) == false)
			{
				return (-1, -1);
			}

			return (LowerBound(v), UpperBound(v) - 1);
		}

		public IEnumerable<T> ToIEnumerable()
		{
			return RandomizedBinarySearchTree<T>.Enumerate(_root);
		}
	}

	class RandomizedBinarySearchTree<T> where T : IComparable
	{
		public class Node
		{
			public T Value;
			public Node LeftChild;
			public Node RightChild;
			public int Count;

			public Node(T v)
			{
				Value = v;
				Count = 1;
			}
		}

		static Random random_ = new Random();

		public static int Count(Node t)
		{
			return t is null ? 0 : t.Count;
		}

		static Node Update(Node t)
		{
			t.Count = Count(t.LeftChild) + Count(t.RightChild) + 1;
			return t;
		}

		public static Node Merge(Node left, Node right)
		{
			if (left is null || right is null)
			{
				return left ?? right;
			}

			if ((double)Count(left) / (Count(left) + Count(right)) > random_.NextDouble())
			{
				left.RightChild = Merge(left.RightChild, right);
				return Update(left);
			}
			else
			{
				right.LeftChild = Merge(left, right.LeftChild);
				return Update(right);
			}
		}

		/// <summary>
		/// split as [0, k), [k, n)
		/// </summary>
		public static (Node, Node) Split(Node t, int k)
		{
			if (t is null)
			{
				return (null, null);
			}

			if (k <= Count(t.LeftChild))
			{
				var (s1, s2) = Split(t.LeftChild, k);
				t.LeftChild = s2;
				return (s1, Update(t));
			}
			else
			{
				var (s1, s2) = Split(t.RightChild, k - Count(t.LeftChild) - 1);
				t.RightChild = s1;
				return (Update(t), s2);
			}
		}

		public static Node Remove(Node t, T v)
		{
			if (Find(t, v) is null)
			{
				return t;
			}

			return RemoveAt(t, LowerBound(t, v));
		}

		public static Node RemoveAt(Node t, int k)
		{
			var (s1, s2) = Split(t, k);
			var (_, s4) = Split(s2, 1);

			return Merge(s1, s4);
		}

		public static bool Contains(Node t, T v)
		{
			return Find(t, v) is null == false;
		}

		public static Node Find(Node t, T v)
		{
			while (t is null == false)
			{
				var cmp = t.Value.CompareTo(v);
				if (cmp > 0)
				{
					t = t.LeftChild;
				}
				else if (cmp < 0)
				{
					t = t.RightChild;
				}
				else
				{
					break;
				}
			}

			return t;
		}

		public static Node FindByIndex(Node t, int index)
		{
			if (t is null)
			{
				return null;
			}

			var currentIndex = Count(t) - Count(t.RightChild) - 1;
			while (t is null == false)
			{
				if (currentIndex == index)
				{
					return t;
				}

				if (currentIndex > index)
				{
					t = t.LeftChild;
					currentIndex -= (Count(t?.RightChild) + 1);
				}
				else
				{
					t = t.RightChild;
					currentIndex += (Count(t?.LeftChild) + 1);
				}
			}

			return null;
		}

		public static int UpperBound(Node t, T v)
		{
			var tOriginal = t;
			if (t is null)
			{
				return -1;
			}

			var ret = int.MaxValue;
			var index = Count(t) - Count(t.RightChild) - 1;

			while (t is null == false)
			{
				var cmp = t.Value.CompareTo(v);

				if (cmp > 0)
				{
					ret = Math.Min(ret, index);
					t = t.LeftChild;
					index -= (Count(t?.RightChild) + 1);
				}
				else
				{
					t = t.RightChild;
					index += (Count(t?.LeftChild) + 1);
				}
			}

			return ret == int.MaxValue ? Count(tOriginal) : ret;
		}

		public static int LowerBound(Node t, T v)
		{
			var tOriginal = t;
			if (t is null)
			{
				return -1;
			}

			var ret = int.MaxValue;
			var index = Count(t) - Count(t.RightChild) - 1;

			while (t is null == false)
			{
				var cmp = t.Value.CompareTo(v);

				if (cmp >= 0)
				{
					t = t.LeftChild;

					if (cmp == 0 || t is null)
					{
						ret = Math.Min(ret, index);
					}

					if (t is null == false)
					{
						index -= (Count(t?.RightChild) + 1);
					}
				}
				else
				{
					t = t.RightChild;
					index += (Count(t?.LeftChild) + 1);

					if (t is null)
					{
						return index;
					}
				}
			}

			return ret == int.MaxValue ? Count(tOriginal) : ret;
		}

		public static Node Insert(Node t, T v)
		{
			var ub = LowerBound(t, v);
			return InsertByIndex(t, ub, v);
		}

		static Node InsertByIndex(Node t, int k, T v)
		{
			var (s1, s2) = Split(t, k);
			return Merge(Merge(s1, new Node(v)), s2);
		}

		public static IEnumerable<T> Enumerate(Node t)
		{
			var ret = new List<T>();
			Enumerate(t, ret);
			return ret;
		}

		static void Enumerate(Node t, List<T> ret)
		{
			if (t is null)
			{
				return;
			}

			Enumerate(t.LeftChild, ret);
			ret.Add(t.Value);
			Enumerate(t.RightChild, ret);
		}
	}
}
