using ClassLibraryDZ2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoSqlRepo
{
    public class TodoSqlRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context) 
        {
            _context = context;
        }

        private TodoItem Get(Guid todoId)
        {
            return _context.Set<TodoItem>().Where(a => a.Id == todoId).FirstOrDefault();
        }

        public TodoItem Get(Guid todoId, Guid userId)
        {
            TodoItem item = _context.Set<TodoItem>().Where(a => a.Id == todoId).FirstOrDefault();

            if (item?.UserId!=userId)
            {
                throw new TodoAccessDeniedException();
            }

            return item;
        }

        public void Add(TodoItem todoItem)
        {
            if (Get(todoItem.Id) != null)
            {
                _context.Set<TodoItem>().Add(todoItem);
            }
            else throw new DuplicateTodoItemException(todoItem.Id);
        }

        public bool Remove(Guid todoId, Guid userId)
        {
            if (Get(todoId) == null) return false;

            TodoItem item = Get(todoId);

            if (item.UserId != userId)
            {
                throw new TodoAccessDeniedException(); //TODO: Customize messages?
            }

            _context.Set<TodoItem>().Remove(item);

            return true;
        }

        public void Update(TodoItem item, Guid userId)
        {
            if (Get(item.Id) != null)
            {
                TodoItem existingItem = Get(item.Id);
                if (existingItem.UserId == userId)
                {
                    existingItem = item;
                }
                else
                {
                    throw new TodoAccessDeniedException();
                }
                return;
            }
            else
            {
                Add(item);
            }
        }

        public bool MarkAsCompleted(Guid todoId, Guid userId)
        {
            if (Get(todoId) == null) return false;
            TodoItem item = Get(todoId);

            if (item.UserId != userId) throw new TodoAccessDeniedException();

            item.DateCompleted = DateTime.Now;
            return true;
        }

        public List<TodoItem> GetAll(Guid userId)
        {
            return _context.Set<TodoItem>().Where(a => a.UserId == userId).OrderByDescending(a=>a.DateCreated).ToList();
        }

        public List<TodoItem> GetActive(Guid userId)
        {
            return GetAll(userId).Where(a => a.DateCompleted == default(DateTime)).ToList();
        }

        public List<TodoItem> GetCompleted(Guid userId)
        {
            return GetAll(userId).Where(a => a.DateCompleted != default(DateTime)).ToList();
        }

        public List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            return GetAll(userId).Where(a => filterFunction(a)).ToList();
        }
    }
}
