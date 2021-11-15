using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {

	class PermutationsStr : Permutations<char> {
		public PermutationsStr(string str) : base(str.ToArray()) { }

		public new IEnumerable<string> Enumerate() {
			foreach (var array in base.Enumerate()) {
				yield return new string(array);
			}
		}

		public new IEnumerable<string> EnumerateFromNow() {
			foreach (var array in base.EnumerateFromNow()) {
				yield return new string(array);
			}
		}
	}

	class Permutations<T> where T : IComparable<T> {
		private T[] initialArray_;
		private T[] array_;

		public Permutations(T[] array) {
			array_ = new T[array.Length];
			initialArray_ = new T[array.Length];
			Array.Copy(array, array_, array.Length);
			Array.Copy(array_, initialArray_, array_.Length);
		}

		public void Reset() {
			Array.Copy(initialArray_, array_, initialArray_.Length);
		}

		private bool Next() {
			while (true) {
				var left = -1;

				for (int i = array_.Length - 1; i > 0; i--) {
					if (array_[i - 1].CompareTo(array_[i]) < 0) {
						left = i - 1;
						break;
					}
				}

				if (left < 0)
					return false;

				var right = -1;

				for (int i = array_.Length - 1; i > left; i--) {
					if (array_[i].CompareTo(array_[left]) > 0) {
						right = i;
						break;
					}
				}

				(array_[right], array_[left]) = (array_[left], array_[right]);

				array_[(left + 1)..].AsSpan().Reverse();

				return true;
			}
		}

		public IEnumerable<T[]> Enumerate() {
			Array.Sort(array_);

			do {
				yield return array_;
			} while (Next());
		}

		public IEnumerable<T[]> EnumerateFromNow() {
			do {
				yield return array_;
			} while (Next());
		}
	}

}
