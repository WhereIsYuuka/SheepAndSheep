using System;
using System.Collections.Generic;

public class Pool<T>
{
    readonly Stack<T> _objects = new();

    readonly Func<T> _objectGenerator;
    readonly Action<T> _objectResetter;

    public Pool(Func<T> objectGenerator, Action<T> objectResetter, int inintalCapacity)
    {
        _objectGenerator = objectGenerator;
        _objectResetter = objectResetter;
        for (int i = 0; i < inintalCapacity; i++)
        {
            _objects.Push(_objectGenerator());
        }
    }

    public T GetObject() => _objects.Count == 0 ? _objectGenerator() : _objects.Pop();

    public void ReturnObject(T obj)
    {
        _objectResetter(obj);
        _objects.Push(obj);
    }

    public void Clear() => _objects.Clear();
    public int Count => _objects.Count; 
}