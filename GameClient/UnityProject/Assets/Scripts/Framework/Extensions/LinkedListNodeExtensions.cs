using System.Collections.Generic;

namespace Tizsoft.Extensions
{
    public static class LinkedListNodeExtensions
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> node)
        {
            if (node == null) return null;
            return node.Next ?? node.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> node)
        {
            if (node == null) return null;
            return node.Previous ?? node.List.Last;
        }
    }
}
