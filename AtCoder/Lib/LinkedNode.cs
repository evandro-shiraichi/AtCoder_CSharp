using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib {

	class LinkedNode<T> : IEnumerable<T> {
		public T Value { get; set; }

		public LinkedNode<T> Next { get; private set; }

		public LinkedNode<T> Previous { get; private set; }

		public LinkedNode(T v) {
			Value = v;
		}

		public void AddAfter(LinkedNode<T> next) {
			if (next.Previous is null == false || Next is null == false) {
				throw new Exception();
			}

			next.Previous = this;
			Next = next;
		}

		public void AddBefore(LinkedNode<T> previous) {
			if (previous.Next is null == false || Previous is null == false) {
				throw new Exception();
			}

			previous.Next = this;
			Previous = previous;
		}

		public void DislinkAfter() {
			if (Next is null == false)
				Next.Previous = null;

			Next = null;
		}

		public void DislinkBefore() {
			if (Previous is null == false)
				Previous.Next = null;

			Previous = null;
		}

		public void Dislink() {
			DislinkAfter();
			DislinkBefore();
		}

		private IEnumerable<T> Enumerate() {
			var statck = new Stack<T>();

			var previous = Previous;

			while (previous is null == false) {
				statck.Push(previous.Value);
				previous = previous.Previous;
			}

			foreach (var temp in statck) {
				yield return temp;
			}

			yield return Value;

			var next = Next;

			while (next is null == false) {
				yield return next.Value;
				next = next.Next;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return Enumerate().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Enumerate().GetEnumerator();
		}
	}
}
