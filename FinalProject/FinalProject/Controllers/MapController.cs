using FinalProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class MapController : Controller
    {
        private franchiseDbEntities ORM = new franchiseDbEntities();

        public ActionResult Index()
        {
            List<Crime> list = ORM.Crimes.ToList();
            List<string> stateNames = new List<string>();

            foreach(Crime state in list)
            {
                stateNames.Add(state.State);
            }

            ViewBag.StateNames = stateNames;
            return View(list);
        }

        public ActionResult GetMapInfo(string state)
        {
            List<Crime> list = ORM.Crimes.ToList();
            List<string> stateNames = new List<string>();

            foreach (Crime s in list)
            {
                stateNames.Add(s.State);
            }

            ViewBag.StateNames = stateNames;

            ViewBag.State = ORM.Crimes.Find(state);

            return View("../Map/Index"); 
        }

        public ActionResult SaveState(string state)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);
            List<Crime> list = ORM.Crimes.ToList();
            List<string> stateNames = new List<string>();

            foreach (Crime s in list)
            {
                stateNames.Add(s.State);
            }

            if(stateNames.Contains(state))
            {
                user.StateId = state;
                ORM.SaveChanges();
                return RedirectToAction("../Home/UserInfo");
            }

            TempData["errorState"] = "Not a valid location, Please choose again!";
            return RedirectToAction("../Home/UserInfo");
        }
    }
}