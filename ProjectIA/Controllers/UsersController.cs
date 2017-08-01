using ProjectIA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectIA.Controllers
{
    public class UsersController : Controller
    {
        ProjectManagmentEntities2 db = new ProjectManagmentEntities2();

        // GET: Users
        public ActionResult Register()
        {
            
            return View();
        }

        // POST: Users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Register(User user)
        {
            if (ModelState.IsValid)
            {
                String fname = Request.Form["fname"];
                String lname = Request.Form["lname"];
                String password = Request.Form["password"];
                String repassword = Request.Form["repassword"];
                String email = Request.Form["email"]; 
                int type = Int32.Parse(Request.Form["type"]);
                String username = Request.Form["username"];
                if(password.Equals(repassword))
                {
                    user.fn = fname;
                    user.ln = lname;
                    user.email = email;
                    user.password = password;
                    user.username = username;
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }


        }



        public ActionResult Login()
        {
            return View();
        }

    }
}