using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {
	class RangeScheduling<T> where T : IComparable<T> {
		private List<(T start, T end)> list_ = new List<(T start, T end)>();

		public RangeScheduling() : this(new List<(T, T)>()) { }

		public RangeScheduling(IList<(T start, T end)> list) {
			list_ = list.ToList();
		}

		public void Add(T start, T end) {
			list_.Add((start, end));
		}

		public void Add((T start, T end) tuple) {
			list_.Add(tuple);
		}

		public IEnumerable<(T start, T end)> Calculate(bool includingBorder = true) {
			var border = includingBorder ? 0 : 1;
			list_.Sort((x, y) => x.end.CompareTo(y.end));

			var (last, _) = list_.FirstOrDefault();

			foreach (var (start, end) in list_) {
				if (start.CompareTo(last) >= border) {
					yield return (start, end);
					last = end;
				}
			}
		}
	}

}
