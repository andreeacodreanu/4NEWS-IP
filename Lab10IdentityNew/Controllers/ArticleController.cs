using ProiectIP.Models;
using ProiectIP2019;
using ProiectIP2019.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectIP.Controllers
{
    //[Authorize]
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
 
        //[Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {
            var articles = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes").OrderByDescending(a => a.Date);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = db.Categories;
            ViewBag.Categories = categories;

            ViewBag.Articles = articles;

            //06.01.2019
            var articlesLatestNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.Date);
            ViewBag.LatestNews = articlesLatestNews;
            var articlesExternalNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link != null).OrderByDescending(a => a.Date);
            ViewBag.ExternalNews = articlesExternalNews;
            var articlesMostCommented = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a=>a.NrComm);
            ViewBag.MostCommented = articlesMostCommented;
            
            //

            var count = 1;
            var poza1 = 0;
            var poza2 = 0;
            var poza3 = 0;
            foreach (var article in articles)
            {
                if (article.Link == null)
                {
                    if (count == 1)
                    {
                        poza1 = article.NrComm;
                    }
                    else if (count == 2)
                    {
                        poza2 = article.NrComm;
                    }

                    else if (count == 3)
                    {
                        poza3 = article.NrComm;
                    }
                    else break;

                    count++;
                }
            }

            ViewBag.poza1 = poza1;
            ViewBag.poza2 = poza2;
            ViewBag.poza3 = poza3;

            


            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchString)
        {
            var articles = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes").OrderByDescending(a => a.Date);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = db.Categories;
            ViewBag.Categories = categories;

            ViewBag.Articles = articles;
            //07.01.2019
            var articlesLatestNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.Date);
            ViewBag.LatestNews = articlesLatestNews;
            var articlesExternalNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link != null).OrderByDescending(a => a.Date);
            ViewBag.ExternalNews = articlesExternalNews;
            var articlesMostCommented = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.NrComm);
            ViewBag.MostCommented = articlesMostCommented;

            //

            if (!string.IsNullOrEmpty(searchString))
            {
                ViewBag.butonCautare = true;
                var articleSearch = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && (a.Title.Contains(searchString) || a.Content.Contains(searchString) || a.User.UserName.Contains(searchString) ) ).OrderBy(a => a.Title);
                ViewBag.articoleCautate = articleSearch;
                //searchString = null;
            }

            return View();
        }


        public ActionResult Show(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            ViewBag.Category = article.Category;
            ViewBag.afisareButoane = false;

            ViewBag.ArticleId = article.Id;
            ViewBag.ArticleTitle = article.Title;
            ViewBag.ArticleContent = article.Content;
            ViewBag.ArticleDate = article.Date;
            ViewBag.ArticleUser = article.User.UserName;
            ViewBag.ArticleImage = article.Image;

            
            
            var articlesLatestNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.Date);
            ViewBag.LatestNews = articlesLatestNews;
            var articlesExternalNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link != null).OrderByDescending(a => a.Date);
            ViewBag.ExternalNews = articlesExternalNews;
            var articlesMostCommented = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null).OrderByDescending(a => a.NrComm);
            ViewBag.MostCommented = articlesMostCommented;
            var articlesRelatedNews = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "yes" && a.Link == null && a.Id != article.Id && a.CategoryId == article.CategoryId).OrderByDescending(a => a.Date);
            ViewBag.articlesRelatedNews = articlesRelatedNews;

            //

            if ( User.IsInRole("Administrator") || article.UserId == User.Identity.GetUserId())
            {
                
                ViewBag.afisareButoane = true;
            }
            if (User.IsInRole("Editor") || User.IsInRole("Administrator") || User.IsInRole("User"))
            {
                ViewBag.afisareButoaneComentariu = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            var categories = db.Categories;
            ViewBag.Categories = categories;

            var comments = from Comments in db.Comments
                           where Comments.ArticleId == article.Id
                           select Comments;
            ViewBag.Comments = comments;

            Comment comment = new Comment();
            comment.ArticleId = id;
            comment.UserId = User.Identity.GetUserId(); ;

            return View(comment);

        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Approve()
        {
            var articles = db.Articles.Include("Category").Include("User").Where(a => a.Approved == "false").OrderByDescending(a => a.Date);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = db.Categories;
            ViewBag.Categories = categories;
            
            ViewBag.Articles = articles;

            return View();

        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult ShowNotApproved(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            article.Categories = GetAllCategories();
            article.UserId = User.Identity.GetUserId();
            
            var categories = db.Categories;
            ViewBag.Categories = categories;

            return View(article);

        }

        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult ShowNotApproved(int id,Article article1)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(article1))
                    {
                        Article article = db.Articles.Find(id);
                        article.Id = article1.Id;
                        article.Date = article1.Date;
                        article.Content = article1.Content;
                        article.Category = article1.Category;
                        article.Image = article1.Image;
                        article.Title = article1.Title;
                        article.Approved = "yes";
                        
                        db.SaveChanges();

                        TempData["message"] = "Articolul a fost adaugat!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(article1);
                    }
                }
                else
                {
                    return View(article1);
                }
            }
            catch (Exception e)
            {
                return View(article1);

            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Show(Comment comment)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            try
            {
                if (ModelState.IsValid)
                {
                    
                    Article article = db.Articles.Find(comment.ArticleId);
                    article.NrComm = article.NrComm + 1;
                    //
                    db.Comments.Add(comment);

                    db.SaveChanges();

                    TempData["message"] = "A fost adaugat!";
                    return RedirectToAction("Show/" + comment.ArticleId);
                }
                else
                {
                    return View(comment);
                }
            }
            catch (Exception e)
            {
                //return View(comment);
                //return RedirectToAction("Index");
                Exception realerror = e;
                while (realerror.InnerException != null)
                    realerror = realerror.InnerException;

                Console.WriteLine(realerror.ToString());
                return RedirectToAction("Show");

            }

        }

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New()
        {
            Article article = new Article();
            // preluam lista de categorii din metoda GetAllCategories()
            article.Categories = GetAllCategories();
            // Preluam ID-ul utilizatorului curent
            article.UserId = User.Identity.GetUserId();
            

            if (!User.IsInRole("Administrator") && !User.IsInRole("Editor"))
            {
                article.Approved = "false";
            }
            else
                article.Approved = "yes";

            var categories = db.Categories;
            ViewBag.Categories = categories;

            return View(article);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            // returnam lista de categorii
            return selectList;
        }


        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New(Article article)
        {
            article.Categories = GetAllCategories();
            //            article.CommentId = 0;

            var categories = db.Categories;
            ViewBag.Categories = categories;

            try
            {
                
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/mytemplate/img/"), fileName);
                            file.SaveAs(path);
                            article.Image = fileName;
                        

                        }

                    }
                    if (!User.IsInRole("Administrator") && !User.IsInRole("Editor"))
                    {
                        article.Approved = "false";
                    }
                    else
                        article.Approved = "yes";

                    
                    article.NrComm = 0;
                    
                    //apel script python
                    string progToRun = "C:\\Users\\Andreea\\Desktop\\Anul3\\ProiectIP2019\\Lab10IdentityNew\\script3.py";
                    char[] splitter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = "C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\Python36_64\\python.exe";
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.UseShellExecute = false;

                    // call script3.py to concatenate passed parameters
                    proc.StartInfo.Arguments = string.Concat(progToRun, " ", article.Content);
                    proc.Start();

                    //StreamReader sReader = proc.StandardOutput;
                    //string[] output = sReader.ReadToEnd().Split(splitter);

                    StreamReader sReader = proc.StandardOutput;
                    String[] output = sReader.ReadToEnd().Split(splitter);

                    string text = System.IO.File.ReadAllText(@"C:\\Users\\Andreea\\Desktop\\Anul3\\ProiectIP2019\\Lab10IdentityNew\\file.txt");
                    article.Resume = text;

                    

                    db.Articles.Add(article);
                    
                    db.SaveChanges();
                  
                    TempData["message"] = "Articolul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.categExista = article.CategoryId;
                    return View(article);
                }
            }
            catch (Exception e)
            {
                return View(article);
                
            }
        }

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Edit(int id)
        {

            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            article.Categories = GetAllCategories();

            var categories = db.Categories;
            ViewBag.Categories = categories;

            

            if (!User.IsInRole("Administrator") && !User.IsInRole("Editor")) // daca incearca utilizatorul sa editeze propria stire va trebuie sa ie reaprobata
            {
                article.Approved = "false";
            }
            

            if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(article);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                return RedirectToAction("Index");
            }
        }


        [HttpPut]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Edit(int id, Article requestArticle)
        {

            var categories = db.Categories;
            ViewBag.Categories = categories;
            try
            {
                if (ModelState.IsValid)
                {
                    Article article = db.Articles.Find(id);

                    if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(article))
                        {
                            article.Title = requestArticle.Title;
                            article.Content = requestArticle.Content;
                            article.Date = requestArticle.Date;
                            article.Image = requestArticle.Image;
                           

                            article.CategoryId = requestArticle.CategoryId;
                            if (Request.Files.Count > 0)
                            {
                                var file = Request.Files[0];
                                if (file != null && file.ContentLength > 0)
                                {
                                    var fileName = Path.GetFileName(file.FileName);
                                    var path = Path.Combine(Server.MapPath("~/Content/mytemplate/img/"), fileName);
                                    file.SaveAs(path);
                                    article.Image = fileName;

                                }
                                /*else
                                {
                                    article.Image = requestArticle.Image;
                                }
*/

                            }
                            if (!User.IsInRole("Administrator") && !User.IsInRole("Editor")) // daca incearca utilizatorul sa editeze propria stire va trebuie sa ie reaprobata
                            {
                                article.Approved = "false";
                                db.SaveChanges();
                                TempData["message"] = "Articolul a fost modificat!";
                                return RedirectToAction("Index");
                            }
                            db.SaveChanges();
                            TempData["message"] = "Articolul a fost modificat!";
                        }
                        return RedirectToAction("Show/"+article.Id);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index");
                    }


                }
                else
                {
                    requestArticle.Categories = GetAllCategories();
                    ViewBag.categExista = requestArticle.CategoryId;
                    return View(requestArticle);
                }

            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {

            Article article = db.Articles.Find(id);

            if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || (article.Approved == "false" && User.IsInRole("Editor")))
            {
                db.Articles.Remove(article);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        

        [HttpDelete]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult DeleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            Article article = db.Articles.Find(comment.ArticleId);

            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || article.UserId == User.Identity.GetUserId())
            {
                
                //Article article = db.Articles.Find(comment.ArticleId);
                article.NrComm = article.NrComm - 1;
                
                //

                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters!";
                
                return RedirectToAction("Show/" + comment.ArticleId);
            }

            return RedirectToAction("Show/" + comment.ArticleId);
        }

       
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult EditComment(int id)
        {

            Comment comment = db.Comments.Find(id);
            ViewBag.Comment = comment;
            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(comment);
                
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult EditComment(int id, Comment requestComment)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comment = db.Comments.Find(id);

                    if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(comment))
                        {
                            
                            comment.CommentContent = requestComment.CommentContent;
                            comment.CommentDate = requestComment.CommentDate;
                            comment.ArticleId = requestComment.ArticleId;

                            


                            db.SaveChanges();
                            TempData["message"] = "Comentariul a fost modificat!";
                        }
                        return RedirectToAction("Show/" + comment.ArticleId);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index");
                    }


                }
                else
                {
                    return View();
                }

            }
            catch (Exception e)
            {
                return View();
            }

        }


    }
}
