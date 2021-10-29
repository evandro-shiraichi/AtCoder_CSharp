using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib
{
	static class Permutation
	{
		static int[][] permutation;
		static int index = 0;
		public static IEnumerable<int[]> GetAllPermutation(int n)
		{
			var max = 1;
			for (int i = 2; i <= n; i++) {
				max *= i;
			}
			permutation = new int[max][];
			index = 0;
			Permutations(n, new Stack<int>());
			return permutation;
		}

		static void Permutations(int n, Stack<int> s, int now = 0)
		{
			if (s.Count == n) {
				permutation[index++] = s.ToArray();
				return;
			}
			for (int i = 0; i < n; i++) {
				if ((now >> i & 1) != 0) {
					continue;
				}
				s.Push(i);
				Permutations(n, s, now + (1 << i));
				s.Pop();
			}
		}
	}

	static class PermutationsNotSaiki
	{
		public static IEnumerable<IList<int>> GetAllPermutaionsInt(int max) {
			var queue = new Queue<(List<int>, HashSet<int>)>();

			queue.Enqueue((new List<int>(), new HashSet<int>()));

			while (queue.Any()) {
				var (nowPerm, yet) = queue.Dequeue();

				if(nowPerm.Count == max) {
					yield return nowPerm;
					continue;
				}

				for(int i = 0; i < max; i++) {
					if (yet.Contains(i))
						continue;

					var newYet = new HashSet<int>(yet);
					var newList = new List<int>(nowPerm);
					newYet.Add(i);
					newList.Add(i);
					queue.Enqueue((newList, newYet));
				}
			}
		}

		public static IEnumerable<IEnumerable<int>> GetAllPermutationStr(int n)
		{
			var stack = new Stack<(string now, int yet)>();

			stack.Push(("", 0));
			var max = (1 << n) - 1;

			while (stack.Count > 0) {
				var (now, yet) = stack.Pop();

				if (yet == max) {
					yield return now.Select(x => x - '0');
					continue;
				}

				for (int i = 0; i < n; i++) {
					if ((yet >> i & 1) != 0) {
						continue;
					}

					stack.Push((now + i, yet + (1 << i)));
				}
			}
		}

	}
}
