using AtCoder.ABC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib {

	class SearchMinDistanceGraph<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		private static IMathArithmeticOperator<T> op_ = IMathArithmeticOperator<T>.GetOperator();
		private readonly int n_;

		private Dictionary<int, T>[] edges_;

		public SearchMinDistanceGraph(int n) {
			n_ = n;
			edges_ = new int[n_].Select(_ => new Dictionary<int, T>()).ToArray();
		}

		public void AddEdgeTwoWay(int a, int b, T cost) {
			AddEdgeOneWay(a, b, cost);
			AddEdgeOneWay(b, a, cost);
		}

		public void AddEdgeOneWay(int from, int to, T cost) {
			edges_[from][to] = cost;
		}

		public void RemoveEdgeTwoWay(int a, int b) {
			RemoveEdgeOneWay(a, b);
			RemoveEdgeOneWay(b, a);
		}

		public void RemoveEdgeOneWay(int a, int b) {
			if (edges_[a].ContainsKey(b))
				edges_[a].Remove(b);
		}

		public IList<int> GetMinDistanceRoute(int goal, int[] routes) {
			var stack = new Stack<int>();
			stack.Push(goal);

			var previous = routes[goal];

			while (previous != -1) {
				stack.Push(previous);
				previous = routes[previous];
			}

			return stack.ToArray();
		}

		public (T[] minDist, int[] route) DoDijkstra(int start) {
			var distances = Enumerable.Repeat(op_.MaxValue, n_).ToArray();
			var isDetermined = Enumerable.Repeat(-1, n_).ToArray();
			var edges = Enumerable.Range(0, n_).Select(x => new Dictionary<int, T>(edges_[x])).ToArray();
			var pQueue = new PriorityQueue<(int to, int from, T cost), T>(x => x.cost, true);

			pQueue.Enqueue((start, start, op_.Zero));
			distances[start] = op_.Zero;

			var count = 0;

			while (pQueue.Count > 0) {
				var (now, from, cost) = pQueue.Dequeue();

				if (isDetermined[now] != -1)
					continue;

				isDetermined[now] = from;

				// ノードがN回更新(取り出)されたらすでに終わっているはず
				if (n_ == count++)
					break;

				foreach (var (next, edgeCost) in edges[now]) {
					var nextCost = op_.Add(cost, edgeCost);
					// 確定ノード、またはすでにコストが現在を下回っているノードは追加しない
					if (isDetermined[next] != -1 || distances[next].CompareTo(nextCost) <= 0)
						continue;

					pQueue.Enqueue((next, now, nextCost));
					distances[next] = nextCost;
				}
			}

			isDetermined[start] = -1;
			return (distances, isDetermined);
		}
	}

	class DoDijkstraClass {
		long[] DoDijkstra(List<(int To, int Cost)>[] edg, int start) {
			var minCosts = Enumerable.Repeat(long.MaxValue, edg.Length).ToArray();

			minCosts[start] = 0;

			var pQueue = new PriorityQueue<(int To, long Cost), long>(x => x.Cost, true);
			pQueue.Enqueue((start, 0L));

			while (pQueue.Count > 0) {
				var (now, cost) = pQueue.Dequeue();

				foreach (var (next, nextCost) in edg[now]) {
					var newCost = cost + nextCost;
					if (minCosts[next] > newCost) {
						pQueue.Enqueue((next, newCost));
						minCosts[next] = newCost;
					}
				}
			}

			return minCosts;
		}
	}

	class PriorityQueue<TObject, TPriority> where TPriority : IComparable<TPriority> {
		private readonly Func<TObject, TPriority> selector_;
		private readonly int reverseFactor;
		private TObject[] heap_;
		int size;

		public TObject Top => heap_[0];
		public int Count {
			get { return size; }
		}

		public PriorityQueue(
			Func<TObject, TPriority> selector,
			bool isReverse = false)
			: this(1024, selector, isReverse) { }

		public PriorityQueue(
			int capacity,
			Func<TObject, TPriority> selector,
			bool isReverse = false) {
			heap_ = new TObject[capacity];
			selector_ = selector;
			reverseFactor = isReverse ? -1 : 1;
		}

		public void Enqueue(TObject x) {
			if (heap_.Length == size) {
				Extend(size * 2);
			}

			int i = size++;

			while (i > 0) {
				int p = (i - 1) / 2;

				if (reverseFactor * selector_(heap_[p]).CompareTo(selector_(x)) >= 0) {
					break;
				}

				heap_[i] = heap_[p];
				i = p;
			}

			heap_[i] = x;
		}

		public TObject Dequeue() {
			TObject ret = heap_[0];

			TObject x = heap_[--size];

			int i = 0;
			while (i * 2 + 1 < size) {
				int a = i * 2 + 1;
				int b = i * 2 + 2;

				if (b < size && reverseFactor * selector_(heap_[b]).CompareTo(selector_(heap_[a])) > 0) {
					a = b;
				}

				if (reverseFactor * selector_(heap_[a]).CompareTo(selector_(x)) <= 0) {
					break;
				}

				heap_[i] = heap_[a];
				i = a;
			}

			heap_[i] = x;

			return ret;
		}

		void Extend(int newSize) {
			var newHeap = new TObject[newSize];

			heap_.CopyTo(newHeap, 0);
			heap_ = newHeap;
		}
	}

	class PriorityQueue<T> where T : IComparable<T> {
		T[] heap_;
		int size;
		int reverseFactor;

		public T Top {
			get { return heap_[0]; }
		}

		public int Count {
			get { return size; }
		}

		public PriorityQueue(bool isReverse = false)
			: this(1024, isReverse) {
		}

		public PriorityQueue(int capacity, bool isReverse = false) {
			heap_ = new T[capacity];
			size = 0;
			reverseFactor = isReverse ? -1 : 1;
		}

		public void Enqueue(T x) {
			if (heap_.Length == size) {
				Extend(size * 2);
			}

			int i = size++;

			while (i > 0) {
				int p = (i - 1) / 2;

				if (reverseFactor * heap_[p].CompareTo(x) >= 0) {
					break;
				}

				heap_[i] = heap_[p];
				i = p;
			}

			heap_[i] = x;
		}

		public T Deque() {
			T ret = heap_[0];

			T x = heap_[--size];

			int i = 0;
			while (i * 2 + 1 < size) {
				int a = i * 2 + 1;
				int b = i * 2 + 2;

				if (b < size && reverseFactor * heap_[b].CompareTo(heap_[a]) > 0) {
					a = b;
				}

				if (reverseFactor * heap_[a].CompareTo(x) <= 0) {
					break;
				}

				heap_[i] = heap_[a];
				i = a;
			}

			heap_[i] = x;

			return ret;
		}

		void Extend(int newSize) {
			var newHeap = new T[newSize];

			heap_.CopyTo(newHeap, 0);
			heap_ = newHeap;
		}
	}
}
