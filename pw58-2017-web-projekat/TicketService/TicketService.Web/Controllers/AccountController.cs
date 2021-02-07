using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketService.Data.Models;
using TicketService.Data.Services;
using TicketService.Data.ViewModels;
using TicketService.Web.Models;

namespace TicketService.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;

        public AccountController()
        {
            HomeController homeController = new HomeController();
            _context = new DataContext();
        }
    
        [HttpGet]
        public ActionResult Register()
        {
            Customer customer = (Customer)Session["customer"];
            if (customer != null && !customer.UserName.Equals(string.Empty))
                return Content("<h1>Error<h1>");
    
            return View();
        }
    
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            Customer customer = new Customer()
            {
                ID = ++HomeController.customerID,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Password = model.Password,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                Role = EUserRole.Customer,
                IsDeleted = false
            };
    
            if (
                customer.FirstName == string.Empty ||
                customer.LastName == string.Empty ||
                customer.UserName == string.Empty ||
                customer.Password == string.Empty ||
                customer.BirthDate == null
                )
            {
                ViewBag.Message = $"All fields must be filled!";
                return View();
            }
    
    
    
            List<Customer> customers = (List<Customer>)HttpContext.Application["customers"];
    
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].UserName == customer.UserName)
                {
                    ViewBag.Message = $"Username is already taken!";
                    return View();
                }
            }
    
            customers.Add(customer);
            CustomerData.SaveCustomer(customer);
    
            return RedirectToAction("Login", "Account");
        }
    
        [HttpGet]
        public ActionResult Login()
        {
            Customer customer = (Customer)Session["customer"];
            if (customer != null && !customer.UserName.Equals(string.Empty))
            {
                Salesman salesman = (Salesman)Session["salesman"];
                if (salesman != null && !salesman.UserName.Equals(string.Empty))
                {
                    Administrator administrator = (Administrator)Session["administrator"];
                    if (administrator != null && !administrator.UserName.Equals(string.Empty))
                        return Content("<h1>Error<h1>");
                }
            }
    
    
            return View();
        }
    
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            EUserRole role = CheckUserRole(model.UserName, model.Password);

            if (role == EUserRole.Customer)
            {
                List<Customer> customers = (List<Customer>)HttpContext.Application["customers"];
                Customer customer = customers.Find(u => u.UserName.Equals(model.UserName) && u.Password.Equals(model.Password));

                if (customer.IsDeleted)
                {
                    ViewBag.Message = $"{customer.UserName} has been blocked!";
                    return View();
                }
                Session["customer"] = customer;
            }
            else if (role == EUserRole.Salesman)
            {
                List<Salesman> salesmen = (List<Salesman>)HttpContext.Application["salesmen"];
                Salesman salesman = salesmen.Find(u => u.UserName.Equals(model.UserName) && u.Password.Equals(model.Password));

                if (salesman.IsDeleted)
                {
                    ViewBag.Message = $"{salesman.UserName} has been blocked!";
                    return View();
                }

                Session["salesman"] = salesman;
            }
            else if (role == EUserRole.Administrator)
            {
                List<Administrator> administrators = (List<Administrator>)HttpContext.Application["administrators"];
                Administrator administrator = administrators.Find(u => u.UserName.Equals(model.UserName) && u.Password.Equals(model.Password));
                Session["administrator"] = administrator;
            }
            else // if (role == EUserRole.Guest)
            {
                ViewBag.Message = $"User with given credentials doesn't exists!";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Customer customer = (Customer)Session["customer"];
            if (customer == null || customer.UserName.Equals(string.Empty))
            {
                Salesman salesman = (Salesman)Session["salesman"];
                if (salesman == null || salesman.UserName.Equals(string.Empty))
                {
                    Administrator administrator = (Administrator)Session["administrator"];
                    if (administrator == null || administrator.UserName.Equals(string.Empty))
                        return Content("<h1>Error<h1>");
                    else
                        Session["administrator"] = null;
                }
                else
                    Session["salesman"] = null;
            }
            else
                Session["customer"] = null;

            CustomerController._myCart.Tickets.Clear();
    
            return RedirectToAction("Login", "Account");
        }
        
        public ActionResult ProfileSettings()
        {
            ProfileViewModel model = new ProfileViewModel();

            if (IsCustomer())
            {
                Customer customer = (Customer)Session["customer"];
                model = new ProfileViewModel()
                {
                    ID = customer.ID,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Gender = customer.Gender,
                    BirthDate = customer.BirthDate,
                    UserName = customer.UserName,
                    Password = customer.Password,
                    OldPassword = string.Empty,
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty,
                    Role = EUserRole.Customer,
                    NumberOfCollectedPoints = customer.NumberOfCollectedPoints,
                    CustomerType = customer.CustomerType
                };
                ViewBag.Layout = "~/Views/Shared/_Layout_Customer.cshtml";
            }
            else if (IsSalesman())
            {
                Salesman salesman = (Salesman)Session["salesman"];
                model = new ProfileViewModel()
                {
                    ID = salesman.ID,
                    FirstName = salesman.FirstName,
                    LastName = salesman.LastName,
                    Gender = salesman.Gender,
                    BirthDate = salesman.BirthDate,
                    UserName = salesman.UserName,
                    Password = salesman.Password,
                    OldPassword = string.Empty,
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty,
                    Role = EUserRole.Salesman
                };
                ViewBag.Layout = "~/Views/Shared/_Layout_Salesman.cshtml";
            }
            else if (IsAdministrator())
            {
                Administrator administrator = (Administrator)Session["administrator"];
                model = new ProfileViewModel()
                {
                    ID = administrator.ID,
                    FirstName = administrator.FirstName,
                    LastName = administrator.LastName,
                    Gender = administrator.Gender,
                    BirthDate = administrator.BirthDate,
                    UserName = administrator.UserName,
                    Password = administrator.Password,
                    OldPassword = string.Empty,
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty,
                    Role = EUserRole.Administrator
                };
                ViewBag.Layout = "~/Views/Shared/_Layout_Administrator.cshtml";
            }
            else
                return Content("<h1>You are not signed in.<h1>");

            return View(model);
        }
        
        [HttpGet]
        public ActionResult ChangeProfileSettings(string role, int id)
        {
            ProfileViewModel model = new ProfileViewModel();

            // optimizovati

            if (role == "Administrator")
            {
                try
                {
                    var administrator = _context.Administrator.GetAll().SingleOrDefault(x => x.ID == id);
                    model = new ProfileViewModel()
                    {
                        ID = administrator.ID,
                        FirstName = administrator.FirstName,
                        LastName = administrator.LastName,
                        Gender = administrator.Gender,
                        BirthDate = administrator.BirthDate,
                        UserName = administrator.UserName,
                        Password = administrator.Password,
                        OldPassword = string.Empty,
                        NewPassword = string.Empty,
                        ConfirmPassword = string.Empty,
                        Role = EUserRole.Administrator
                    };
                    ViewBag.Layout = "~/Views/Shared/_Layout_Administrator.cshtml";
                }
                catch (Exception)
                {
                    throw new KeyNotFoundException();
                }
            }
            else if (role == "Salesman")
            {
                try
                {
                    var salesman = _context.Salesman.GetAll().SingleOrDefault(x => x.ID == id);
                    model = new ProfileViewModel()
                    {
                        ID = salesman.ID,
                        FirstName = salesman.FirstName,
                        LastName = salesman.LastName,
                        Gender = salesman.Gender,
                        BirthDate = salesman.BirthDate,
                        UserName = salesman.UserName,
                        Password = salesman.Password,
                        OldPassword = string.Empty,
                        NewPassword = string.Empty,
                        ConfirmPassword = string.Empty,
                        Role = EUserRole.Administrator
                    };
                    ViewBag.Layout = "~/Views/Shared/_Layout_Salesman.cshtml";
                }
                catch (Exception)
                {
                    throw new KeyNotFoundException();
                }
            }
            else if (role == "Customer")
            {
                try
                {
                    var customer = _context.Customer.GetAll().SingleOrDefault(x => x.ID == id);
                    model = new ProfileViewModel()
                    {
                        ID = customer.ID,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Gender = customer.Gender,
                        BirthDate = customer.BirthDate,
                        UserName = customer.UserName,
                        Password = customer.Password,
                        OldPassword = string.Empty,
                        NewPassword = string.Empty,
                        ConfirmPassword = string.Empty,
                        Role = EUserRole.Administrator
                    };
                    ViewBag.Layout = "~/Views/Shared/_Layout_Customer.cshtml";
                }
                catch (Exception)
                {
                    throw new KeyNotFoundException();
                }
            }
            else
                return Content("<h1>You are not signed in.<h1>");


            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeProfileSettings(ProfileViewModel model)
        {
            if (model.Role == EUserRole.Administrator)
            {
                Administrator administrator = (Administrator)Session["administrator"];

                administrator.ID = model.ID;
                administrator.FirstName = model.FirstName;
                administrator.LastName = model.LastName;
                administrator.Gender = model.Gender;
                administrator.BirthDate = model.BirthDate;
                administrator.UserName = model.UserName;
                administrator.Password = model.Password;


                List<Administrator> administrators = (List<Administrator>)_context.Administrator.GetAll();

                int editID = model.ID - 1;

                administrators[editID].ID = model.ID;
                administrators[editID].FirstName = model.FirstName;
                administrators[editID].LastName = model.LastName;
                administrators[editID].Gender = model.Gender;
                administrators[editID].BirthDate = model.BirthDate;
                administrators[editID].UserName = model.UserName;
                administrators[editID].Password = model.Password;

                

                AdministratorData.SaveAdministrators(administrators);
                // save administrators

                return RedirectToAction("ProfileSettings", "Account");
            }
            else if (model.Role == EUserRole.Salesman)
            {
                Salesman salesman = (Salesman)Session["salesman"];

                salesman.ID = model.ID;
                salesman.FirstName = model.FirstName;
                salesman.LastName = model.LastName;
                salesman.Gender = model.Gender;
                salesman.BirthDate = model.BirthDate;
                salesman.UserName = model.UserName;
                salesman.Password = model.Password;

                List<Salesman> salesmen = (List<Salesman>)_context.Salesman.GetAll();

                int editID = model.ID - 1;

                salesmen[editID].ID = model.ID;
                salesmen[editID].FirstName = model.FirstName;
                salesmen[editID].LastName = model.LastName;
                salesmen[editID].Gender = model.Gender;
                salesmen[editID].BirthDate = model.BirthDate;
                salesmen[editID].UserName = model.UserName;
                salesmen[editID].Password = model.Password;

                SalesmanData.SaveSalesmen(salesmen);
                // save administrators

                return RedirectToAction("ProfileSettings", "Account");
            }
            else if (model.Role == EUserRole.Customer)
            {
                Customer customer = (Customer)Session["customer"];

                customer.ID = model.ID;
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.Gender = model.Gender;
                customer.BirthDate = model.BirthDate;
                customer.UserName = model.UserName;
                customer.Password = model.Password;

                List<Customer> customers = (List<Customer>)_context.Customer.GetAll();

                int editID = model.ID - 1;

                customers[editID].ID = model.ID;
                customers[editID].FirstName = model.FirstName;
                customers[editID].LastName = model.LastName;
                customers[editID].Gender = model.Gender;
                customers[editID].BirthDate = model.BirthDate;
                customers[editID].UserName = model.UserName;
                customers[editID].Password = model.Password;

                CustomerData.SaveCustomers(customers);

                return RedirectToAction("ProfileSettings", "Account");
            }
            else
                return Content("<h1>You are not signed in.<h1>");
        }

        #region Methods
        
        private EUserRole CheckUserRole(string username, string password)
        {
            if (IsCustomer(username, password))
                return EUserRole.Customer;
            else if (IsSalesman(username, password))
                return EUserRole.Salesman;
            else if (IsAdministrator(username, password))
                return EUserRole.Administrator;
            else
                return EUserRole.Guest;
        }

        private bool IsAdministrator()
        {
            Administrator administrator = (Administrator)Session["administrator"];
            if (administrator == null || administrator.Role != EUserRole.Administrator || administrator.UserName == string.Empty)
                return false;

            return true;
        }

        private bool IsSalesman()
        {
            Salesman salesman = (Salesman)Session["salesman"];
            if (salesman == null || salesman.Role != EUserRole.Salesman || salesman.UserName == string.Empty)
                return false;

            return true;
        }

        private bool IsCustomer()
        {
            Customer customer = (Customer)Session["customer"];
            if (customer == null || customer.Role != EUserRole.Customer || customer.UserName == string.Empty)
                return false;

            return true;
        }

        private bool IsAdministrator(string username, string password)
        {
            List<Administrator> administrators = (List<Administrator>)HttpContext.Application["administrators"];
            Administrator administrator = administrators.Find(u => u.UserName.Equals(username) && u.Password.Equals(password));
            if (administrator == null)
                return false;

            return true;
        }

        private bool IsSalesman(string username, string password)
        {
            List<Salesman> salesmen = (List<Salesman>)HttpContext.Application["salesmen"];
            Salesman salesman = salesmen.Find(u => u.UserName.Equals(username) && u.Password.Equals(password));
            if (salesman == null)
                return false;

            return true;
        }

        private bool IsCustomer(string username, string password)
        {
            List<Customer> customers = (List<Customer>)HttpContext.Application["customers"];
            Customer Customer = customers.Find(u => u.UserName.Equals(username) && u.Password.Equals(password));
            if (Customer == null)
                return false;

            return true;
        }
        
        #endregion

    }
}
