using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public class PriorityQueue<T> {
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public void Enqueue(T item, float priority) {
        elements.Add(new KeyValuePair<T, float>(item, priority));
        elements = elements.OrderBy(pair => pair.Value).ToList();
    }

    public T Dequeue() {
        if (elements.Count == 0) {
            throw new InvalidOperationException("The queue is empty.");
        }

        var item = elements[0].Key;
        elements.RemoveAt(0);
        return item;
    }

    public T Peek() {
        if (elements.Count == 0) {
            throw new InvalidOperationException("The queue is empty.");
        }

        return elements[0].Key;
    }

    public int Count => elements.Count;

    public bool IsEmpty() => Count == 0;

}