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
    public class HomeController : Controller
    {
        #region Fields
        private readonly DataContext _context;
        public static int administratorID = 0;
        public static int commentID = 0;
        public static int customerID = 0;
        public static int customertypeID = 0;
        public static int eventID = 0;
        public static int locationID = 0;
        public static int salesmanID = 0;
        public static int ticketID = 0;
        public static int venueID = 0;
        #endregion

        public HomeController()
        {
            _context = new DataContext();
            administratorID = _context.Administrator.GetAll().Count();
            commentID = _context.Comment.GetAll().Count();
            customerID = _context.Customer.GetAll().Count();
            customertypeID = _context.CustomerType.GetAll().Count();
            eventID = _context.Event.GetAll().Count();
            locationID = _context.Location.GetAll().Count();
            salesmanID = _context.Salesman.GetAll().Count();
            ticketID = _context.Ticket.GetAll().Count() + 1000000000;
            venueID = _context.Venue.GetAll().Count();
        }

        public ActionResult Index()
        {
            var allEvents = _context.Event.GetAll();
            var allVenues = _context.Venue.GetAll();

            var model = new List<EventViewModel>();
            if (CheckUserRole() == EUserRole.Administrator)
            {
                foreach (var item in allEvents)
                {
                    var v = allVenues.SingleOrDefault(x => x.ID == item.VenueID);

                    model.Add(new EventViewModel()
                    {
                        ID = item.ID,
                        SalesmanID = item.SalesmanID,
                        VenueID = item.VenueID,
                        Name = item.Name,
                        EventType = item.EventType,
                        EventDate = item.EventDate,
                        VenueName = v.Name,
                        VenueAddress = v.Address + ", " + v.City + ", " + v.ZIPCode,
                        Poster = item.Poster,
                        TicketPrice = item.TicketPrice,
                        Rating = GetEventRating(item.ID),
                        Status = item.Status,
                        IsDeleted = item.IsDeleted
                    }
                        );
                }
            }
            else
            {
                foreach (var item in allEvents)
                {
                    if (!item.IsDeleted && item.Status)
                    {
                        var v = allVenues.SingleOrDefault(x => x.ID == item.VenueID);

                        model.Add(new EventViewModel()
                        {
                            ID = item.ID,
                            SalesmanID = item.SalesmanID,
                            VenueID = item.VenueID,
                            Name = item.Name,
                            EventType = item.EventType,
                            EventDate = item.EventDate,
                            VenueName = v.Name,
                            VenueAddress = v.Address + ", " + v.City + ", " + v.ZIPCode,
                            Poster = item.Poster,
                            TicketPrice = item.TicketPrice,
                            Rating = GetEventRating(item.ID),
                            Status = item.Status,
                        }
                    );
                    }
                }
            }

            ViewBag.UserRole = CheckUserRole().ToString();

            if (ViewBag.UserRole == "Administrator")
                ViewBag.Layout = $"~/Views/Shared/_Layout_Administrator.cshtml";
            else if (ViewBag.UserRole == "Salesman")
                ViewBag.Layout = $"~/Views/Shared/_Layout_Salesman.cshtml";
            else if (ViewBag.UserRole == "Customer")
                ViewBag.Layout = $"~/Views/Shared/_Layout_Customer.cshtml";
            else //if (ViewBag.UserRole == "Guest")
                ViewBag.Layout = $"~/Views/Shared/_Layout.cshtml";


            var modelpart1 = model.Where(x => x.EventDate > DateTime.Now).ToList();
            var modelpart2 = model.Where(x => x.EventDate <= DateTime.Now).ToList();

            modelpart1 = modelpart1.OrderBy(x => x.EventDate).ToList();
            modelpart2 = modelpart2.OrderByDescending(x => x.EventDate).ToList();

            model = modelpart1.Concat(modelpart2).ToList();

            return View(model);
        }
        
        public ActionResult EventDetails(int id)
        {
            var allEvents = _context.Event.GetAll();
            var allVenues = _context.Venue.GetAll();
            var allTickets = _context.Ticket.GetAll();
            var allComments = _context.Comment.GetAll();
            var allCustomers = _context.Customer.GetAll();


            Event e = allEvents.SingleOrDefault(x => x.ID == id);
            Venue v = allVenues.SingleOrDefault(x => x.ID == e.VenueID);
            List<Comment> comments = allComments.Where(x => x.EventID == e.ID && !x.IsDeleted).ToList();

            List<CommentViewModel> commentsViewModel = new List<CommentViewModel>();

            foreach (var item in comments)
            {
                commentsViewModel.Add(new CommentViewModel()
                {
                    ID = item.ID,
                    CustomerUsername = allCustomers.SingleOrDefault(x => x.ID == item.CustomerID).UserName,
                    Text = item.Text,
                    Rating = item.Rating
                });
            }

            int numberOfTicketsSold = allTickets.Where(x => x.EventID == e.ID && x.Status).ToList().Count;
            int numberOfRemainingTickets = e.Capacity - numberOfTicketsSold;

            var model = new EventViewModel()
            {
                ID = e.ID,
                SalesmanID = e.SalesmanID,
                VenueID = e.VenueID,
                Capacity = e.Capacity,
                NumberOfRemainingTickets = numberOfRemainingTickets,
                Name = e.Name,
                EventType = e.EventType,
                EventDate = e.EventDate,
                VenueName = v.Name,
                VenueAddress = v.Address + ", " + v.City + ", " + v.ZIPCode,
                Poster = e.Poster,
                TicketPrice = e.TicketPrice,
                Rating = GetEventRating(e.ID),
                Status = e.Status,
                CommentList = commentsViewModel
            };

            ViewBag.UserRole = CheckUserRole().ToString();
            ViewBag.CanComment = "False";

            if (ViewBag.UserRole == "Administrator")
                ViewBag.Layout = "~/Views/Shared/_Layout_Administrator.cshtml";
            else if (ViewBag.UserRole == "Salesman")
                ViewBag.Layout = "~/Views/Shared/_Layout_Salesman.cshtml";
            else if (ViewBag.UserRole == "Customer")
            {
                ViewBag.Layout = "~/Views/Shared/_Layout_Customer.cshtml";

                var ticketsList = allTickets.Where(x => x.EventID == e.ID).ToList();

                Customer customer = (Customer)Session["customer"];


                if (ticketsList.Where(x => x.CustomerID == customer.ID).ToList().Count > 0 && e.EventDate < DateTime.Now
                    && comments.Where(x => x.CustomerID == customer.ID && !x.IsDeleted).ToList().Count < 1)
                    ViewBag.CanComment = "True";
            }
            else
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";

            return View(model);
        }

        #region Search

        public JsonResult GetSearchingData(string SearchEventNameValue, string SearchVenueValue, string SearchAddressValue, string SearchDateFromValue, string SearchDateToValue, string SearchPriceFromValue, string SearchPriceToValue, string EventType, string TicketStatus)
        {
            var allEvents = _context.Event.GetAll();
            var allVenues = _context.Venue.GetAll();

            var model = new List<EventViewModel>();
            if (CheckUserRole() == EUserRole.Administrator)
            {
                foreach (var item in allEvents)
                {
                    var v = allVenues.SingleOrDefault(x => x.ID == item.VenueID);

                    model.Add(new EventViewModel()
                    {
                        ID = item.ID,
                        SalesmanID = item.SalesmanID,
                        VenueID = item.VenueID,
                        Name = item.Name,
                        EventType = item.EventType,
                        EventDate = item.EventDate,
                        VenueName = v.Name,
                        VenueAddress = v.Address + ", " + v.City + ", " + v.ZIPCode,
                        Poster = item.Poster,
                        TicketPrice = item.TicketPrice,
                        Capacity = item.Capacity,
                        Rating = GetEventRating(item.ID),
                        Status = item.Status,
                        IsDeleted = item.IsDeleted
                    }
                        );
                }
            }
            else
            {
                foreach (var item in allEvents)
                {
                    if (!item.IsDeleted && item.Status)
                    {
                        var v = allVenues.SingleOrDefault(x => x.ID == item.VenueID);

                        model.Add(new EventViewModel()
                        {
                            ID = item.ID,
                            SalesmanID = item.SalesmanID,
                            VenueID = item.VenueID,
                            Name = item.Name,
                            EventType = item.EventType,
                            EventDate = item.EventDate,
                            VenueName = v.Name,
                            VenueAddress = v.Address + ", " + v.City + ", " + v.ZIPCode,
                            Poster = item.Poster,
                            TicketPrice = item.TicketPrice,
                            Capacity = item.Capacity,
                            Rating = GetEventRating(item.ID),
                            Status = item.Status,
                        }
                    );
                    }
                }
            }

            model = model.OrderByDescending(x => x.EventDate).ToList();

            if (!string.IsNullOrEmpty(SearchEventNameValue))
            {
                model = model.Where(x => x.Name.ToLower().Contains(SearchEventNameValue.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(SearchVenueValue))
            {
                model = model.Where(x => x.VenueName.ToLower().Contains(SearchVenueValue.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(SearchAddressValue))
            {
                model = model.Where(x => x.VenueAddress.ToLower().Contains(SearchAddressValue.ToLower())).ToList();
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
            
            
            if (EventType == "Concert")
            {
                model = model.Where(x => x.EventType == EEventType.Concert).ToList();
            }
            else if (EventType == "Festival")
            {
                model = model.Where(x => x.EventType == EEventType.Festival).ToList();
            }
            else if (EventType == "Theater")
            {
                model = model.Where(x => x.EventType == EEventType.Theater).ToList();
            }
            else if (EventType == "Party")
            {
                model = model.Where(x => x.EventType == EEventType.Party).ToList();
            }

            if (TicketStatus == "Available")
            {
                var allTickets = _context.Ticket.GetAll();
                int ticketsSold = 0;
                foreach (var item in model.ToList())
                {
                    ticketsSold = allTickets.Where(x => x.EventID == item.ID).ToList().Count;
                    if (item.Capacity - ticketsSold <= 0)
                    {
                        model.Remove(item);
                    }
                }
            }
            else if (TicketStatus == "Unavailable")
            {
                var allTickets = _context.Ticket.GetAll();
                int ticketsSold = 0;
                foreach (var item in model.ToList())
                {
                    ticketsSold = allTickets.Where(x => x.EventID == item.ID).ToList().Count;
                    if (item.Capacity - ticketsSold > 0)
                    {
                        model.Remove(item);
                    }
                }
            }

            var modelpart1 = model.Where(x => x.EventDate > DateTime.Now).ToList();
            var modelpart2 = model.Where(x => x.EventDate <= DateTime.Now).ToList();

            modelpart1 = modelpart1.OrderBy(x => x.EventDate).ToList();
            modelpart2 = modelpart2.OrderByDescending(x => x.EventDate).ToList();

            model = modelpart1.Concat(modelpart2).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in model)
            {
                sb
                .Append("<tr>")
                .Append("<td>")
                .Append(item.Name)
                .Append("</td>")
                .Append("<td>")
                .Append(item.EventType)
                .Append("</td>")
                .Append("<td>")
                .Append(item.EventDate.ToString("dd.MM.yyyy, HH:mm"))
                .Append("</td>")
                .Append("<td>")
                .Append(item.VenueName)
                .Append("</td>")
                .Append("<td>")
                .Append(item.VenueAddress)
                .Append("</td>")
                .Append("<td>")
                .Append("<img src=\"/Images/")
                .Append(item.Poster)
                .Append("\" alt=\"HTML5 Icon\" style=\"width:128px;height:128px;\">")
                .Append("</td>")
                .Append("<td>")
                .Append("$")
                .Append(item.TicketPrice)
                .Append("</td>");

                if (item.Rating == -1)
                {
                    sb
                    .Append("<td>")
                    .Append("-")
                    .Append("</td>");
                }
                else
                {
                    sb
                    .Append("<td>")
                    .Append(item.Rating)
                    .Append("</td>");
                }
                

                if (CheckUserRole() == EUserRole.Administrator)
                {
                    sb

                        .Append("<td>")
                        .Append(item.Status ? "Active" : "Inactive")
                        .Append("</td>")
                        .Append("<td>")
                        .Append(item.IsDeleted ? "Yes" : "No")
                        .Append("</td>");
                }
                
                sb
                    .Append("<td>")
                    .Append("<a href=\"/Home/EventDetails/")
                    .Append(item.ID)
                    .Append("\">Details</a>")
                    .Append("</td>")
                    .Append("</tr>");
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion




        private EUserRole CheckUserRole()
        {
            Customer customer = (Customer)Session["customer"];
            if (customer == null || customer.UserName.Equals(string.Empty) || customer.Role != EUserRole.Customer)
            {
                Salesman salesman = (Salesman)Session["salesman"];
                if (salesman == null || salesman.UserName.Equals(string.Empty) || salesman.Role != EUserRole.Salesman)
                {
                    Administrator administrator = (Administrator)Session["administrator"];
                    if (administrator == null || administrator.UserName.Equals(string.Empty) || administrator.Role != EUserRole.Administrator)
                        return EUserRole.Guest;
                    else
                        return EUserRole.Administrator;
                }
                else
                    return EUserRole.Salesman;
            }
            else
                return EUserRole.Customer;
        }

        private double GetEventRating(int EventID)
        {
            int counter = 0;
            int sum = 0;

            var comments = _context.Comment.GetAll().Where(x => x.EventID == EventID && !x.IsDeleted);

            foreach (var item in comments)
            {
                sum += item.Rating;
                counter++;
            }

            if (counter == 0)
                return -1;

            return Math.Round(((double)sum / counter), 1);
        }
    }
}