namespace TrakHound.Builder
{
    /// <summary>
    /// Collection-related extension helpers used by the release-builder
    /// script.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }

            if (typeof(IList<T>).IsAssignableFrom(enumerable.GetType()))
            {
                return ((IList<T>)enumerable).Count < 1;
            }

            var a = enumerable as Array;
            if (a != null)
            {
                return a.Length < 1;
            }

            //foreach (var debug in enumerable)
            //{
            //    if (debug == null)
            //    {
            //        if (debug != null)
            //        {

            //        }
            //    }
            //}

            return !enumerable.Any();
        }

        /// <summary>
        /// Returns the input sequence with duplicate strings removed,
        /// preserving the original insertion order.
        /// </summary>
        /// <param name="strings">Input strings, possibly null.</param>
        /// <returns>The distinct sequence, or the original sequence
        /// when it is null or empty.</returns>
        public static IEnumerable<string> ToDistinct(this IEnumerable<string> strings)
        {
            if (!strings.IsNullOrEmpty())
            {
                var x = new HashSet<string>();
                foreach (var s in strings) if (!x.Contains(s)) x.Add(s);
                return x;
            }

            return strings;
        }
    }
}
