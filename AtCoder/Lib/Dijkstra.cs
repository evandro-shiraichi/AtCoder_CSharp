using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AtCoder.Lib
{
	class Dijkstra
	{
		public static void Main(string[] args)
		{
			var nk = Console.ReadLine().Split(" ").Select(x => int.Parse(x)).ToArray();
			var n = nk[0];
			var k = nk[1];

			var nodes = new long[n];

			var to = new Dictionary<int, long>[n];

			for (int i = 0; i < n; i++) {
				to[i] = new Dictionary<int, long>();
			}

			var stringBuilder = new StringBuilder();

			for (int i = 0; i < k; i++) {
				var order = Console.ReadLine().Split(" ").Select(x => int.Parse(x)).ToArray();
				if (order[0] == 0) {
					for (int j = 0; j < n; j++) {
						nodes[j] = long.MaxValue;
					}

					var start = order[1] - 1;
					var end = order[2] - 1;
					var queue = new PriorityQueue<(int to, long cost), long>(a => a.cost, true);

					queue.Enqueue((start, 0));

					while (queue.Count != 0) {
						var temp = queue.Dequeue();

						if (nodes[temp.to] < temp.cost) {
							continue;
						}

						nodes[temp.to] = temp.cost;

						foreach (var t in to[temp.to]) {
							queue.Enqueue((t.Key, t.Value + temp.cost));
						}
					}

					if (nodes[end] != long.MaxValue) {
						stringBuilder.AppendLine(nodes[end].ToString());
					} else {
						stringBuilder.AppendLine("-1");
					}
				} else {
					var c = order[1] - 1;
					var d = order[2] - 1;
					var e = order[3];

					if (to[c].ContainsKey(d) == false || to[c][d] > e) {
						to[c][d] = e;
						to[d][c] = e;
					}
				}
			}

			Console.Write(stringBuilder.ToString());
		}

		public class PriorityQueue<TObject, TPriority> where TPriority : IComparable<TPriority>
		{
			private readonly Func<TObject, TPriority> selector_;
			private readonly int reverseFactory_;
			private TObject[] heap_;
			private TPriority[] priorities_;

			public TObject Top => heap_[0];
			public int Count { get; private set; }

			public PriorityQueue(
				Func<TObject, TPriority> selector,
				bool reverses = false)
				: this(1024, selector, reverses) { }

			public PriorityQueue(
				int capacity,
				Func<TObject, TPriority> selector,
				bool reverses = false)
			{
				heap_ = new TObject[capacity];
				priorities_ = new TPriority[capacity];
				selector_ = selector;
				reverseFactory_ = reverses ? -1 : 1;
			}

			public void Enqueue(TObject item)
			{
				if (heap_.Length == Count) {
					Extend(heap_.Length * 2);
				}

				var priority = selector_(item);
				heap_[Count] = item;
				priorities_[Count] = priority;
				++Count;

				int c = Count - 1;
				while (c > 0) {
					int p = (c - 1) >> 1;
					if (Compare(priorities_[p], priority) < 0) {
						heap_[c] = heap_[p];
						priorities_[c] = priorities_[p];
						c = p;
					} else {
						break;
					}
				}

				heap_[c] = item;
				priorities_[c] = priority;
			}

			public TObject Dequeue()
			{
				TObject ret = heap_[0];
				int n = Count - 1;

				var item = heap_[n];
				var priority = priorities_[n];
				int p = 0;
				int c = (p << 1) + 1;
				while (c < n) {
					if (c != n - 1 && Compare(priorities_[c + 1], priorities_[c]) > 0) {
						++c;
					}

					if (Compare(priority, priorities_[c]) < 0) {
						heap_[p] = heap_[c];
						priorities_[p] = priorities_[c];
						p = c;
						c = (p << 1) + 1;
					} else {
						break;
					}
				}

				heap_[p] = item;
				priorities_[p] = priority;
				Count--;

				return ret;
			}

			public void Clear() => Count = 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private int Compare(TPriority x, TPriority y)
				=> x.CompareTo(y) * reverseFactory_;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void Extend(int newSize)
			{
				var newHeap = new TObject[newSize];
				heap_.CopyTo(newHeap, 0);
				heap_ = newHeap;
				var newPriorities = new TPriority[newSize];
				priorities_.CopyTo(newPriorities, 0);
				priorities_ = newPriorities;
			}
		}
	}
}
