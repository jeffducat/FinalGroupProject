using FinalProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class ShopController : Controller
    {
        private franchiseDbEntities ORM = new franchiseDbEntities();

        public ActionResult Index(string message)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if(user.C_Hero_Villain_ == null)
            {
                return RedirectToAction("UserInfo", "Home");
            }

            ViewBag.Message = message;
            ViewBag.UserItems = Inventory();

            return View(ORM.Items.Where(i => i.Availability == ((bool)user.C_Hero_Villain_ ? "good" : "bad") || i.Availability == "both"));
        }
        
        public List<UserItem> Inventory()
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            return ORM.UserItems.Where(i => i.UserId == user.Id).ToList();
        }

        public ActionResult Sell(int item, int quantity)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            UserItem selling = ORM.UserItems.Find(item);
            if (quantity < 1 || quantity > selling.Quantity)
            {
                return RedirectToAction("Index", new { message = "Invalid quantity!" });
            }
            ORM.UserItems.Attach(selling);
            ORM.AspNetUsers.Attach(user);
            selling.Quantity -= quantity;
            user.Bitcoin += selling.Item.Cost * (decimal)0.4;
            if(selling.Quantity <= 0)
            {
                ORM.UserItems.Remove(selling);
            }
            ORM.SaveChanges();
            return RedirectToAction("Index", new { message = $"Item has been sold." });
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

        public ActionResult Purchase(int id, int quantity)
        {
            if(User == null)
            {
                return RedirectToAction("Login", "Account");
            }

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

            if(quantity < 1)
            {
                return RedirectToAction("Index", new { message = "Invalid quantity!" });
            }

            return RedirectToAction("Index", new { message = $"You don't have the funds for {item.ItemName}!" });
        }
    }
}