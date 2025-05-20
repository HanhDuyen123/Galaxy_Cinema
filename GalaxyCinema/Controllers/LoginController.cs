using GalaxyCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GalaxyCinema.Controllers
{
    public class LoginController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return View();
            }

            var hashedPassword = GetMD5(password);


            var emp = db.EMPLOYEEs
                .FirstOrDefault(e => e.EEMAIL == email && e.EPASSWORD == hashedPassword);

            if (emp != null)
            {

                Session["userIDs"] = emp.EMPLOYEEID;
                Session["fullname"] = emp.ELASTNAME + " " + emp.EFIRSTNAME;
                Session["roleName"] = emp.ROLENAME;

                FormsAuthentication.SetAuthCookie(emp.EEMAIL, false);


                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    emp.EEMAIL,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    emp.ROLENAME
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);

                if (emp.ROLENAME == "Admin")
                    return RedirectToAction("Index", "Revenue");
                else
                    return RedirectToAction("Index", "Employee");
            }


            var cus = db.CUSTOMERs
                .FirstOrDefault(c => c.CEMAIL == email && c.CPASSWORD == hashedPassword);
            if (cus != null)
            {
                Session["userIDs"] = cus.CUSTOMERID;
                Session["fullname"] = cus.LASTNAME + " " + cus.FIRSTNAME;


                FormsAuthentication.SetAuthCookie(cus.CEMAIL, false);

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    cus.CEMAIL,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    "Customer"
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                return RedirectToAction("Index", "Customer");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }
        private string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}