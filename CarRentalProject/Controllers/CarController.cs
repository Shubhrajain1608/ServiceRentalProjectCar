using CarRentalProject.Models;
using CarRentalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarRentalProject.Controllers
{
    public class CarController : Controller
    {
        ApplicationDbContext _context;

        public CarController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Car
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CarForm(ApplicationUser applicationUser)
        {
            var viewModel = new SingleCarViewModel
            {
                ApplicationUser = applicationUser
            };

            return View(viewModel);
        }

        [HttpPost]

        public ActionResult Save(SingleCarViewModel viewModel)
        {
            if (!ModelState.IsValid)

            {
                return View("CarForm", viewModel);
            }

            viewModel.Car.UserId = viewModel.ApplicationUser.Id;
            var applicationUser = _context.Users.Find(viewModel.ApplicationUser.Id);
            var car = viewModel.Car;
            _context.Cars.Add(car);
            _context.SaveChanges();

            return RedirectToAction("CustAndCarForm", "Customer", applicationUser);

        }

        public ActionResult EditForm(Car car)
        {
            var applicationUser = _context.Users.Find(car.UserId);
            var viewModel = new SingleCarViewModel
            {
                Car = car,
                ApplicationUser = applicationUser
            };

            return View(viewModel);
        }

        [HttpPost]

        public ActionResult Edit(Car car)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new SingleCarViewModel
                {
                    Car = car
                };
                return View("EditForm", viewModel);
            }
            else
            {
                var carInDb = _context.Cars.Find(car.Id);
                var applicationUser = _context.Users.Find(carInDb.UserId);
                carInDb.VIN = car.VIN;
                carInDb.Make = car.Make;
                carInDb.Model = car.Model;
                carInDb.Color = car.Color;
                carInDb.Year = car.Year;
                carInDb.Miles = car.Miles;

                _context.SaveChanges();

                return RedirectToAction("CustAndCarForm", "Customer", applicationUser);
            }
        }

        public ActionResult Delete(int id)
        {
            Car car = _context.Cars.Find(id);
            var applicationUser = _context.Users.Find(car.UserId);
            _context.Cars.Remove(car);
            _context.SaveChanges();
            return RedirectToAction("CustAndCarForm", "Customer", applicationUser);
        }

        public ActionResult Delete1(int id)
        {
            Service service = _context.Services.Find(id);
            var car = _context.Cars.Find(service.CarId);
            _context.Services.Remove(service);
            _context.SaveChanges();
            return RedirectToAction("AddNewServices", "Car", car);
        }

        public ActionResult ShowLessServiceForm(Car car)
        {
            var service = _context.Services.Where(c => c.CarId == car.Id).OrderByDescending(c => c.DateAdded).ToList();
            var serviceList = new List<Service>();
            int i = 0;

            foreach (var item in service)
            {
                i++;
                if (i <= 5)
                {
                    serviceList.Add(item);
                }
            }

            var viewModel = new CarAndServiceViewModel
            {
                Car = car,
                Services = serviceList,
                CheckInteger = i,
                ServiceType = _context.ServiceTypes.ToList()
            };

            return View(viewModel);
        }

        public ActionResult AddNewServices(Car car)
        {
            var service = _context.Services.Where(c => c.CarId == car.Id).OrderByDescending(c => c.DateAdded).ToList();

            List<ServiceType> serviceType = _context.ServiceTypes.ToList();

            var viewModel = new CarAndServiceViewModel
            {
                Car = car,
                Services = service,
                ServiceType = serviceType
            };

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AddServices(CarAndServiceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Service.DateAdded = DateTime.Today;
                viewModel.Service.CarId = viewModel.Car.Id;

                var car = _context.Cars.Find(viewModel.Car.Id);

                _context.Services.Add(viewModel.Service);
                _context.SaveChanges();

                return RedirectToAction("AddNewServices", "Car", car);
            }

            viewModel.Car = _context.Cars.Find(viewModel.Car.Id);
            viewModel.Services = _context.Services.Where(c => c.CarId == viewModel.Car.Id).OrderByDescending(c => c.DateAdded).ToList();
            viewModel.ServiceType = _context.ServiceTypes.ToList();

            return View("AddNewServices", viewModel);

        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}