using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TicketService.Data.Models;
using TicketService.Data.Services;
using TicketService.Data.ViewModels;

namespace TicketService.Web.Controllers
{
    public class SalesmanController : Controller
    {
        private readonly DataContext _context;

        public SalesmanController()
        {
            _context = new DataContext();
            HomeController homeController = new HomeController();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MyEvents()
        {
            if (!IsSalesman())
                return Content("<h1>Unauthorized<h1>");

            Salesman salesman = (Salesman)Session["salesman"];

            var allEvents = _context.Event.GetAllForSalesman(salesman.ID);
            var allVenues = _context.Venue.GetAll();

            var model = new List<EventViewModel>();

            foreach (var item in allEvents)
            {
                if (!item.IsDeleted)
                {
                    model.Add(new EventViewModel()
                    {
                        ID = item.ID,
                        SalesmanID = item.SalesmanID,
                        VenueID = item.VenueID,
                        Name = item.Name,
                        EventType = item.EventType,
                        EventDate = item.EventDate,
                        VenueName = allVenues.SingleOrDefault(x => x.ID == item.VenueID).Name,
                        Poster = item.Poster,
                        TicketPrice = item.TicketPrice,
                        Rating = GetEventRating(item.ID),
                        Status = item.Status,
                    }
                );
                }
            }

            return View(model);
        }

        public ActionResult MyTickets()
        {
            if (!IsSalesman())
                return Content("<h1>Unautorized<h1>");

            Salesman salesman = (Salesman)Session["salesman"];

            var allEvents = (List<Event>)_context.Event.GetAll();
            var allCustomers = (List<Customer>)_context.Customer.GetAll();
            var allTickets = (List<Ticket>)_context.Ticket.GetAll();

            var model = new List<TicketViewModel>();

            foreach (var ticket in allTickets.Where(x => x.SalesmanID == salesman.ID && !x.IsDeleted && x.Status).ToList())
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

        [HttpGet]
        public ActionResult CreateEvent()
        {
            if (!IsSalesman())
                return Content("<h1>Unauthorized<h1>");

            return View();
        }

        [HttpPost]
        public ActionResult CreateEvent(CreateEventViewModel model)
        {
            if (!IsSalesman())
                return Content("<h1>Unauthorized<h1>");

            Salesman salesman = (Salesman)Session["salesman"];

            int venueID = 0;
            try
            {
                venueID = _context.Venue.GetAll().SingleOrDefault(x => x.Name == model.VenueName).ID;
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Given venue does not exist";
                return View();
            }

            var allEvents = _context.Event.GetAll();

            if (allEvents.Where(x => x.VenueID == venueID && DateTime.Compare(x.EventDate, model.EventDate) == 0).ToList().Count > 0)
            {
                ViewBag.ErrorMsg = $"The venue is already occupied on {model.EventDate.ToString("dd.MM.yyyy, HH:mm")}";
                return View();
            }

            if (DateTime.Compare(model.EventDate, DateTime.Now) <= 0)
            {
                ViewBag.ErrorMsg = $"The given date was not valid.";
                return View();
            }

            try
            {
                HttpPostedFileBase file = Request.Files["ImageData"];
                //if (file.ContentLength == 0)
                //    //something
                
                Event newEvent = new Event()
                {
                    ID = ++HomeController.eventID,
                    SalesmanID = salesman.ID,
                    Name = model.Name,
                    EventType = model.EventType,
                    Capacity = model.Capacity,
                    EventDate = model.EventDate,
                    TicketPrice = model.TicketPrice,
                    Status = false,
                    VenueID = venueID,
                    Poster = file.FileName
                };

                // "C:\\Users\\Bogdan\\source\\repos\\WEB projekat\\pw58-2017-web-projekat\\TicketService\\TicketService.Web\\App_Data\\Images\\thumbnail1.png"
                string saveFileName = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));
                file.SaveAs(saveFileName);

                EventData.SaveEvent(newEvent);
            }
            catch (Exception e)
            {
                throw e;
            }


            return RedirectToAction("Index", "Salesman");
        }

        [HttpGet]
        public ActionResult EditEvent(int id)
        {
            if (!IsSalesman())
                return Content("<h1>Unauthorized<h1>");

            var e = _context.Event.GetAll().SingleOrDefault(x => x.ID == id);
            var venue = _context.Venue.GetAll().SingleOrDefault(x => x.ID == e.VenueID);

            var model = new CreateEventViewModel()
            {
                ID = e.ID,
                Name = e.Name,
                EventType = e.EventType,
                EventDate = e.EventDate,
                Capacity = e.Capacity,
                TicketPrice = e.TicketPrice,
                VenueName = venue.Name
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditEvent(CreateEventViewModel model)
        {
            if (!IsSalesman())
                return Content("<h1>Unauthorized<h1>");

            Salesman salesman = (Salesman)Session["salesman"];

            int venueID = 0;
            try
            {
                venueID = _context.Venue.GetAll().SingleOrDefault(x => x.Name == model.VenueName).ID;
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Given venue does not exist";
                return View();
            }

            var allEvents = _context.Event.GetAll();

            if (allEvents.Where(x => x.VenueID == venueID && DateTime.Compare(x.EventDate, model.EventDate) == 0 && x.ID != model.ID).ToList().Count > 0)
            {
                ViewBag.ErrorMsg = $"The venue is already occupied on {model.EventDate.ToString("dd.MM.yyyy, HH:mm")}";
                return View();
            }
            
            if (DateTime.Compare(model.EventDate, DateTime.Now) <= 0)
            {
                ViewBag.ErrorMsg = $"The given date was not valid.";
                return View();
            }

            try
            {
                HttpPostedFileBase file = Request.Files["ImageData"];
                //if (file.ContentLength == 0)
                //    //something

                List<Event> events = (List<Event>)_context.Event.GetAll();

                int editID = model.ID - 1;

                events[editID].ID = model.ID;
                events[editID].Name = model.Name;
                events[editID].EventType = model.EventType;
                events[editID].Capacity = model.Capacity;
                events[editID].EventDate = model.EventDate;
                events[editID].TicketPrice = model.TicketPrice;
                events[editID].Status = false;
                events[editID].VenueID = venueID;
                events[editID].Poster = file.FileName;

                // "C:\\Users\\Bogdan\\source\\repos\\WEB projekat\\pw58-2017-web-projekat\\TicketService\\TicketService.Web\\App_Data\\Images\\thumbnail1.png"
                string saveFileName = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));
                file.SaveAs(saveFileName);

                EventData.SaveEvents(events);
            }
            catch (Exception e)
            {
                throw e;
            }


            return RedirectToAction("Index", "Salesman");
        }

        public ActionResult DeleteEvent(int id)
        {
            List<Event> events = (List<Event>)_context.Event.GetAll();

            int deleteID = id - 1;

            events[deleteID].IsDeleted = true;

            EventData.SaveEvents(events);
            return RedirectToAction("Index", "Salesman");
        }

        private bool IsSalesman()
        {
            Salesman salesman = (Salesman)Session["salesman"];
            if (salesman == null || salesman.Role != EUserRole.Salesman || salesman.UserName == string.Empty)
                return false;

            return true;
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