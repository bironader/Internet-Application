using InternetApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetApplication.Controllers
{
    public class UserController : Controller
    {
        ProjectManagmentEntities6 db = new ProjectManagmentEntities6();
        public static bool flag = false;


       
        public bool userExist(string username)
        {
            
           User user = db.Users.FirstOrDefault(u => u.username.Equals(username) );
            if (user == null)
            {
                return false;
            }
            else
                return true;
        }
        public User getUser(string username)
        {
            User user = db.Users.FirstOrDefault(u => u.username.Equals(username));
            if (user != null)
                return user;
            else
                return new User();


        }

      public User GetLoggedIn()
        {
            HttpCookie cookie = Request.Cookies["user_info"];
            
            User user = db.Users.Find(Int16.Parse(cookie["id"]));
            if (user != null)
                return user;
            else
                return new User();
        }


        public bool isLoggedIn()
        {
            HttpCookie cookie = Request.Cookies["user_info"];
            try
            {
                User user = db.Users.Find(Int16.Parse(cookie["id"]));
                return true;
            }
            catch(NullReferenceException n)
            {
                return false;
            }
           
         
        }

        // GET: Default
        public ActionResult Login()
        {
            ViewBag.error = false;
            return View();
        }
        // POST
        [HttpPost]
        public ActionResult Login(User user)
        {
           
            string remember = Request.Form["remember"];
            
            User newuser = getUser(user.username);
            if (user.password!=null &&newuser != null && newuser.password.Trim().Equals(user.password))
            {
                ViewBag.error = false;
                HttpCookie cookie = new HttpCookie("user_info");
                cookie["id"] = newuser.id.ToString();
                cookie["type"] = newuser.user_type_id.ToString();
               
                if (remember != null)
                {
                    cookie.Expires = DateTime.Now.AddYears(20);
                }
                Response.SetCookie(cookie);
                if (newuser.user_type_id == 2)
                    return RedirectToAction("Home", "Project");
                else if (newuser.user_type_id == 3)
                    return RedirectToAction("Assign", "Project");
                else if (newuser.user_type_id == 1)
                    return RedirectToAction("Edit");
               else
                return RedirectToAction("Register");

            }
            else {
                ViewBag.error = true;
                return View();
            }
           
        }

        public ActionResult Logout()
        {
            if(isLoggedIn())
            {
                if(Request.Cookies["user_info"]!=null)
                {
                    var c = new HttpCookie("MyCookie");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }
            }
            return RedirectToAction("Login");
        }


        // GET: Default/Create
        public ActionResult Register()
        {
            if (isLoggedIn())
            {
                ViewBag.error = false;
                return View();
            }
            else {
                if (GetLoggedIn().user_type_id == 2)
                {
                    return RedirectToAction("Home", "Project");
                }
                else if (GetLoggedIn().user_type_id == 3)
                    return RedirectToAction("Assign", "Project");
                else if (GetLoggedIn().user_type_id == 1)
                    return RedirectToAction("Edit");
                else
                    return RedirectToAction("Login");
            }
        }

        // POST: Default/Create
        [HttpPost]
        public ActionResult Register(User user, HttpPostedFileBase fileUploader)
        {
            if (isLoggedIn())
            {
                string repassword = Request.Form["repassword"].Trim();


                if (user.username != null && !userExist(user.username) && repassword != null && user.password.Equals(repassword) && fileUploader != null
                    && user.fn != null && user.ln != null && user.password != null && user.jobdesc != null && user.mobile != null)
                {
                    user.photo = new byte[fileUploader.ContentLength];
                    fileUploader.InputStream.Read(user.photo, 0, fileUploader.ContentLength);
                    db.Users.Add(user);
                    ViewBag.error = false;
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = true;
                    return View();
                }


            }
            else {
                if (GetLoggedIn().user_type_id == 2)
                {
                    return RedirectToAction("Home", "Project");
                }
                else if (GetLoggedIn().user_type_id == 3)
                    return RedirectToAction("Assign", "Project");
                else if (GetLoggedIn().user_type_id == 1)
                    return RedirectToAction("Edit");
                else
                    return RedirectToAction("Login");
            }
                
        }
       
        

        // GET: Default/Edit/5
        public ActionResult Edit()
        {
            if ( GetLoggedIn().user_type_id==1)
            {
                ViewBag.userslist = db.Users.ToList();

                return View(db.projects.ToList());
            }
            else
                return RedirectToAction("Login");
        }

        // POST: Default/Edit/5
        [HttpPost]
        public ActionResult Edit(int id)
        {
            try
            {
                if (GetLoggedIn().user_type_id == 1)
                {
                    int projectid = Int16.Parse(Request.Form["id"]);
                    string subject = Request.Form["subject"];
                    string description = Request.Form["description"];
                    project p = db.projects.Find(projectid);
                    p.subj = subject;
                    p.descr = description;

                    db.Entry(p).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    return RedirectToAction("Edit");
                }
                else
                    return RedirectToAction("Login");
            }
            catch
            {
                return View();
            }
        }


       

        // POST: Default/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                if (isLoggedIn() && GetLoggedIn().user_type_id == 1)
                {
                    project p = db.projects.Find(id);
                    db.projects.Remove(p);
                    db.SaveChanges();
                    return RedirectToAction("Edit");
                }
                else
                    return RedirectToAction("Login");
            }
            catch
            {
                return View();
            }
        }
        

        
        public ActionResult CustomerProfile(string username)
        {
            if (isLoggedIn() && GetLoggedIn().user_type_id == 2)
            {
                HttpCookie cookie = Request.Cookies["user_info"];
                int id = Int16.Parse(cookie["id"]);
                User user = GetLoggedIn();
                var base64 = Convert.ToBase64String(user.photo);
                ViewBag.img = string.Format("data:image/gif;base64,{0}", base64);
                ViewBag.fullname = user.fn + " " + user.ln;
                ViewBag.username = "@" + user.username;
                ViewBag.phone = user.mobile;
                ViewBag.desc = user.jobdesc;
                ViewBag.email = user.email;
                return View(db.projects.Where(p => p.createdby == id).ToList());
            }
            else
                return RedirectToAction("Login");
        }
        public ActionResult EditUser()
        {
            if (isLoggedIn() && GetLoggedIn().user_type_id == 1)
            {
                string fn = Request.Form["fname"];
                string ln = Request.Form["lname"];
                string email = Request.Form["email"];
                string username = Request.Form["username"];
                string jobdesc = Request.Form["job"];
                int usertype = Int16.Parse(Request.Form["type"]);
                string mobile = Request.Form["mobile"];
                User user = db.Users.FirstOrDefault(u => u.username.Equals(username));
                user.fn = fn;
                user.ln = ln;
                user.email = email;
                user.jobdesc = jobdesc;
                user.user_type_id = usertype;
                user.mobile = mobile;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit");
            }
            else
                return RedirectToAction("Login");
        }
        public ActionResult DeleteUser(string username)
        {
            if (isLoggedIn() && GetLoggedIn().user_type_id == 1)
            {
                User user = db.Users.FirstOrDefault(u => u.username.Equals(username));
                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Edit");
            }
            else
                return RedirectToAction("Login");
        }
    }
}
