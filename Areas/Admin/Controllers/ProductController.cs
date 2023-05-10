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
        private readonly IWebHostEnvironment _webHostEnvironment;                       //to get root path of this application
        public ProductController(IUnitofWork unitWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitWork = unitWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitWork.ProductRepository.GetAll(properties: "Category").ToList();

            return View(products);
        }
        [HttpGet]
        public IActionResult Upsert(int ? id)   //create+edit
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
                Product = new Product(),                            //blank/null product object
                Categories = categoryList
            };

            if(id == 0 || id == null)
                return View(pdct);                                  //create

            else                                                    //update
            {
                pdct.Product = _unitWork.ProductRepository.Get(x => x.Id == id);
                return View(pdct);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductCategory obj, IFormFile? file)    //create+edit
        {
            try 
            {
                if (file != null)
                {
                    string wwwRoot = _webHostEnvironment.WebRootPath;               //get the root folder path

                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);     //filename

                    string filepath = Path.Combine(wwwRoot, @"images\products");        //filepath to store

                    if(obj.Product.ImageURL != null)                //already a file exists for this obj
                    {
                        //delete the file from file system
                        string oldPath = obj.Product.ImageURL;      //\images\products\6e024256-b787-4b72-9074-07ea7dd93150.jpg

                        string toDeleteFile = wwwRoot + oldPath;

                        if (System.IO.File.Exists(toDeleteFile))
                        {
                            System.IO.File.Delete(toDeleteFile);    //delete it
                        }
                    }

                    using (FileStream fs = new FileStream(Path.Combine(filepath, filename), FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }

                    obj.Product.ImageURL = @"\images\products\" + filename;
                }


                if (ModelState.IsValid)
                {
                    if(obj.Product.Id == 0 || obj.Product.Id  == null)
                        _unitWork.ProductRepository.Add(obj.Product);
                    else
                        _unitWork.ProductRepository.Update(obj.Product);
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
            catch(Exception e)
            {
                return View();
            }
            


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

        #region API CALL
        public IActionResult GetAll()
        {
            List<Product> products = _unitWork.ProductRepository.GetAll(properties: "Category").ToList();

            return Json(new { data = products});
        }
        #endregion
    }
}
