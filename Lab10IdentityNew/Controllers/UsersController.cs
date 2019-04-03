using ProiectIP2019.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ProiectIP2019.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: User
        public ActionResult Index()
        {

            var categories = db.Categories;
            ViewBag.Categories = categories;

            var users = db.Users;

            //var users = from user in db.Users
            //            orderby user.UserName
            //            select user;
            ViewBag.UsersList = users;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }
        public ActionResult Edit(string id)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            ViewBag.Email = user.Email;
            ViewBag.UserName = user.UserName;
            return View(user);
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            
        try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new
               RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(context));
                ViewBag.FalseEmail = 0;
                ViewBag.Phone = 0;

                string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(emailRegex);
                if (newData.Email != null && !re.IsMatch(newData.Email))
                {
                    ViewBag.Email = newData.Email;
                    newData.Email = null;
                    ViewBag.FalseEmail = 1;

                }

                string phoneRegex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                Regex re1 = new Regex(phoneRegex);
                if (newData.PhoneNumber != null && !re1.IsMatch(newData.PhoneNumber))
                {
                    
                    ViewBag.Phone = 1;

                }


                if (newData.Email != null && newData.UserName != null && ViewBag.Phone == 0)
                {
                    
                    if (TryUpdateModel(user))
                    {
                        user.UserName = newData.UserName;
                        user.Email = newData.Email;
                        user.PhoneNumber = newData.PhoneNumber;
                        var roles = from role in db.Roles select role;
                        foreach (var role in roles)
                        {
                            UserManager.RemoveFromRole(id, role.Name);
                        }
                        var selectedRole =
                        db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                        UserManager.AddToRole(id, selectedRole.Name);
                        db.SaveChanges();
                        TempData["message"] = "User has been modified!";
                        TempData["color"] = "alert-info";

                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }
                    var selectedRole =
                    db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);
                    newData.AllRoles = GetAllRoles();
                    ViewBag.Email = newData.Email;
                    ViewBag.UserName = newData.UserName;
                    
                    return View(newData);
                }

            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return View(user);
            }

        }


    }
}