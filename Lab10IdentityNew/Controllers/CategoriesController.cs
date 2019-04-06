using ProiectIP.Models;
using ProiectIP2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectIP.Controllers
{

    public class CategoriesController : Controller
    {

        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index()
        {

            var categories = db.Categories;
            ViewBag.Categories = categories;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        public ActionResult Show(int id)
        {
            Category Category = db.Categories.Find(id);
            ViewBag.Category = Category;
            var articles = db.Articles.Where(a => a.CategoryId == id && a.Approved == "yes").OrderByDescending(a => a.Date);

            ViewBag.articles = articles;
            var categories = db.Categories;
            ViewBag.Categories = categories;

            //07.01.2019
            var articlesLatestNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.Date);
            ViewBag.LatestNews = articlesLatestNews;
            var articlesExternalNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link != null).OrderByDescending(a => a.Date);
            ViewBag.ExternalNews = articlesExternalNews;
            var articlesMostCommented = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.NrComm);
            ViewBag.MostCommented = articlesMostCommented;

            //

            return View(Category);
        }

        [HttpPost]
        public ActionResult Show(int id, string SortType, string OrderBy)
        {
            Category Category = db.Categories.Find(id);
            ViewBag.Category = Category;

            var categories = db.Categories;
            ViewBag.Categories = categories;

            //07.01.2019
            var articlesLatestNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.Date);
            ViewBag.LatestNews = articlesLatestNews;
            var articlesExternalNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link != null).OrderByDescending(a => a.Date);
            ViewBag.ExternalNews = articlesExternalNews;
            var articlesMostCommented = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.NrComm);
            ViewBag.MostCommented = articlesMostCommented;

            //

            var articles = db.Articles.Where(a => a.CategoryId == id);
            if (SortType == "Name" && OrderBy == "Ascending")
               articles  = articles.OrderBy(a => a.Title);
            else
                if (SortType == "Name" && OrderBy == "Descending")
                    articles = articles.OrderByDescending(a => a.Title);
                else
                    if (SortType == "Date" && OrderBy == "Ascending")
                        articles = articles.OrderBy(a => a.Date);
                    else
                        if (SortType == "Date" && OrderBy == "Descending")
                            articles = articles.OrderByDescending(a => a.Date);

            ViewBag.Articles = articles;

            return View(Category);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult New()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult New(Category category)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            try
            {
                var ok = 1;
                foreach (var element in categories)
                {
                    if (category.CategoryName.ToLower().Equals(element.CategoryName.ToLower()))
                        ok = 0;
                }

                if (ok == 1)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["message"] = "Category has been added!";
                    TempData["color"] = "alert-info";
//                    ViewBag.color = "alert-info";
                }
                else
                {
                    TempData["message"] = "Category already exists!";
                    TempData["color"] = "alert-danger";
//                    ViewBag.color = "alert-danger";
                }
            
//                db.Categories.Add(category);
//                db.SaveChanges();
//                TempData["message"] = "Category has been added!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View(category);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);

                    var ok = 1;
                    foreach (var element in categories)
                    {
                        if (requestCategory.CategoryName.ToLower().Equals(element.CategoryName.ToLower()))
                            ok = 0;
                    }

                    

                    if (TryUpdateModel(category) && ok == 1)
                    {
                        category.CategoryName = requestCategory.CategoryName;
                        db.SaveChanges();
                        TempData["message"] = "Category has been modified!";
                        TempData["color"] = "alert-info";
                    }
                    else
                    {
                        TempData["message"] = "Category already exists!";
                        TempData["color"] = "alert-danger";
                        //                    ViewBag.color = "alert-danger";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }

            }
            catch (Exception e)
            {
                return View(requestCategory);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["message"] = "Category has been deleted!";
            TempData["color"] = "alert-danger";
            return RedirectToAction("Index");
        }
    }
}