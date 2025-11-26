using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test6_PriorityQueue<T>
{
    private List<(T item, float priority)> heap = new List<(T, float)>();
    public int Count => heap.Count;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }
    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T rootItem = heap[0].item;
        heap[0] = heap[^1];          //뒤에서 첫번태 요소를 루트로 이동
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return rootItem;
    }

    public T Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        return heap[0].item;
    }

    public bool Contains(T item)
    {
        foreach (var h in heap)
            if(h.Equals(item)) return true;

        return false;
    }

    private void HeapifyUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority >= heap[parent].priority)
                break;
            (heap[parent], heap[i]) = (heap[i], heap[parent]);
            i = parent;
        }
    }

    private void HeapifyDown(int i)
    {
        int last = heap.Count - 1;
        while (true)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int smallest = i;

            if (left <= last && heap[smallest].priority > heap[left].priority)
                smallest = left;
            if (right <= last && heap[smallest].priority > heap[right].priority)
                smallest = right;

            if (smallest == i) break;
            (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
            i = smallest;
        }
    }
}
