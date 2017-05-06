using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Linked list based on built in C# .NET one
// Additions: Get object by index, like array access!
public class CSLinkedList<T> : LinkedList<T> {
	// Get by index
	public T this[int nIndex] {
		get {
			if (nIndex >= Count) {
				return Last.Value;
			} else if (nIndex < 0) {
				return First.Value;
			}

			// Loop through and find the one with the index
			// If we're past the halfway point, iterate backwards!
			LinkedListNode<T> oCurrent;
			if (nIndex > Count / 2) {
				oCurrent = Last;
				for (int i = Count; i > nIndex; i--) {
					oCurrent = oCurrent.Previous;
				}
			} else {
				oCurrent = First;
				for (int i = 0; i < nIndex; i++) {
					oCurrent = oCurrent.Next;
				}
			}

			return oCurrent.Value;
		}

		set {
			if (nIndex >= Count) {
				Last.Value = value;
				return;
			} else if (nIndex < 0) {
				First.Value = value;
				return;
			}

			// Loop through and find the one with the index
			// If we're past the halfway point, iterate backwards!
			LinkedListNode<T> oCurrent;
			if (nIndex > Count / 2) {
				oCurrent = Last;
				for (int i = 0; i < nIndex; i++) {
					oCurrent = oCurrent.Previous;
				}
			} else {
				oCurrent = First;
				for (int i = 0; i < nIndex; i++) {
					oCurrent = oCurrent.Next;
				}
			}

			oCurrent.Value = value;
		}
	}

	public LinkedListNode<T> GetByIndex(int nIndex) {
		if (nIndex >= Count) {
			return Last;
		} else if (nIndex < 0) {
			return First;
		}

		// Loop through and find the one with the index
		// If we're past the halfway point, iterate backwards!
		LinkedListNode<T> oCurrent;
		if (nIndex > Count / 2) {
			oCurrent = Last;
			for (int i = Count; i > nIndex; i--) {
				oCurrent = oCurrent.Previous;
			}
		} else {
			oCurrent = First;
			for (int i = 0; i < nIndex; i++) {
				oCurrent = oCurrent.Next;
			}
		}

		return oCurrent;
	}

	public void RemoveLastExceptFinal() {
		if (Count <= 1) {
			return;
		}

		RemoveLast();
	}

	public void RemoveLast(int nHowMany, bool bRemoveFinal = true) {
		for(int i = 0; i < nHowMany; i++) {
			if(bRemoveFinal) {
				RemoveLast();
			} else {
				RemoveLastExceptFinal();
			}
		}
	}

	public int LastIdx {
		get {
			return Count - 1;
		}
	}
}