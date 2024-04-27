using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCproj.Models;
using MVCproj.Services;
using System.Data;

namespace MVCproj.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment) {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var products = context.Prodcts.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]        
        public IActionResult Create(ProjectDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFilename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFilename += Path.GetExtension(productDto.ImageFile.FileName);

            string imageFullPath = environment.WebRootPath + "/Images/" + newFilename;
            using(var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Price = productDto.Price,
                Category = productDto.Category,
                Description = productDto.Description,
                ImageFileName = newFilename,
                CreatedAt = DateTime.Now,
            };
            context.Prodcts.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index","Products");
        }
        public IActionResult Edit(int id)
        {
            var product = context.Prodcts.Find(id);
            if(product==null){
                return RedirectToAction("Index", "Products");
            }
            var productDto = new ProjectDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Description = product.Description,
                Price = product.Price,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
            return View(productDto);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProjectDto productDto)
        {
            var product = context.Prodcts.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Products");

            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                return View(productDto);
            }

            string newFileName = product.ImageFileName;
            if(productDto.ImageFile!= null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);
                string imageFullPath = environment.WebRootPath + "/Images/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                string oldImageFullPath = environment.WebRootPath + "/Images/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Description = productDto.Description;
            product.Price= productDto.Price;
            product.Category = productDto.Category;
            product.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }
        public IActionResult Delete(int id)
        {
            var product = context.Prodcts.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");

            }
            string imageFullPath = environment.WebRootPath + "/Images/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);
            context.Prodcts.Remove(product);
            context.SaveChanges(true);
            return RedirectToAction("Index", "Products");
        }
    }
}
