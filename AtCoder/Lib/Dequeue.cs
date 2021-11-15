using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib {
	class Dequeue<T> {
		private T[] array_;

		public int Count { get; private set; }

		private int left_ = 0;
		private int right_ = 0;

		public Dequeue(int capa = 4) {
			array_ = new T[capa];
		}

		public T this[int i] {
			get {
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException();

				return array_[(left_ + i) % array_.Length];
			}

			set {
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException();

				array_[(left_ + i) % array_.Length] = value;
			}
		}

		public void PushLeft(T val) {
			left_--;

			if (left_ < 0)
				left_ += array_.Length;

			array_[left_] = val;
			Count++;

			if (array_.Length == Count)
				Resize();
		}

		public void PushRight(T val) {
			array_[right_] = val;
			Count++;

			right_++;
			right_ %= array_.Length;

			if (array_.Length == Count)
				Resize();
		}

		public T PopLeft() {
			if (Count <= 0)
				throw new Exception();

			var ret = array_[left_];
			array_[left_] = default;
			left_++;
			left_ %= array_.Length;
			Count--;

			if (Count == 0)
				left_ = right_ = 0;

			return ret;
		}

		public T PopRight() {
			if (Count <= 0)
				throw new Exception();

			right_--;

			if (right_ < 0)
				right_ += array_.Length;

			var ret = array_[right_];
			array_[right_] = default;
			Count--;

			if (Count == 0)
				left_ = right_ = 0;

			return ret;
		}

		private void Resize() {
			var temp = new T[array_.Length * 2];

			for (int i = 0; i < Count; i++) {
				temp[i] = this[i];
			}

			array_ = temp;
			left_ = 0;
			right_ = Count;
		}

		public IEnumerable<T> ToIEnumerable() {
			for (int i = 0; i < Count; i++) {
				yield return this[i];
			}
		}
	}

}
