using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PriorityQueue<T> {
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public void Enqueue(T item, float priority) {
        elements.Add(new KeyValuePair<T, float>(item, priority));
        elements = elements.OrderByDescending(pair => pair.Value).ToList();
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

    public void Reverse() {
        for (int i = 0; i < elements.Count; i++)
        {
            var item = elements[i].Key;
            var invertedPriority = elements[i].Value * -1;
            elements[i] = new KeyValuePair<T, float>(item, invertedPriority);
        }

        elements.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
    }

    public List<T> ToList() {
        List<T> result = new List<T>();
        foreach (KeyValuePair<T, float> pair in elements) {
            result.Add(pair.Key);
        }
        return result;
    }

    public void PrintElements() {
        Debug.Log("Queue Contents:");
        foreach (var element in elements) {
            Debug.Log($"Item: {element.Key}, Priority: {element.Value}");
        }
    }
}