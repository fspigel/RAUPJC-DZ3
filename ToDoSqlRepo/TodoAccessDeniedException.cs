using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoSqlRepo
{
    public class TodoAccessDeniedException : Exception
    {
        public TodoAccessDeniedException() : base("Access Denied (user ID does not match owner ID)")
        {

        }
    }
}
