using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
	class PriorityQueue<TObject, TPriority> where TPriority : IComparable<TPriority>
	{
		private readonly Func<TObject, TPriority> selector_;
		private readonly int reverseFactor;
		private TObject[] heap_;
		int size;

		public TObject Top => heap_[0];
		public int Count
		{
			get { return size; }
		}

		public PriorityQueue(
			Func<TObject, TPriority> selector,
			bool isReverse = false)
			: this(1024, selector, isReverse) { }

		public PriorityQueue(
			int capacity,
			Func<TObject, TPriority> selector,
			bool isReverse = false)
		{
			heap_ = new TObject[capacity];
			selector_ = selector;
			reverseFactor = isReverse ? -1 : 1;
		}

		public void Enqueue(TObject x)
		{
			if (heap_.Length == size) {
				Extend(size * 2);
			}

			int i = size++;

			while (i > 0) {
				int p = (i - 1) / 2;

				if (reverseFactor * selector_(heap_[p]).CompareTo(selector_(x)) >= 0) {
					break;
				}

				heap_[i] = heap_[p];
				i = p;
			}

			heap_[i] = x;
		}

		public TObject Deque()
		{
			TObject ret = heap_[0];

			TObject x = heap_[--size];

			int i = 0;
			while (i * 2 + 1 < size) {
				int a = i * 2 + 1;
				int b = i * 2 + 2;

				if (b < size && reverseFactor * selector_(heap_[b]).CompareTo(selector_(heap_[a])) > 0) {
					a = b;
				}

				if (reverseFactor * selector_(heap_[a]).CompareTo(selector_(x)) <= 0) {
					break;
				}

				heap_[i] = heap_[a];
				i = a;
			}

			heap_[i] = x;

			return ret;
		}

		void Extend(int newSize)
		{
			var newHeap = new TObject[newSize];

			heap_.CopyTo(newHeap, 0);
			heap_ = newHeap;
		}
	}

	class PriorityQueue<T> where T : IComparable<T>
	{
		T[] heap_;
		int size;
		int reverseFactor;

		public T Top
		{
			get { return heap_[0]; }
		}

		public int Count
		{
			get { return size; }
		}

		public PriorityQueue(bool isReverse = false)
			: this(1024, isReverse)
		{
		}

		public PriorityQueue(int capacity, bool isReverse = false)
		{
			heap_ = new T[capacity];
			size = 0;
			reverseFactor = isReverse ? -1 : 1;
		}

		public void Enqueue(T x)
		{
			if (heap_.Length == size) {
				Extend(size * 2);
			}

			int i = size++;

			while (i > 0) {
				int p = (i - 1) / 2;

				if (reverseFactor * heap_[p].CompareTo(x) >= 0) {
					break;
				}

				heap_[i] = heap_[p];
				i = p;
			}

			heap_[i] = x;
		}

		public T Deque()
		{
			T ret = heap_[0];

			T x = heap_[--size];

			int i = 0;
			while (i * 2 + 1 < size) {
				int a = i * 2 + 1;
				int b = i * 2 + 2;

				if (b < size && reverseFactor * heap_[b].CompareTo(heap_[a]) > 0) {
					a = b;
				}

				if (reverseFactor * heap_[a].CompareTo(x) <= 0) {
					break;
				}

				heap_[i] = heap_[a];
				i = a;
			}

			heap_[i] = x;

			return ret;
		}

		void Extend(int newSize)
		{
			var newHeap = new T[newSize];

			heap_.CopyTo(newHeap, 0);
			heap_ = newHeap;
		}
	}
}
