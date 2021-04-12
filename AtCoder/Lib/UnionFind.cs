using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class UnionFindTree
	{
		private readonly int[] data_;

		public int Count => data_.Length;
		public int GroupCount { get; private set; }

		public UnionFindTree(int count)
		{
			data_ = new int[count];
			for (int i = 0; i < count; i++) {
				data_[i] = -1;
			}

			GroupCount = count;
		}

		public int GetSizeOf(int k) => -data_[Find(k)];

		public bool IsUnited(int x, int y) => Find(x) == Find(y);
		public bool Unite(int x, int y)
		{
			x = Find(x);
			y = Find(y);
			if (x == y) {
				return false;
			}

			if (data_[x] > data_[y]) {
				(x, y) = (y, x);
			}

			--GroupCount;
			data_[x] += data_[y];
			data_[y] = x;
			return true;
		}

		public int Find(int k)
		{
			while (data_[k] >= 0) {
				if (data_[data_[k]] >= 0) {
					data_[k] = data_[data_[k]];
				}

				k = data_[k];
			}

			return k;
		}

		public IEnumerable<int> GetAllRoots()
		{
			for (int i = 0; i < data_.Length; i++) {
				if (data_[i] < 0) {
					yield return i;
				}
			}
		}
	}
}
