using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents a read-only collection of match groups.
    /// </summary>
    public class GroupCollection : IReadOnlyList<Group>
    {
        Group[] _groups;
        int _index;

        /// <summary>
        /// Get the match group at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Group this[int index] => _groups[index];

        /// <summary>
        /// Get the total number of groups in this collection.
        /// </summary>
        public int Count => _groups.Length;
        
        internal GroupCollection(int capacity)
        {
            _groups = new Group[capacity];
        }

        internal void Add(Group g)
        {
            _groups[_index++] = g;
        }

        public IEnumerator<Group> GetEnumerator()
        {
            return ((IEnumerable<Group>)_groups).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _groups.GetEnumerator();
        }
    }
}
