using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibraryDZ2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;
        private UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var todoItems = _repository.GetActive(Guid.Parse(currentUser.Id));

            return View(todoItems);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTodoViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Guid userId = await Task.Run(YourAction);
            TodoItem myTodoItem = new TodoItem(model.Text, userId);
            if (ModelState.IsValid)
            {
                _repository.Add(myTodoItem);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SeeCompleted()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var todoItems = _repository.GetCompleted(Guid.Parse(currentUser.Id));

            return View(todoItems);
        }

        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            _repository.MarkAsCompleted(id, Guid.Parse(currentUser.Id));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SeeActive()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var todoItems = _repository.GetActive(Guid.Parse(currentUser.Id));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            _repository.Remove(id, Guid.Parse(currentUser.Id));

            return RedirectToAction("SeeCompleted");
        }

        private async Task<Guid> YourAction()
        {
            // Get currently logged -in user using userManager
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return new Guid(user.Id);

        }

    }
}