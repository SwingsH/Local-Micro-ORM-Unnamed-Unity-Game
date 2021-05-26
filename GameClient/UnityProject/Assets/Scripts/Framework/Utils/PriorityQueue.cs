using System;
using System.Collections.Generic;

/// <summary>
/// 優先佇列, 不使用 Heap, 使用 C#內建SortedList (tree based too) 
/// 在筆數少的情況下，用SortedList來作效率會較高，筆數高的話則把資料全部插入List後再用Sort排序反而有更好的效率。
/// </summary>
/// <typeparam name="T"></typeparam>
class PriorityQueue<T>
{
    //    public const int MAX_PRIORITY = 100;
    SortedList<Pair<int>, T> _list; //SortedList有禁止 sort key 重複的問題, 因此使用 Pair為 key
    int serialNumber;

    public PriorityQueue()
    {
        serialNumber = 1;
        _list = new SortedList<Pair<int>, T>(new PairComparer<int>()); //建立SortedList並植入比較器PairComparer
    }

    ~PriorityQueue()
    {
        Clear();
    }

    public void Enqueue(T item, int priority)
    {
        _list.Add(new Pair<int>(priority, serialNumber), item);
        ++serialNumber;
    }

    public T Dequeue()
    {
        if (_list.Count == 0)
            return default(T);
        T item = _list[_list.Keys[0]];
        //T item = _list.Values[0];
        _list.RemoveAt(0);
        return item;
    }

    /// <summary>
    /// 移除特定 Value 的元素
    /// </summary>
    public T Dequeue(T specificValue)
    {
        int index = _list.IndexOfValue(specificValue);
        if ((index >= _list.Count) || (index < 0))
            return default(T);
        T item = _list.Values[index];
        _list.RemoveAt(index);
        return item;
    }

    /// <summary>
    /// 佇列中在最前端的元素
    /// </summary>
    public T Front
    {
        get
        {
            try
            {
                if (_list.Count == 0)
                    return default(T);
                return _list[_list.Keys[0]];
                //return _list.Values[0];
            }
            catch
            {
                return default(T);
            }
        }
    }

    public T Item(int index)
    {
        T result;
        if (index >= _list.Count || index < 0)
            return default(T);
        result = _list.Values[index];
        return result;
    }

    /// <summary>
    /// 依據優先度, 重新插入特定 Value 的元素
    /// </summary>
    public void Reinsert(T specificValue, int priority)
    {
        int index = _list.IndexOfValue(specificValue);
        if (index >= _list.Count)
            return;
        T item = _list.Values[index];
        _list.RemoveAt(index);

        Enqueue(item, priority);
    }

    public Pair<int> GetKey(int index)
    {
        Pair<int> result;
        if (index >= _list.Count)
            return default(Pair<int>);
        result = _list.Keys[index];
        return result;
    }

    public int Count
    {
        get { return _list.Count; }
    }

    public void Clear()
    {
        if (_list != null)
            _list.Clear();
    }
}

/// <summary>
/// Pair value, 兩個type相同的值組
/// </summary>
/// <typeparam name="T"></typeparam>
public class Pair<T>
{
    public T First { get; set; }
    public T Second { get; private set; }

    public Pair(T first, T second)
    {
        First = first;
        Second = second;
    }

    public override int GetHashCode()
    {
        return First.GetHashCode() ^ Second.GetHashCode();
    }

    /// <summary>
    /// 兩值是否相等, as Object.Equal
    /// </summary>
    public override bool Equals(object other)
    {
        Pair<T> pair = other as Pair<T>;
        if (pair == null)
        {
            return false;
        }

        return (this.First.Equals(pair.First) && this.Second.Equals(pair.Second));
    }
}

/// <summary>
/// 實作 IComparable的 值組(Pair)比較器
/// </summary>
/// <typeparam name="T"></typeparam>
class PairComparer<T> : IComparer<Pair<T>> where T : IComparable
{
    public int Compare(Pair<T> x, Pair<T> y)
    {
        if (x.First.CompareTo(y.First) < 0) /// x 值小於 y 
            return -1;
        else if (x.First.CompareTo(y.First) > 0) /// x 值大於 y
            return 1;
        else /// x 與 y 相等, 比較第二值, 後進後出
            return x.Second.CompareTo(y.Second);
    }
}