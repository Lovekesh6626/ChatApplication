using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApplication.Controllers
{
    public class HomeController : Controller
    {
        ChatprojectdbEntities db = new ChatprojectdbEntities();
        public ActionResult SignUp()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Include = "id,FullName,Email,Password")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Account.Add(account);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(account);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(ChatApplication.Models.Account pop)
        {
            bool isvalid = db.Account.Any(x => x.Email == pop.Email && x.Password == pop.Password);
            if(isvalid)
            {
                Session["email"] = pop.Email.ToString();
                var obj = db.Account.Where(a => a.Email.Equals(pop.Email) && a.Password.Equals(pop.Password)).FirstOrDefault();
                if(obj!= null)
                {
                    Session["name"] = obj.FullName.ToString();
                    Session["id"] = obj.id.ToString();
                    return RedirectToAction("List", "Home");

                }
            }
            return View();
        }

        public ActionResult List()
        {
            var sessionid= Session["id"];

            var userlist = db.Account.Where(x=>x.id.ToString()!= sessionid.ToString()).ToList();
            return View(userlist);
        }


        [HttpPost]
        public ActionResult Partial(int ?fid)
        {
            var sessionid = Session["id"];
            //var sessionid1 = Session["id"];
            var list = db.chat.Where(x => x.senderid.ToString() == sessionid.ToString() && x.receiverid == fid || x.receiverid.ToString() == sessionid.ToString() && x.senderid == fid).ToList();
            return View(list);
        }

       
        public ActionResult LogOut()
        {
            Session.Abandon();

            return Redirect("Login");
        }



        public ActionResult Message(int ?id)
        {
            ViewBag.receiverid = id;
            ViewBag.name = db.Account.Where(x => x.id == id).Select(x => x.FullName).SingleOrDefault();

            return View();
        }

        [HttpPost]

        public JsonResult Message([Bind(Include = "sno,senderid,receiverid,message,chattime")] chat chat)
        {
            if (ModelState.IsValid)
            {
                chat.chattime = DateTime.Now;
                db.chat.Add(chat);
                db.SaveChanges();
                return Json(chat);
            }

          

            return Json("true", JsonRequestBehavior.DenyGet);
        }
    }
}