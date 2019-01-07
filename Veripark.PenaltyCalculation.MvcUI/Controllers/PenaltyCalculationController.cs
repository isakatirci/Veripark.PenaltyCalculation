using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veripark.PenaltyCalculation.MvcUI.Models;
using Veripark.PenaltyCalculation.MvcUI.Models.ViewModels;

namespace Veripark.PenaltyCalculation.MvcUI.Controllers
{
    public class PenaltyCalculationController : Controller
    {
        IPenaltyCalculationDbContext dbContext = new PenaltyCalculationDbContext();

        public string Success { set { TempData["Success"] = ViewData["Success"] = value; } }
        public string Failure { set { TempData["Failure"] = ViewData["Failure"] = value; } }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (TempData["Success"] != null) ViewData["Success"] = TempData["Success"];
            if (TempData["Failure"] != null) ViewData["Failure"] = TempData["Failure"];

            base.OnActionExecuting(filterContext);
        }

        // GET: PenaltyCalculation
        public ActionResult Index()
        {
            var viewModel = new PenaltyCalculationVM();
            try
            {
                ViewBag.CountryId = new SelectList(dbContext.Countries.ToList(), "Id", "Name");
            }
            catch (Exception ex)
            {
                ViewBag.CountryId = new SelectList(Enumerable.Empty<Country>().ToList(), "Id", "Name");
                Failure = ex.Message;
            }
            return View(viewModel);
        }

        public void ValidateModel(MyJsonResult jsonResult, PenaltyCalculationVM model)
        {
            var countryError = "Country Missing";
            var checkedOutDateError = "Checked Out Date Missing";
            var returnedDateError = "Returned Date Missing";

            if (string.IsNullOrWhiteSpace(model.CountryId))
            {
                jsonResult.ErrorMessage = countryError;
            }

            var countryId = 0;
            if (!int.TryParse(model.CountryId, out countryId))
            {
                jsonResult.ErrorMessage = countryError;
            }

            var country = dbContext.Countries.FirstOrDefault(x => x.Id == countryId);
            if (country == null)
            {
                jsonResult.ErrorMessage = countryError;
            }

            if (string.IsNullOrWhiteSpace(model.CheckedOutDate))
            {
                jsonResult.ErrorMessage = checkedOutDateError;
            }

            if (string.IsNullOrWhiteSpace(model.ReturnedDate))
            {
                jsonResult.ErrorMessage = returnedDateError;
            }

            string format = "dd/mm/yyyy";
            DateTime dateTime;
            if (!DateTime.TryParseExact(model.CheckedOutDate, format, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces, out dateTime))
            {
                jsonResult.ErrorMessage = checkedOutDateError;
            }

            if (!DateTime.TryParseExact(model.ReturnedDate, format, CultureInfo.InvariantCulture,
              DateTimeStyles.AllowWhiteSpaces, out dateTime))
            {
                jsonResult.ErrorMessage = returnedDateError;
            }

        }

        [NonAction]
        private void CalculatePenalty(PenaltyCalculationVM model, MyJsonResult jsonResult)
        {
            this.ValidateModel(jsonResult, model);
            if (!string.IsNullOrWhiteSpace(jsonResult.ErrorMessage))
                return;

            var countryId = int.Parse(model.CountryId);
            var startDate = DateTime.ParseExact(model.CheckedOutDate, "dd/mm/yyyy", null);
            var endDate = DateTime.ParseExact(model.ReturnedDate, "dd/mm/yyyy", null);

            var diff = endDate - startDate;
            var totalDays = diff.Days;

            var holidays = dbContext.Holidays.Where(x => x.CountryId == countryId).ToList();
            var weekends = dbContext.Weekends.Where(x => x.CountryId == countryId).ToList();

            for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
            {
                if (holidays.Any() && holidays.Any(x => x.Date.Date == i.Date))
                {
                    totalDays--;
                }
                else if (weekends.Any() && weekends.Any(x => String.Equals(x.DayOfWeek, i.DayOfWeek.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    totalDays--;
                }
            }

            if (totalDays <= 10)
            {
                jsonResult.SuccessMessage = "You have the book delivered on time";
                return;
            }


            var penalizedAmount = (dbContext.Penalizes.FirstOrDefault(x => x.CountryId == countryId) ?? new Penalize()).PenalizedAmount;
            var penalizedDays = totalDays - 10;
            var total = penalizedDays * penalizedAmount;
            var currency = (dbContext.Currencies.FirstOrDefault(x => x.CountryId == countryId) ?? new Currency()).Name;

            jsonResult.ErrorMessage = string.Format("{0} Days Lated. Total Penalized Price: {1} {2}", penalizedDays, total, currency);
        }

        [HttpPost]
        public ActionResult Calculate(PenaltyCalculationVM model)
        {
            var jsonResult = new MyJsonResult();
            try
            {
                CalculatePenalty(model, jsonResult);
            }
            catch (Exception ex)
            {
                jsonResult.ErrorMessage = ex.Message;
            }
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Test()
        {
            return View(new MyClass());
        }



        [HttpPost]
        public ActionResult Test([ModelBinder(typeof(MyBinder))] MyClass model)
        {

            return View(new MyClass());
        }


    }
    public class MyBinder : IModelBinder
    {
        private int @int(string a)
        {
            try
            {
                return int.Parse(a);
            }
            catch (Exception)
            {
                return 0;
            }         
        }
        private DateTime @datetime(string a)
        {
            try
            {
                return DateTime.ParseExact(a, "dd/MM/yyyy", null);
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new MyClass
            {
                MyProperty1 = @int(controllerContext.HttpContext.Request.Form["MyProperty1"]),
                MyProperty2 = @datetime(controllerContext.HttpContext.Request.Form["MyProperty2"])

            };
        }
    }


    public class MyClass
    {
        public int MyProperty1 { get; set; }
        public DateTime MyProperty2 { get; set; }
    }
}

