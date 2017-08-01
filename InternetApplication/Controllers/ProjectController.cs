using InternetApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetApplication.Controllers
{
    public class ProjectController : Controller
    {
        ProjectManagmentEntities6 db = new ProjectManagmentEntities6();
        // GET: Project
        public ActionResult Home()
        {
            
          
          
            return View(db.projects.Where(p=> p.assignedto==null).ToList());
        }


        // POST
        [HttpPost]
        public ActionResult Home(project p)
        {
               
                HttpCookie cookie = Request.Cookies["user_info"];
                p.createdby = Int16.Parse(cookie["id"]);
                User user = db.Users.Find(Int16.Parse(cookie["id"]));
                p.created_by_name = user.username;
                db.projects.Add(p);
                db.SaveChanges();
                ViewBag.username = user.username;
                if(user.user_type_id==1)
                {
                return RedirectToAction("Edit", "User");
                }
                else
                return RedirectToAction("Home");
           
        }

        // POST: Project/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Project/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Project/Delete/5
        public ActionResult Delete(int id)
        {

            return View();
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
            catch (NullReferenceException n)
            {
                return false;
            }


        }

        // POST: Project/Delete/5

        // GET
        public ActionResult Assign()
        {
            User user = GetLoggedIn();
            if (isLoggedIn() && user.user_type_id == 3)
            {

                ViewBag.username = user.username;
                return View((db.projects.Where(p => p.assignedto == null).ToList()));
            }
            else
                return RedirectToAction("Login","User");
        }

        [HttpPost]
        public ActionResult Assign(int id)
        {
            User user =GetLoggedIn();
            int Assing_id = user.id;
            project p = db.projects.Find(id);
            p.assignedto = Assing_id;
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Assign");
        }

        // GET: Project
        public ActionResult Assigned()
        {
            return View(db.projects.Where(p => p.assignedto != null).ToList());
        }


        // POST
        [HttpPost]
        public ActionResult Assigned(int p)
        {
            string result = Request.Form["action"];
            if (result == "reject")
            {
                project pro = db.projects.Find(p);
                pro.assignedto = null;
                db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return View();

        }

    }
}
