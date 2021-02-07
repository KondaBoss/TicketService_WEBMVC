using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TicketService.Data.Models;
using TicketService.Data.Services;
using TicketService.Data.ViewModels;

namespace TicketService.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly DataContext _context;
        public static ShoppingCart _myCart = new ShoppingCart()
        {
            ID = 1,
            Tickets = new List<ShoppingTicket>()
        };

        public CustomerController()
        {
            _context = new DataContext();
            HomeController homeController = new HomeController();
        }
        
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MyTickets()
        {
            if (!IsCustomer())
                return Content("<h1>Unautorized<h1>");

            Customer customer = (Customer)Session["customer"];

            var allEvents = (List<Event>)_context.Event.GetAll();
            var allTickets = (List<Ticket>)_context.Ticket.GetAll();

            var model = new List<TicketViewModel>();

            foreach (var ticket in allTickets.Where(x => x.CustomerID == customer.ID && !x.IsDeleted).ToList())
            {
                Event e = allEvents.SingleOrDefault(x => x.ID == ticket.EventID);
                model.Add(new TicketViewModel()
                {
                    ID = ticket.ID,
                    EventID = ticket.EventID,
                    CustomerID = ticket.CustomerID,
                    EventName = e.Name,
                    TicketPrice = ticket.Price,
                    EventDate = e.EventDate,
                    Type = ticket.Type,
                    Status = ticket.Status,
                    IsDeleted = ticket.IsDeleted
                }
                );
            }

            return View(model);
        }

        public ActionResult CancelTicket(int id)
        {
            if (!IsCustomer())
                return Content("<h1>Unautorized<h1>");
            
            List<Ticket> tickets = (List<Ticket>)_context.Ticket.GetAll();

            int cancelID = id - 1000000001;
            tickets[cancelID].Status = false;

            TicketData.SaveTickets(tickets);


            List<Customer> customers = (List<Customer>)_context.Customer.GetAll();
            Customer customer = (Customer)Session["customer"];

            int editID = customer.ID - 1;
            customers[editID].NumberOfCollectedPoints -= tickets[cancelID].Price / 1000 * 133 * 4;
            customer.NumberOfCollectedPoints -= tickets[cancelID].Price / 1000 * 133 * 4;

            if (customers[editID].NumberOfCollectedPoints < 0)
                customers[editID].NumberOfCollectedPoints = 0;

            if (customer.NumberOfCollectedPoints < 0)
                customer.NumberOfCollectedPoints = 0;

            if (customers[editID].NumberOfCollectedPoints >= 1500)
            {
                customers[editID].CustomerType = 3;
                customer.CustomerType = 3;
            }
            else if (customers[editID].NumberOfCollectedPoints >= 500)
            {
                customers[editID].CustomerType = 2;
                customer.CustomerType = 2;
            }
            else
            {
                customers[editID].CustomerType = 1;
                customer.CustomerType = 1;
            }

            if (++customers[editID].NumberOfTicketsCancelled >= 5)
                customers[editID].IsSuspitious = true;

            if (++customer.NumberOfTicketsCancelled >= 5)
                customer.IsSuspitious = true;
            

            CustomerData.SaveCustomers(customers);


            return RedirectToAction("MyTickets", "Customer");
        }


        [HttpGet]
        public ActionResult ShoppingCart()
        {
            if (!IsCustomer())
                return Content("<h1>Unautorized<h1>");

            Customer customer = (Customer)Session["customer"];

            _myCart.CustomerID = customer.ID;
            

            return View(_myCart);
        }

        [HttpPost]
        public ActionResult PostComment(int EventID, string commentText, int commentRating)
        {
            if (!IsCustomer())
                return Content("<h1>Unautorized<h1>");

            Customer customer = (Customer)Session["customer"];

            Comment c = new Comment()
            {
                ID = ++HomeController.commentID,
                EventID = EventID,
                CustomerID = customer.ID,
                Text = commentText,
                Rating = commentRating,
                IsDeleted = false
            };

            CommentData.SaveComment(c);

            return RedirectToAction($"EventDetails/{EventID}", "Home");
        }

        public JsonResult IncreaseNumberOfTickets(int TicketID)
        {
            var model = _myCart.Tickets.SingleOrDefault(x => x.ID == TicketID);

            if (model.NumberOfTicketsToBuy + 1 <= model.NumberOfTicketsRemaining)
                model.NumberOfTicketsToBuy++;
            
            object obj = new
            {
                NumberOfTicketsToBuy = model.NumberOfTicketsToBuy,
                TicketPrice = model.NumberOfTicketsToBuy * model.SingleTicketPrice,
                TotalPrice = _myCart.TotalPrice
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DecreaseNumberOfTickets(int TicketID)
        {
            var model = _myCart.Tickets.SingleOrDefault(x => x.ID == TicketID);

            if (model.NumberOfTicketsToBuy > 1)
                model.NumberOfTicketsToBuy--;

            object obj = new
            {
                NumberOfTicketsToBuy = model.NumberOfTicketsToBuy,
                TicketPrice = model.NumberOfTicketsToBuy * model.SingleTicketPrice,
                TotalPrice = _myCart.TotalPrice
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveTicket(int TicketID)
        {
            if (_myCart.Tickets.Where(x => x.ID == TicketID).ToList().Count > 0)
            {
                var deleteTicket = _myCart.Tickets.SingleOrDefault(x => x.ID == TicketID);
                _myCart.Tickets.Remove(deleteTicket);
            }
            
            return Json(_myCart.TotalPrice, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeTicketType(int EventID, string TicketType)
        {
            var myEvent = _context.Event.GetAll().SingleOrDefault(x => x.ID == EventID);

            double price;

            if (TicketType == "Regular")
                price = myEvent.TicketPrice;
            else if (TicketType == "Fanpit")
                price = myEvent.TicketPrice * 2;
            else
                price = myEvent.TicketPrice * 4;

            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddToCart(int EventID, string TicketType, double TicketPrice)
        {
            ETicketType typeOfTicket = ETicketType.REGULAR;
            if (TicketType == "Fanpit")
            {
                TicketPrice = TicketPrice * 2;
                typeOfTicket = ETicketType.FANPIT;
            }
            else if (TicketType == "VIP")
            {
                TicketPrice = TicketPrice * 4;
                typeOfTicket = ETicketType.VIP;
            }

            var ticket = _myCart.Tickets.SingleOrDefault(x => x.EventID == EventID && x.TypeOfTicket == typeOfTicket);


            if (ticket == null)
            {
                Customer customer = (Customer)Session["customer"];

                double discount = 1.0;

                if (customer.CustomerType == 2) // Silver
                    discount = 0.9;
                else if (customer.CustomerType == 3) // Gold
                    discount = 0.8;

                var e = _context.Event.GetAll().SingleOrDefault(x => x.ID == EventID);
                int sold = _context.Ticket.GetAll().Where(x => x.EventID == EventID && x.Status).ToList().Count;

                _myCart.Tickets.Add(new ShoppingTicket()
                {
                    ID = _myCart.Tickets.Count + 1,
                    EventID = EventID,
                    EventName = e.Name,
                    NumberOfTicketsRemaining = e.Capacity - sold,
                    NumberOfTicketsToBuy = 1,
                    SingleTicketPrice = TicketPrice * discount,
                    TypeOfTicket = typeOfTicket
                });
            }
            else
            {
                ticket.NumberOfTicketsToBuy++;
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmPurchase()
        {
            if (_myCart.Tickets.Count == 0)
            {
                return Json("empty-cart", JsonRequestBehavior.AllowGet);
            }

            var totalPrice = _myCart.TotalPrice;
            var customerID = _myCart.CustomerID;
            int salesmanID = 0;

            foreach (var item in _myCart.Tickets)
            {
                salesmanID = _context.Event.GetAll().SingleOrDefault(x => x.ID == item.EventID).SalesmanID;
                for (int i = 0; i < item.NumberOfTicketsToBuy; i++)
                {
                    TicketData.SaveTicket(new Ticket()
                    {
                        ID = ++HomeController.ticketID,
                        CustomerID = _myCart.CustomerID,
                        EventID = item.EventID,
                        SalesmanID = salesmanID,
                        Type = item.TypeOfTicket,
                        Price = item.SingleTicketPrice,
                        Status = true,
                        IsDeleted = false
                    });
                }
            }

            List<Customer> customers = (List<Customer>)_context.Customer.GetAll();
            Customer customer = (Customer)Session["customer"];

            int editID = customer.ID - 1;
            customers[editID].NumberOfCollectedPoints += _myCart.TotalPrice / 1000 * 133;
            customer.NumberOfCollectedPoints += _myCart.TotalPrice / 1000 * 133;

            if (customers[editID].NumberOfCollectedPoints >= 1500)
            {
                customers[editID].CustomerType = 3;
                customer.CustomerType = 3;
            }
            else if (customers[editID].NumberOfCollectedPoints >= 500)
            {
                customers[editID].CustomerType = 2;
                customer.CustomerType = 2;
            }
            else
            {
                customers[editID].CustomerType = 1;
                customer.CustomerType = 1;
            }

            CustomerData.SaveCustomers(customers);

            _myCart.Tickets.Clear();

            return Json("", JsonRequestBehavior.AllowGet);
        }



        #region Search

        public JsonResult GetSearchingData(string SearchEventNameValue, string SearchDateFromValue, string SearchDateToValue, string SearchPriceFromValue, string SearchPriceToValue, string EventType, string TicketStatus)
        {
            Customer customer = (Customer)Session["customer"];

            var allEvents = (List<Event>)_context.Event.GetAll();
            var allTickets = (List<Ticket>)_context.Ticket.GetAll();

            var model = new List<TicketViewModel>();

            foreach (var ticket in allTickets.Where(x => x.CustomerID == customer.ID && !x.IsDeleted).ToList())
            {
                Event e = allEvents.SingleOrDefault(x => x.ID == ticket.EventID);
                model.Add(new TicketViewModel()
                {
                    ID = ticket.ID,
                    EventID = ticket.EventID,
                    CustomerID = ticket.CustomerID,
                    EventName = e.Name,
                    TicketPrice = ticket.Price,
                    EventDate = e.EventDate,
                    Type = ticket.Type,
                    Status = ticket.Status,
                    IsDeleted = ticket.IsDeleted
                }
                );
            }

            if (!string.IsNullOrEmpty(SearchEventNameValue))
            {
                model = model.Where(x => x.EventName.ToLower().Contains(SearchEventNameValue.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(SearchDateFromValue))
            {
                DateTime dateFrom = new DateTime();
                try
                {
                    dateFrom = DateTime.ParseExact(SearchDateFromValue, "yyyy-MM-ddTHH:mm",
                                           System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new ArgumentException("DateTime couldnt parse", "datetime");
                }
                model = model.Where(x => x.EventDate >= dateFrom).ToList();
            }
            if (!string.IsNullOrEmpty(SearchDateToValue))
            {
                DateTime dateTo = new DateTime();
                try
                {
                    dateTo = DateTime.ParseExact(SearchDateToValue, "yyyy-MM-ddTHH:mm",
                                           System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new ArgumentException("DateTime couldnt parse", "datetime");
                }
                model = model.Where(x => x.EventDate <= dateTo).ToList();
            }
            if (!string.IsNullOrEmpty(SearchPriceFromValue))
            {
                try
                {
                    double priceFrom = Convert.ToDouble(SearchPriceFromValue);
                    model = model.Where(x => x.TicketPrice >= priceFrom).ToList();
                }
                catch (FormatException)
                {
                    throw new FormatException();
                }
            }
            if (!string.IsNullOrEmpty(SearchPriceToValue))
            {
                try
                {
                    double priceTo = Convert.ToDouble(SearchPriceToValue);
                    model = model.Where(x => x.TicketPrice <= priceTo).ToList();
                }
                catch (FormatException)
                {
                    throw new FormatException();
                }
            }

            // Filter


            if (EventType == "Regular")
            {
                model = model.Where(x => x.Type == ETicketType.REGULAR).ToList();
            }
            else if (EventType == "Fanpit")
            {
                model = model.Where(x => x.Type == ETicketType.FANPIT).ToList();
            }
            else if (EventType == "VIP")
            {
                model = model.Where(x => x.Type == ETicketType.VIP).ToList();
            }

            if (TicketStatus == "Reserved")
            {
                model = model.Where(x => x.Status == true).ToList();
            }
            else if (TicketStatus == "Cancelled")
            {
                model = model.Where(x => x.Status == false).ToList();
            }

            StringBuilder sb = new StringBuilder();

            foreach (var item in model)
            {
                sb
                    .Append("<tr>")
                    .Append("<td>")
                    .Append(item.EventName)
                    .Append("</td>")
                    .Append("<td>")
                    .Append(item.TicketPrice)
                    .Append("</td>")
                    .Append("<td>")
                    .Append(item.EventDate.ToString("dd.MM.yyyy, HH:mm"))
                    .Append("</td>")
                    .Append("<td>")
                    .Append(item.Status ? "Reserved" : "Cancelled")
                    .Append("</td>");
                
                if (item.Status && DateTime.Now <= item.EventDate.AddDays(-7))
                {
                    sb
                        .Append("<td>")
                        .Append("<a href=\"/Customer/CancelTicket/")
                        .Append(item.ID)
                        .Append("\">Cancel</a>")
                        .Append("</td>")
                        .Append("</tr>");
                }
                else
                {
                    sb
                        .Append("<td>")
                        .Append("</td>")
                        .Append("</tr>");
                }
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion



        private bool IsCustomer()
        {
            Customer customer = (Customer)Session["customer"];
            if (customer == null || customer.Role != EUserRole.Customer || customer.UserName == string.Empty)
                return false;

            return true;
        }
    }
}