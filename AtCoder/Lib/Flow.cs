using AtCoder.ABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {

	class Flow<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		static readonly IMathArithmeticOperator<T> math_ = IMathArithmeticOperator<T>.GetOperator();

		public Flow(int node_size) {
			V = node_size;
			G = Enumerable.Repeat(0, V).Select(_ => new List<Edge>()).ToList();
			level = Enumerable.Repeat(0, V).ToList();
			iter = Enumerable.Repeat(0, V).ToList();
		}

		private class Edge {
			public Edge(int to, T cap, int rev) {
				To = to;
				Cap = cap;
				Rev = rev;
			}
			public int To { get; set; }
			public T Cap { get; set; }
			public int Rev { get; set; }
		}

		private readonly List<List<Edge>> G;
		private readonly int V;
		private List<int> level;
		private List<int> iter;

		public void AddEdge(int from, int to, T cap) {
			G[from].Add(new Edge(to, cap, G[to].Count));
			G[to].Add(new Edge(from, math_.Zero, G[from].Count - 1));
		}

		public T MaxFlow(int s, int t) {
			T flow = math_.Zero;
			while (true) {
				BFS(s);
				if (level[t] < 0) { return flow; }
				iter = Enumerable.Repeat(0, V).ToList();
				var f = DFS(s, t, math_.MaxValue);
				while (f.CompareTo(math_.Zero) > 0) {
					flow = math_.Add(flow, f);
					f = DFS(s, t, math_.MaxValue);
				}
			}
		}

		private void BFS(int s) {
			level = Enumerable.Repeat(-1, V).ToList();
			level[s] = 0;
			var que = new Queue<int>();
			que.Enqueue(s);
			while (que.Count != 0) {
				var v = que.Dequeue();
				for (int i = 0; i < G[v].Count; i++) {
					var e = G[v][i];
					if (e.Cap.CompareTo(math_.Zero) > 0 && level[e.To] < 0) {
						level[e.To] = level[v] + 1;
						que.Enqueue(e.To);
					}
				}
			}
		}

		private T DFS(int v, int t, T f) {
			if (v == t)
				return f;
			for (int i = iter[v]; i < G[v].Count; i++) {
				iter[v] = i;
				var e = G[v][i];
				if (e.Cap.CompareTo(math_.Zero) > 0 && level[v] < level[e.To]) {
					var d = DFS(e.To, t, math_.Min(f, e.Cap));
					if (d.CompareTo(math_.Zero) > 0) {
						e.Cap = math_.Substract(e.Cap, d);
						G[e.To][e.Rev].Cap = math_.Add(G[e.To][e.Rev].Cap, d);
						return d;
					}
				}
			}
			return math_.Zero;
		}
	}

}
