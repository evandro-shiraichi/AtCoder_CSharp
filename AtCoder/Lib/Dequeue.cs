using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class Dequeue<T>
	{
		private int _capacity;
		private T[] _array;

		private int _firstIndex = 0;
		private int _lastIndex = 1;

		public Dequeue(int capacity = 16)
		{
			_capacity = capacity;
			_array = new T[_capacity];
		}

		public T this[int i]
		{
			get
			{
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException();
				return _array[ToIndex(_firstIndex + 1 + i)];
			}
			set
			{
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException();
				_array[ToIndex(_firstIndex + 1 + i)] = value;
			}
		}

		public int Count
		{
			get { return _lastIndex - _firstIndex - 1; }
		}

		public bool Any()
		{
			return Count > 0;
		}

		private int ToIndex(int index)
		{
			index %= _capacity;
			if (index < 0)
				index += _capacity;
			return index;
		}

		public void PushBack(T data)
		{
			if (_capacity == Count)
				Resize();

			_array[ToIndex(_lastIndex++)] = data;
		}

		public void PushFront(T data)
		{
			if (_capacity == Count)
				Resize();

			_array[ToIndex(_firstIndex--)] = data;
		}

		public T PopBack()
		{
			if (Any() == false)
				throw new InvalidOperationException();

			var ret = _array[ToIndex(_lastIndex - 1)];
			_lastIndex--;
			return ret;
		}

		public T PopFront()
		{
			if (Any() == false)
				throw new InvalidOperationException();

			var ret = _array[ToIndex(_firstIndex + 1)];
			_firstIndex++;
			return ret;
		}

		private void Resize()
		{
			var newArray = new T[_capacity * 2];

			for (int i = _firstIndex; i < _lastIndex - 1; i++)
			{
				var index = i - _firstIndex;
				newArray[index] = _array[ToIndex(i + 1)];
			}
			_firstIndex = -1;
			_lastIndex = _capacity;
			_capacity *= 2;
			_array = newArray;
		}
	}
}
