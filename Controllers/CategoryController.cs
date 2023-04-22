using Bulky.Models;
using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.DataAccess.Repository;

namespace BulkyBook.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitofWork _unitWork;
        public CategoryController(IUnitofWork unitWork)
        {
            _unitWork = unitWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = _unitWork.CategoryRepository.GetAll().ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name","Name and display order can't be same");
            }
            if (ModelState.IsValid)
            {
                _unitWork.CategoryRepository.Add(obj);
                _unitWork.Save();
                TempData["success"] = "Category successfully added";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Category can not be created";
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? obj = _unitWork.CategoryRepository.Get(x=>x.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            
            if (ModelState.IsValid)
            {
                _unitWork.CategoryRepository.Update(obj);
                _unitWork.Save();
                TempData["success"] = "Category successfully edited";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Category can not be edited";
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? obj = _unitWork.CategoryRepository.Get(x => x.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if(id == null || id == 0)
            {
                TempData["error"] = "Category can not be deleted";
                return NotFound();
            }

            Category? obj = _unitWork.CategoryRepository.Get(x => x.Id == id);

            if(obj == null)
            {
                TempData["error"] = "Category can not be deleted";
                return RedirectToAction("Index");
            }

            _unitWork.CategoryRepository.Remove(obj);
            _unitWork.Save();
            TempData["success"] = "Category successfully deleted";
            return RedirectToAction("Index");
        }
    }
}
