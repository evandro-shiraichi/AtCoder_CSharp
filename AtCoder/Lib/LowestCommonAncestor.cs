using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class LowestCommonAncestorTree
	{
		private readonly int n_;
		private readonly List<int>[] parents_;
		private readonly List<int>[] edges_;
		private readonly int[] depthes_;
		private readonly int top_;

		public LowestCommonAncestorTree(List<int>[] edges, int top = 0)
		{
			edges_ = edges;
			n_ = edges.Length;
			parents_ = new List<int>[n_];

			for (int i = 0; i < n_; i++)
			{
				parents_[i] = new List<int>();
			}

			depthes_ = new int[n_];
			top_ = top;

			DFS(top_, new List<int>(), new HashSet<int>());
		}

		private void DFS(int node, List<int> route, HashSet<int> yet)
		{
			if (yet.Contains(node))
			{
				return;
			}

			var depth = route.Count;
			depthes_[node] = depth;

			var k = 0;

			while ((1 << k) <= depth)
			{
				var target = 1 << k;
				var t = route[^target];
				parents_[node].Add(t);
				k++;
			}

			route.Add(node);
			yet.Add(node);

			foreach (var next in edges_[node])
			{
				DFS(next, route, yet);
			}

			route.RemoveAt(route.Count - 1);
		}

		public int Query(int node, int n)
		{
			if (n == 0)
			{
				return node;
			}

			var k = GetMaxPow(n);

			return Query(parents_[node][k], n - (1 << k));
		}

		private int GetMaxPow(int n)
        {
			var k = 0;

			while((1 << (k + 1)) <= n)
            {
				k++;
            }

			return k;
        }

		public int GetRoot(int node1, int node2)
		{
			var (a, b) = depthes_[node1] > depthes_[node2]
				? (node2, node1)
				: (node1, node2);

			b = Query(b, depthes_[b] - depthes_[a]);

			if (a == b)
			{
				return a;
			}

			var ng = -1;
			var ok = depthes_[a];

			while (Math.Abs(ng - ok) > 1)
			{
				var c = (ng + ok) / 2;

				if (Query(a, c) == Query(b, c))
				{
					ok = c;
				}
				else
				{
					ng = c;
				}
			}

			return Query(a, ok);
		}

		public int GetDepth(int node)
		{
			return depthes_[node];
		}
	}
}
