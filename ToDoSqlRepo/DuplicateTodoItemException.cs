using System;

namespace ClassLibraryDZ2
{
    public class DuplicateTodoItemException : Exception
    {
        public DuplicateTodoItemException(Guid id) : base("duplicate item: " + id.ToString())
        {

        }
    }
}
