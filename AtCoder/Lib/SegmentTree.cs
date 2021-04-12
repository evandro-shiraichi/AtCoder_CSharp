using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
	class SegmentTree<T>
	{
		private readonly int n_;
		private readonly T def_;
		private readonly T[] data_;
		private readonly Func<T, T, T> operation_;
		private readonly Func<T, T, T> update_;

		public SegmentTree(int size, T def, Func<T, T, T> operation, Func<T, T, T> update)
		{
			def_ = def;
			operation_ = operation;
			update_ = update;
			n_ = 1;

			while (n_ < size) {
				n_ *= 2;
			}

			data_ = Enumerable.Repeat(def, 2 * n_ - 1).ToArray();
		}

		// 場所i(0-indexed)の値をxで更新
		public void Change(int i, T x)
		{
			i += n_ - 1;
			data_[i] = update_(data_[i], x);
			while (i > 0) {
				i = (i - 1) / 2;
				data_[i] = operation_(data_[i * 2 + 1], data_[i * 2 + 2]);
			}
		}

		// [a, b)の区間クエリを実行
		public T Query(int a, int b)
		{
			return QueryCore(a, b, 0, 0, n_);
		}

		// 添字でアクセス
		public T this [int i]
		{
			get { return data_[i + n_ - 1]; }
			set	{ Change(i, value);	}
		}
		private T QueryCore(int a, int b, int k, int l, int r)
		{
			if (r <= a || b <= l) {
				// 交差しない
				return def_;
			}

			if (a <= l && r <= b)
				// a,l,r,bの順で完全に含まれる
				return data_[k]; 
			else {
				T c1 = QueryCore(a, b, 2 * k + 1, l, (l + r) / 2); // 左の子
				T c2 = QueryCore(a, b, 2 * k + 2, (l + r) / 2, r); // 右の子
				return operation_(c1, c2);
			}
		}
	}
}
