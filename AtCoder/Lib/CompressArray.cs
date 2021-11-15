using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {

	class CompressArray<T> : IEnumerable<(long, T)> {
		private readonly SortedSet<long> sortedSet_ = new SortedSet<long>();
		private readonly T default_;
		private Dictionary<int, long> decompressIndexDict_;
		private Dictionary<long, int> compressIndexDict_;

		private T[] array_;

		public int Length {
			get { return array_.Length; }
		}

		public T this[int i] {
			get { return array_[i]; }
			set { array_[i] = value; }
		}

		public CompressArray(IList<long> indexes, T defaultValue = default) {
			default_ = defaultValue;
			foreach (var index in indexes) {
				sortedSet_.Add(index);
			}

			Update();
		}

		public CompressArray(T defaultValue = default) {
			default_ = defaultValue;
		}

		public void AddIndex(long i) {
			sortedSet_.Add(i);
		}

		public void Update() {
			array_ = Enumerable.Repeat(default_, sortedSet_.Count).ToArray();
			compressIndexDict_ = new Dictionary<long, int>();
			decompressIndexDict_ = new Dictionary<int, long>();

			var i = 0;
			foreach (var set in sortedSet_) {
				compressIndexDict_.Add(set, i);
				decompressIndexDict_.Add(i, set);
				i++;
			}
		}

		public int CompressIndex(long index) {
			return compressIndexDict_[index];
		}

		public long DecompressIndex(int index) {
			return decompressIndexDict_[index];
		}

		public IList<T> Decompress(long max = -1) {
			if (max < 0) {
				max = sortedSet_.Last();
			}
			var decompress = Enumerable.Repeat(default_, (int)max).ToArray();

			foreach (var (i, t) in this) {
				decompress[i] = t;
			}

			return decompress;
		}

		public IEnumerator<(long, T)> GetEnumerator() {
			return Enumerable.Range(0, Length).Select(x => (decompressIndexDict_[x], array_[x])).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}

}
