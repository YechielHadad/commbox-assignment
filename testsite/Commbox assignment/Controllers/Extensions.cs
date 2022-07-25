using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Commbox.Controllers
{
    public static class JsonExtensions
    {
        public static List<JToken> FindTokens(this JToken containerToken, string name)
        {
            List<JToken> matches = new List<JToken>();
            FindTokens(containerToken, name, matches);
            return matches;
        }

        private static void FindTokens(JToken containerToken, string name, List<JToken> matches)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    if (child.Name == name)
                    {
                        matches.Add(child.Value);
                    }
                    FindTokens(child.Value, name, matches);
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, name, matches);
                }
            }
        }
    }


    public static class ListExt
    {
        /// <summary>
        /// Inserts a new value into a sorted collection.
        /// </summary>
        /// <typeparam name="T">The type of collection values</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="item">The item being inserted</param>
        /// <param name="comparerFunction">An IComparer to comparer T values, e.g. Comparer&lt;T&gt;.Create((x, y) =&gt; (x.Property &lt; y.Property) ? -1 : (x.Property &gt; y.Property) ? 1 : 0)</param>
        public static void InsertSorted<T>(this IList<T> collection, T item, IComparer<T> comparerFunction)
        {
            if (collection.Count == 0)
            {
                collection.Add(item);
            }
            else if (comparerFunction.Compare(item, collection[collection.Count - 1]) >= 0)
            {
                // Add to the end as the item being added is greater than the last item by comparison.
                collection.Add(item);
            }
            else if (comparerFunction.Compare(item, collection[0]) <= 0)
            {
                // Add to the front as the item being added is less than the first item by comparison.
                collection.Insert(0, item);
            }
            else
            {
                // Otherwise, search for the place to insert.
                int index = 0;
                if (collection is List<T>)
                {
                    var list = new List<T>();
                    index = list.BinarySearch(item, comparerFunction);
                }
                else if (collection is T[])
                {
                    var arr = new T[collection.Count];
                    index = Array.BinarySearch(arr, item, comparerFunction);
                }
                else
                {
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (comparerFunction.Compare(collection[i], item) <= 0)
                        {
                            // If the item is the same or before, then the insertion point is here.
                            index = i;
                            break;
                        }

                        // Otherwise loop. We're already tested the last element for greater than count.
                    }
                }

                if (index < 0)
                {
                    // The zero-based index of item if item is found,
                    // otherwise, a negative number that is the bitwise complement of the index of the next element that is larger 
                    //than item or, if there is no larger element, the bitwise complement of Count.
                    index = ~index;
                }

                collection.Insert(index, item);
            }
        }
    }
}