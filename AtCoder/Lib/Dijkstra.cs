using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AtCoder.Lib
{
	class Dijkstra
	{
		public static int[] DoDijkstra(IList<IList<(int to, int cost)>> edges, int start) {
			var maxCosts = Enumerable.Repeat(int.MaxValue, edges.Count).ToArray();

			var pQueue = new PriorityQueue<(int to, int cost), int>(x => x.cost, true);

			pQueue.Enqueue((start, 0));

			while(pQueue.Count > 0) {
				var (now, cost) = pQueue.Dequeue();

				if(maxCosts[now] <= cost) {
					continue;
                }

				maxCosts[now] = cost;

				foreach (var (nextNode, nextCost) in edges[now]) {
					if(nextCost + cost >= maxCosts[nextNode]) {
						continue;
                    }

					pQueue.Enqueue((nextNode, nextCost + cost));
                }
            }

			return maxCosts.Select(x => x == int.MaxValue ? -1 : x).ToArray();
        }
	}
}
