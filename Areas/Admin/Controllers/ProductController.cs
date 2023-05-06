using Bulky.Models;
using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitofWork _unitWork;
        public ProductController(IUnitofWork unitWork)
        {
            _unitWork = unitWork;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitWork.ProductRepository.GetAll().ToList();

            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {

            IEnumerable<SelectListItem> categoryList = _unitWork.CategoryRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            //ViewBag.categoryList = categoryList;
            //encapsulate both into single view models
            ProductCategory pdct = new ProductCategory()
            {
                Product = new Product(),
                Categories = categoryList
            };
            return View(pdct);
        }
        [HttpPost]
        public IActionResult Create(ProductCategory obj)
        {
            
            if (ModelState.IsValid)
            {
                _unitWork.ProductRepository.Add(obj.Product);
                _unitWork.Save();
                TempData["success"] = "Product successfully added";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Product can not be created";
                obj.Categories = _unitWork.CategoryRepository.GetAll().Select(x => new SelectListItem
                { 
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                return View(obj);
            }

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? obj = _unitWork.ProductRepository.Get(x => x.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> categoryList = _unitWork.CategoryRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            ViewBag.categoryList = categoryList;
            //encapsulate both into single view models
            ProductCategory pdct = new ProductCategory()
            {
                Product = obj,
                Categories = categoryList
            };


            return View(pdct);
        }
        [HttpPost]
        public IActionResult Edit(ProductCategory obj)
        {

            if (ModelState.IsValid)
            {
                _unitWork.ProductRepository.Update(obj.Product);
                _unitWork.Save();
                TempData["success"] = "Product successfully edited";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Product can not be edited";
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? obj = _unitWork.ProductRepository.Get(x => x.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Product can not be deleted";
                return NotFound();
            }

            Product? obj = _unitWork.ProductRepository.Get(x => x.Id == id);

            if (obj == null)
            {
                TempData["error"] = "Product can not be deleted";
                return RedirectToAction("Index");
            }

            _unitWork.ProductRepository.Remove(obj);
            _unitWork.Save();
            TempData["success"] = "Product successfully deleted";
            return RedirectToAction("Index");
        }
    }
}
