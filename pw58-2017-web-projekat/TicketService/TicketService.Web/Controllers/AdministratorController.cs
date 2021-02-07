using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketService.Data.Models;
using TicketService.Data.Services;
using TicketService.Data.ViewModels;

namespace TicketService.Web.Controllers
{
    public class AdministratorController : Controller
    {
        //public static int salesmanID = 0;
        private readonly DataContext _context;

        public AdministratorController()
        {
            _context = new DataContext();
            HomeController homeController = new HomeController();
        }

        [HttpGet]
        public ActionResult CreateNewSalesman()
        {
            if (!IsAdministrator())
                return Content("<h1>Unauthorized<h1>");
            
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewSalesman(Salesman newSalesman)
        {
            if (!IsAdministrator())
                return Content("<h1>Unauthorized<h1>");

            Salesman s = new Salesman()
            {
                ID = ++HomeController.salesmanID,
                FirstName = newSalesman.FirstName,
                LastName = newSalesman.LastName,
                UserName = newSalesman.UserName,
                BirthDate = newSalesman.BirthDate,
                Gender = newSalesman.Gender,
                Password = newSalesman.Password,
                IsDeleted = false,
                Role = EUserRole.Salesman
            };

            if (
                s.FirstName == string.Empty ||
                s.LastName == string.Empty ||
                s.UserName == string.Empty ||
                s.Password == string.Empty ||
                s.BirthDate == null
                )
            {
                ViewBag.Message = $"All fields must be filled!";
                return View();
            }
            
            List<Salesman> salesmen = (List<Salesman>)HttpContext.Application["salesmen"];

            for (int i = 0; i < salesmen.Count; i++)
            {
                if (salesmen[i].UserName == s.UserName)
                {
                    ViewBag.Message = $"Username is already taken!";
                    return View();
                }
            }

            salesmen.Add(s);
            SalesmanData.SaveSalesman(s);
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AllUsers()
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            var model = new List<UserDTO>();

            List<Administrator> admins = _context.Administrator.GetAll().OrderBy(x => x.UserName).ToList();
            List<Salesman> salesmen = _context.Salesman.GetAll().OrderBy(x => x.UserName).ToList();
            List<Customer> customers = _context.Customer.GetAll().OrderBy(x => x.UserName).ToList();

            foreach (Administrator a in admins)
            {
                model.Add(new UserDTO()
                {
                    ID = a.ID,
                    UserName = a.UserName,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    BirthDate = a.BirthDate,
                    Gender = a.Gender,
                    Role = EUserRole.Administrator,
                    NumberOfCollectedPoints = null,
                    CustomerType = 0,
                    NumberOfTicketsCancelled = null,
                    IsSuspitious = false,
                    IsDeleted = false
                });
            }
            foreach (Salesman s in salesmen)
            {
                model.Add(new UserDTO()
                {
                    ID = s.ID,
                    UserName = s.UserName,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    BirthDate = s.BirthDate,
                    Gender = s.Gender,
                    Role = EUserRole.Salesman,
                    NumberOfCollectedPoints = null,
                    CustomerType = 0,
                    NumberOfTicketsCancelled = null,
                    IsSuspitious = false,
                    IsDeleted = s.IsDeleted
                });
            }
            foreach (Customer c in customers)
            {
                model.Add(new UserDTO()
                {
                    ID = c.ID,
                    UserName = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    BirthDate = c.BirthDate,
                    Gender = c.Gender,
                    Role = EUserRole.Customer,
                    NumberOfCollectedPoints = c.NumberOfCollectedPoints,
                    CustomerType = c.CustomerType,
                    NumberOfTicketsCancelled = c.NumberOfTicketsCancelled,
                    IsSuspitious = c.IsSuspitious,
                    IsDeleted = c.IsDeleted
                });
            }

            return View(model);
        }

        public ActionResult AllTickets()
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            var allEvents = (List<Event>)_context.Event.GetAll();
            var allCustomers = (List<Customer>)_context.Customer.GetAll();
            var allTickets = (List<Ticket>)_context.Ticket.GetAll();

            var model = new List<TicketViewModel>();

            foreach (var ticket in allTickets)
            {
                Event e = allEvents.SingleOrDefault(x => x.ID == ticket.EventID);
                Customer c = allCustomers.SingleOrDefault(x => x.ID == ticket.CustomerID);
                model.Add(new TicketViewModel()
                                              {
                                                  ID = ticket.ID,
                                                  EventID = ticket.EventID,
                                                  CustomerID = ticket.CustomerID,
                                                  EventName = e.Name,
                                                  TicketPrice = e.TicketPrice,
                                                  EventDate = e.EventDate,
                                                  CustomerName = c.FirstName + " " + c.LastName,
                                                  Type = ticket.Type,
                                                  Status = ticket.Status,
                                                  IsDeleted = ticket.IsDeleted
                                              }
                );
            }

            return View(model);
        }

