using FinalProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        private franchiseDbEntities ORM = new franchiseDbEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return RedirectToAction("UserInfo");
        }

        public ActionResult UserInfo()
        {
            string userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            AspNetUser user = ORM.AspNetUsers.Find(userId);
            ORM.AspNetUsers.Attach(user);

            #region Drop-down for State names
            List<Crime> crimeList = ORM.Crimes.ToList();
            List<string> stateNames = new List<string>();


            foreach (Crime s in crimeList)
            {
                stateNames.Add(s.State);
            }

            ViewBag.StateNames = stateNames;


            if (user.StateId != null)
            {
                ViewBag.Img = "..\\Pictures\\StateImages\\" + user.StateId + ".jpg";
            }
            #endregion

            #region Drop-down for superpowers
            List<Ability> list = ORM.Abilities.ToList();
            List<string> powerNames = new List<string>();

            foreach (Ability power in list)
            {
                powerNames.Add(power.Ability1);
            }

            ViewBag.PowerNames = powerNames;
            #endregion

            #region Drop-down for personalities

            List<Mentor> mentors = ORM.Mentors.ToList();
            List<string> personalities = new List<string>();
            List<string> personalitiesDistinct = new List<string>();


            foreach (Mentor men in mentors)
            {
                personalities.Add(men.Personality);
                personalitiesDistinct = personalities.Distinct().ToList();
            }

            ViewBag.Personalities = personalitiesDistinct;

            #endregion

            #region Drop-down for mentors
            List<Mentor> badMentors = ORM.Mentors.Where(u => u.Hero_Villain == false).ToList();
            List<Mentor> goodMentors = ORM.Mentors.Where(u => u.Hero_Villain == true).ToList();

            if (user.C_Hero_Villain_ != null)
            {
                if ((bool)user.C_Hero_Villain_)
                {
                    ViewBag.Mentors = goodMentors;
                }
                else
                {
                    ViewBag.Mentors = badMentors;
                }
            }
            else
            {
                ViewBag.Error = "Please Choose an Alliance";
            }
            #endregion

            if (user.Mentor != null)
            {
                ViewBag.UserMentor = user.Mentor;
            }

            if(TempData["errorPersonality"] != null)
            {
                ViewBag.errorPersonality = TempData["errorPersonality"];
            }
            if (TempData["errorSuperPower"] != null)
            {
                ViewBag.errorSuperPower = TempData["errorSuperPower"];
            }
            if (TempData["errorAlliance"] != null)
            {
                ViewBag.errorAlliance = TempData["errorAlliance"];
            }
            if (TempData["errorMentor"] != null)
            {
                ViewBag.errorMentor = TempData["errorMentor"];
            }
            if (TempData["errorState"] != null)
            {
                ViewBag.errorState = TempData["errorState"];
            }

            return View(user);
        }

        public ActionResult Test()
        {
            return View();
        }

        #region Code-First for Crimes Table from API
        //////////////////////
        ///How we got states//
        //////////////////////
        //string[] states = { "AL","AK","AZ","AR","CA","CO","CT","DE","FL","GA","HI",
        //                    "ID","IL","IN","IA","KS","KY","LA","ME","MD","MA","MI",
        //                    "MN","MS","MO","MT","NE","NV","NH","NJ","NM","NY","NC",
        //                    "ND","OH","OK","OR","PA","RI","SC","SD","TN","TX","UT",
        //                    "VT","VA","WA","WV","WI","WY" };

        //for(int i = 0; i < states.Length; i++)
        //{
        //    Crime crime = new Crime(states[i]);
        //    ORM.Crimes.Add(crime);

        /* public Crime(string state)
         {
             JObject allData = JObject.Parse(GetStateData(state));
             JObject data = (JObject)(allData["results"].Where(s => (int)s["year"] == 2017).First());


             State = (string)(data)["state_abbr"];
             Population = (double?)(data)["population"] ?? 0;
             Violent_Crime = (double?)(data)["violent_crime"] ?? 0;
             Homicide = (double?)(data)["homicide"] ?? 0;

             Rape_Revised = (double?)(data)["rape_revised"] ?? 0;
             Robbery = (double?)(data)["robbery"] ?? 0;
             Aggravated_Assault = (double?)(data)["aggravated_assault"] ?? 0;
             Property_Crime = (double?)(data)["property_crime"] ?? 0;

             Burglary = (double?)(data)["burglary"] ?? 0;
             Larceny = (double?)(data)["larceny"] ?? 0;
             Motor_Vehicle_Theft = (double?)(data)["motor_vehicle_theft"] ?? 0;
             Arson = (double?)(data)["arson"] ?? 0;


         }
         public Crime()
         {

         }*/

        /*private static string apiKey = ConfigurationManager.AppSettings["apiKey"];

        public static string GetStateData(string state)
        {
            HttpWebRequest request = WebRequest.CreateHttp($"https://api.usa.gov/crime/fbi/sapi/api/estimates/states/{state}/2000/2017?api_key={apiKey}");

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader data = new StreamReader(response.GetResponseStream());
                return data.ReadToEnd();
            }
            return null;
        }*/
        #endregion


        public ActionResult SaveSuperName(string superName)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);
            user.SuperName = superName;
            ORM.SaveChanges();
            return RedirectToAction("UserInfo");
        }
        public ActionResult SavePersonality(string personality)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);
            List<Mentor> mentors = ORM.Mentors.ToList();
            List<string> personalities = new List<string>();
            List<string> personalitiesDistinct = new List<string>();

            foreach (Mentor men in mentors)
            {
                personalities.Add(men.Personality);
            }
            personalitiesDistinct = personalities.Distinct().ToList();

            if(personalitiesDistinct.Contains(personality))
            {
                user.Personality = personality;
                ORM.SaveChanges();
                return RedirectToAction("UserInfo");
            }

            TempData["errorPersonality"] = "Not a valid personality, Please choose again!";
            return RedirectToAction("UserInfo");
        }
        public ActionResult SaveSuperPower(string superPower)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);

            List<Ability> list = ORM.Abilities.ToList();
            List<string> powerNames = new List<string>();

            foreach (Ability power in list)
            {
                powerNames.Add(power.Ability1);
            }

            if (powerNames.Contains(superPower))
            {
                user.SuperPower = superPower;
                ORM.SaveChanges();
                return RedirectToAction("UserInfo");
            }

            TempData["errorSuperPower"] = "Not a valid superpower, Please choose again!";
            return RedirectToAction("UserInfo");
        }
        public ActionResult SaveAlliance(string HeroVillain)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);

            if(bool.Parse(HeroVillain) == true || bool.Parse(HeroVillain) == false)
            {
                user.C_Hero_Villain_ = bool.Parse(HeroVillain);
                ORM.SaveChanges();
                return RedirectToAction("UserInfo");
            }
            
            TempData["errorAlliance"] = "Not a valid alliance, Please choose again!";
            return RedirectToAction("UserInfo");
        }
        public ActionResult SaveMentor(string Id)
        {
            AspNetUser user = ORM.AspNetUsers.Find(User.Identity.GetUserId());
            ORM.AspNetUsers.Attach(user);
            List<Mentor> mentors = ORM.Mentors.ToList();
            List<string> mentorIds = new List<string>();

            foreach (Mentor mentor in mentors)
            {
                mentorIds.Add(mentor.Id.ToString());
            }

            if (mentorIds.Contains(Id))
            {
                Mentor current = ORM.Mentors.Find(Id);
                if((bool)user.C_Hero_Villain_ == true)
                {
                    if ((bool)current.Hero_Villain == false)
                    {
                        user.MentorId = null;
                    }
                    else
                    {
                        user.MentorId = int.Parse(Id);
                        ORM.SaveChanges();
                        return RedirectToAction("UserInfo");
                    }
                }
                else if((bool)user.C_Hero_Villain_ == false)
                {
                    if ((bool)current.Hero_Villain == true)
                    {
                        user.MentorId = null;
                    }
                    else
                    {
                        user.MentorId = int.Parse(Id);
                        ORM.SaveChanges();
                        return RedirectToAction("UserInfo");
                    }
                }
            }

            TempData["errorMentor"] = "Not a valid mentor, Please choose again!";
            return RedirectToAction("UserInfo");
        }
    }
}






