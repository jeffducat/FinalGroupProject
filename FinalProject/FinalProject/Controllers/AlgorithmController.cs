using FinalProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class AlgorithmController : Controller
    {
        private franchiseDbEntities ORM = new franchiseDbEntities();

        // GET: Algorithm
        public ActionResult Index(string message = null)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());

            if (user.C_Hero_Villain_ == true)
            {
                #region Generate a List of crime names that the chosen Ability by the User is 'Good' at or has a 'True' relationship with
                Ability ability = ORM.Abilities.Find(user.SuperPower);

                List<string> GoodAt = new List<string>();

                foreach (var crime in ability.GetType().GetProperties())
                {
                    if (crime.GetValue(ability) is bool)
                    {
                        bool b = (bool)crime.GetValue(ability);

                        if (b)
                        {
                            GoodAt.Add(crime.Name);
                        }
                    }
                }
                #endregion

                #region Using that List, find the max crime rate out of the entire Crimes table for only our crime list and get the State Information, img, and suggested items
                List<Item> SuggestedItems = new List<Item>();
                double maxValue = 0;
                foreach (Crime crime in ORM.Crimes)
                {
                    foreach (var property in crime.GetType().GetProperties())
                    {

                        if (GoodAt.Contains(property.Name))
                        {
                            double current = (Convert.ToDouble(property.GetValue(crime))/((double)crime.Population))*100000;
                            //int current = Convert.ToInt32(property.GetValue(crime));
                            if (current > maxValue)
                            {
                                ViewBag.Name = (string)property.Name;
                                ViewBag.Img = "..\\Pictures\\StateImages\\" + crime.State + ".jpg";
                                ViewBag.State = crime;
                                maxValue = current;
                                SuggestedItems.Add(ORM.Items.Where(i => i.Crime == (string)property.Name && (i.Availability == "good" || i.Availability == "both")).FirstOrDefault());
                            }
                        }
                    }
                }

                ViewBag.Max = maxValue.ToString("0.00");
                ViewBag.Ability = ability.Ability1;
                ViewBag.SuggestedItems = SuggestedItems.Distinct();
                #endregion

                #region User chooses personality, what mentors they are good with

                List<Mentor> mentors = ORM.Mentors.Where(m => (bool)m.Hero_Villain == true).ToList();
                List<string> GoodWith = new List<string>();

                foreach (Mentor m in mentors)
                {
                    if (user.Personality == m.Personality)
                    {
                        GoodWith.Add(m.Name);
                    }
                }
                ViewBag.Mentors = GoodWith;


                #endregion
            }
            else
            {
                #region Generate a List of crime names that the chosen Ability by the User is 'Bad' at or has a 'false' relationship with
                Ability ability = ORM.Abilities.Find(user.SuperPower);

                List<string> BadAt = new List<string>();

                foreach (var crime in ability.GetType().GetProperties())
                {
                    if (crime.GetValue(ability) is bool)
                    {
                        bool b = (bool)crime.GetValue(ability);

                        if (!b)
                        {
                            BadAt.Add(crime.Name);
                        }
                    }
                }
                #endregion

                #region Using that List, find the max crime rate out of the entire Crimes table for only our crime list and get the State Information, img, and suggested items
                List<Item> SuggestedItems = new List<Item>();
                double maxValue = 0;
                foreach (Crime crime in ORM.Crimes)
                {
                    foreach (var property in crime.GetType().GetProperties())
                    {

                        if (BadAt.Contains(property.Name))
                        {
                            double current = (Convert.ToDouble(property.GetValue(crime)) / ((double)crime.Population)) * 100000;
                            if (current > maxValue)
                            {
                                ViewBag.Name = (string)property.Name;
                                ViewBag.Img = "..\\Pictures\\StateImages\\" + crime.State + ".jpg";
                                ViewBag.State = crime;
                                maxValue = current;
                                SuggestedItems.Add(ORM.Items.Where(i => i.Crime == (string)property.Name && (i.Availability == "bad" || i.Availability == "both")).FirstOrDefault());
                            }
                        }
                    }
                }

                ViewBag.Max = maxValue.ToString("0.00");
                ViewBag.Ability = ability.Ability1;
                ViewBag.SuggestedItems = SuggestedItems.Distinct();
                #endregion

                #region User chooses personality, what mentors they are good with

                List<Mentor> mentors = ORM.Mentors.Where(m => (bool)m.Hero_Villain == false).ToList();
                List<string> GoodWith = new List<string>();

                foreach (Mentor m in mentors)
                {
                    if (user.Personality == m.Personality)
                    {
                        GoodWith.Add(m.Name);
                    }
                }

                ViewBag.Mentors = GoodWith;
                #endregion
            }

            ViewBag.Message = message;
            ViewBag.Inventory = Inventory();
            return View(user);
        }

        public ActionResult SaveFranchise(string location, string mentorName)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);
            Crime franchiseLoc = ORM.Crimes.Find(location);
            Mentor suggestedMentor = ORM.Mentors.Where(m => m.Name == mentorName).FirstOrDefault();

            user.StateId = franchiseLoc.State;
            user.Mentor = suggestedMentor;
            ORM.SaveChanges();
            return RedirectToAction("../Home/UserInfo");
        }
        public ActionResult ViewUserFranchise()
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ViewBag.Inventory = Inventory();
            ViewBag.Img = "..\\Pictures\\StateImages\\"+ user.StateId + ".jpg";
            return View(user);
        }

        #region purchase stuff
        public ActionResult Purchase(int id, int quantity)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());

            Item item = ORM.Items.Find(id);

            if (CanPurchase(item, quantity))
            {
                if (ORM.UserItems.Any(i => i.ItemId == item.Id && i.UserId == user.Id))
                {
                    UserItem existing = ORM.UserItems.Where(i => i.ItemId == item.Id && i.UserId == user.Id).FirstOrDefault();
                    ORM.UserItems.Attach(existing);
                    existing.Quantity += quantity;
                }
                else
                {
                    UserItem newItem = new UserItem
                    {
                        Item = item,
                        AspNetUser = user,
                        ItemId = item.Id,
                        UserId = user.Id,
                        Quantity = quantity
                    };
                    ORM.UserItems.Add(newItem);
                }
                user.Bitcoin -= item.Cost * quantity;
                ORM.SaveChanges();

                return RedirectToAction("Index", new { message = $"{item.ItemName} purchased successfully." });
            }

            if (quantity < 1)
            {
                return RedirectToAction("Index", new { message = "Invalid quantity!" });
            }

            return RedirectToAction("Index", new { message = $"You don't have the funds for {item.ItemName}!" });
        }

        public bool CanPurchase(Item item, int quantity)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            if (item.Cost * quantity > user.Bitcoin)
            {
                return false;
            }

            if (quantity < 1)
            {
                return false;
            }

            return true;
        }

        public List<UserItem> Inventory()
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            return ORM.UserItems.Where(i => i.UserId == user.Id).ToList();
        }
        #endregion
    }
}