        public JsonResult ChangeEventStatus(int EventID)
        {
            List<Event> events = (List<Event>)_context.Event.GetAll();

            int approveID = EventID - 1;

            events[approveID].Status = !events[approveID].Status;

            EventData.SaveEvents(events);

            string status;

            if (events[approveID].Status == true)
                if (events[approveID].EventDate < DateTime.Now)
                    status = "Finished";
                else
                    status = "Active";
            else
                status = "Inactive";
            

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEvent(int EventID)
        {
            List<Event> events = (List<Event>)_context.Event.GetAll();

            int deleteID = EventID - 1;

            events[deleteID].IsDeleted = !events[deleteID].IsDeleted;

            EventData.SaveEvents(events);

            string status;

            if (events[deleteID].IsDeleted == true)
                status = "Yes";
            else
                status = "No";

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUser(int UserID, string UserRole)
        {
            string status;

            if (UserRole == "Administrator")
            {
                // Cant delete another administrator

                object obj = new
                {
                    Status = "Active",
                    Message = true
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else if (UserRole == "Salesman")
            {
                List<Salesman> salesmen = (List<Salesman>)_context.Salesman.GetAll();

                int deleteID = UserID - 1;
                salesmen[deleteID].IsDeleted = !salesmen[deleteID].IsDeleted;
                SalesmanData.SaveSalesmen(salesmen);

                if (salesmen[deleteID].IsDeleted == true)
                    status = "Blocked";
                else
                    status = "Active";

                object obj = new
                {
                    Status = status,
                    Message = false
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<Customer> customers = (List<Customer>)_context.Customer.GetAll();

                int deleteID = UserID - 1;
                customers[deleteID].IsDeleted = !customers[deleteID].IsDeleted;
                CustomerData.SaveCustomers(customers);

                if (customers[deleteID].IsDeleted == true)
                    status = "Blocked";
                else
                    status = "Active";

                object obj = new
                {
                    Status = status,
                    Message = false
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteTicket(int id)
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            List<Ticket> tickets = (List<Ticket>)_context.Ticket.GetAll();

            int deleteID = id - 1000000001;
            tickets[deleteID].IsDeleted = !tickets[deleteID].IsDeleted;

            TicketData.SaveTickets(tickets);

            return RedirectToAction("AllTickets", "Administrator");
        }

        public ActionResult BlockCustomer(int id)
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            List<Customer> customers = (List<Customer>)_context.Customer.GetAll();

            int deleteID = id - 1;
            customers[deleteID].IsDeleted = true;

            CustomerData.SaveCustomers(customers);

            return RedirectToAction($"CustomerDetails/{id}", "Administrator");
        }

        public ActionResult UnblockCustomer(int id)
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            List<Customer> customers = (List<Customer>)_context.Customer.GetAll();

            int deleteID = id - 1;
            customers[deleteID].IsDeleted = false;

            CustomerData.SaveCustomers(customers);

            return RedirectToAction($"CustomerDetails/{id}", "Administrator");
        }

        public ActionResult CustomerDetails(int id)
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            List<Customer> customers = (List<Customer>)_context.Customer.GetAll();

            var model = customers.SingleOrDefault(x => x.ID == id);

            return View(model);
        }

        public ActionResult UserDetails(int id, EUserRole role)
        {
            if (!IsAdministrator())
                return Content("<h1>Unautorized<h1>");

            UserDTO user = new UserDTO();

            if (role == EUserRole.Administrator)
            {
                var a = ((List<Administrator>)_context.Administrator.GetAll()).SingleOrDefault(x => x.ID == id);

                user.ID = a.ID;
                user.UserName = a.UserName;
                user.FirstName = a.FirstName;
                user.LastName = a.LastName;
                user.BirthDate = a.BirthDate;
                user.Gender = a.Gender;
                user.Role = EUserRole.Administrator;
                user.NumberOfCollectedPoints = null;
                user.CustomerType = 0;
                user.NumberOfTicketsCancelled = null;
                user.IsSuspitious = false;
                user.IsDeleted = false;
            }
            else if (role == EUserRole.Salesman)
            {
                var s = ((List<Salesman>)_context.Salesman.GetAll()).SingleOrDefault(x => x.ID == id);
                
                user.ID = s.ID;
                user.UserName = s.UserName;
                user.FirstName = s.FirstName;
                user.LastName = s.LastName;
                user.BirthDate = s.BirthDate;
                user.Gender = s.Gender;
                user.Role = EUserRole.Salesman;
                user.NumberOfCollectedPoints = null;
                user.CustomerType = 0;
                user.NumberOfTicketsCancelled = null;
                user.IsSuspitious = false;
                user.IsDeleted = s.IsDeleted;
            }
            else
            {
                var c = ((List<Customer>)_context.Customer.GetAll()).SingleOrDefault(x => x.ID == id);
                
                user.ID = c.ID;
                user.UserName = c.UserName;
                user.FirstName = c.FirstName;
                user.LastName = c.LastName;
                user.BirthDate = c.BirthDate;
                user.Gender = c.Gender;
                user.Role = EUserRole.Customer;
                user.NumberOfCollectedPoints = c.NumberOfCollectedPoints;
                user.CustomerType = c.CustomerType;
                user.NumberOfTicketsCancelled = c.NumberOfTicketsCancelled;
                user.IsSuspitious = c.IsSuspitious;
                user.IsDeleted = c.IsDeleted;
            }

            return View(user);
        }

        #region Search

        public JsonResult GetSearchingData(string SearchUsernameValue, string SearchFirstNameValue, string SearchLastNameValue, string SearchCollectedPointsFromValue, string SearchCollectedPointsToValue, string UserRole, string UserType)
        {
            var model = new List<UserDTO>();

            List<Administrator> admins = _context.Administrator.GetAll().OrderBy(x => x.UserName).ToList();
            List<Salesman> salesmen = _context.Salesman.GetAll().OrderBy(x => x.UserName).ToList();
            List<Customer> customers = _context.Customer.GetAll().OrderBy(x => x.UserName).ToList();

            foreach (Administrator a in admins)
            {
                model.Add(new UserDTO()
                {
                    ID = a.ID,
                    UserName = a.UserName,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    BirthDate = a.BirthDate,
                    Gender = a.Gender,
                    Role = EUserRole.Administrator,
                    NumberOfCollectedPoints = null,
                    CustomerType = 0,
                    NumberOfTicketsCancelled = null,
                    IsSuspitious = false,
                    IsDeleted = false
                });
            }
            foreach (Salesman s in salesmen)
            {
                model.Add(new UserDTO()
                {
                    ID = s.ID,
                    UserName = s.UserName,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    BirthDate = s.BirthDate,
                    Gender = s.Gender,
                    Role = EUserRole.Salesman,
                    NumberOfCollectedPoints = null,
                    CustomerType = 0,
                    NumberOfTicketsCancelled = null,
                    IsSuspitious = false,
                    IsDeleted = s.IsDeleted
                });
            }
            foreach (Customer c in customers)
            {
                model.Add(new UserDTO()
                {
                    ID = c.ID,
                    UserName = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    BirthDate = c.BirthDate,
                    Gender = c.Gender,
                    Role = EUserRole.Customer,
                    NumberOfCollectedPoints = c.NumberOfCollectedPoints,
                    CustomerType = c.CustomerType,
                    NumberOfTicketsCancelled = c.NumberOfTicketsCancelled,
                    IsSuspitious = c.IsSuspitious,
                    IsDeleted = c.IsDeleted
                });
            }


            if (!string.IsNullOrEmpty(SearchUsernameValue))
            {
                model = model.Where(x => x.UserName.ToLower().Contains(SearchUsernameValue.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(SearchFirstNameValue))
            {
                model = model.Where(x => x.FirstName.ToLower().Contains(SearchFirstNameValue.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(SearchLastNameValue))
            {
                model = model.Where(x => x.LastName.ToLower().Contains(SearchLastNameValue.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(SearchCollectedPointsFromValue))
            {
                try
                {
                    double collectedPointsFrom = Convert.ToDouble(SearchCollectedPointsFromValue);
                    model = model.Where(x => x.NumberOfCollectedPoints >= collectedPointsFrom).ToList();
                }
                catch (FormatException)
                {
                    //throw new FormatException();
                }
            }

            if (!string.IsNullOrEmpty(SearchCollectedPointsToValue))
            {
                try
                {
                    double collectedPointsTo = Convert.ToDouble(SearchCollectedPointsToValue);
                    model = model.Where(x => x.NumberOfCollectedPoints <= collectedPointsTo).ToList();
                }
                catch (FormatException)
                {
                    //throw new FormatException();
                }
            }


            // Filter

            if (UserRole == "Administrator")
            {
                model = model.Where(x => x.Role == EUserRole.Administrator).ToList();
            }
            else if (UserRole == "Salesman")
            {
                model = model.Where(x => x.Role == EUserRole.Salesman).ToList();
            }
            else if (UserRole == "Customer")
            {
                model = model.Where(x => x.Role == EUserRole.Customer).ToList();
            }

            if (UserType == "Bronze")
            {
                model = model.Where(x => x.CustomerType == 1).ToList();
            }
            else if (UserType == "Silver")
            {
                model = model.Where(x => x.CustomerType == 2).ToList();
            }
            else if (UserType == "Gold")
            {
                model = model.Where(x => x.CustomerType == 3).ToList();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private bool IsAdministrator()
        {
            Administrator admin = (Administrator)Session["administrator"];
            if (admin == null || admin.Role != EUserRole.Administrator || admin.UserName == string.Empty)
                return false;

            return true;
        }
    }
}