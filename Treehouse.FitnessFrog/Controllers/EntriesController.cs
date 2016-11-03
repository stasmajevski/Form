using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
              
            };
            SetUpActivitiesSelectListItems();
            //   ViewBag.IntensitySelectListItems = new SelectList(Entry.Intensity)
            return View(entry);
        }
        [HttpPost]
        
        public ActionResult Add(Entry entry)
        {
            //ModelState.AddModelError("", "This is a global message");

            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }
            SetUpActivitiesSelectListItems();
            return View(entry);
        }

  

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO get the requeted entry from the reposotiry
            Entry entry = _entriesRepository.GetEntry((int)id);
            // TODO return a status of not found if the entry not foulnd
            if(entry == null)
            {
                return HttpNotFound();
            }
            // TODO Pass the entry into the view
            //TODO populate the activities select list items viewbag property
            SetUpActivitiesSelectListItems();

            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            // TODO validate the entry.
            ValidateEntry(entry);
            if(ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");

            }

            // TODO if the entry is valid.
            // 1) use the repository to update the entry
            // 2) redirect the user to the Entries list page
            // TODO populate the activities select list items property
            SetUpActivitiesSelectListItems();

            return View(entry);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }
        private void ValidateEntry(Entry entry)
        {
            // if there aren't any Duration field validation errors
            // then make sure that the duration is greather than 0
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greather than 0");
            }
        }
        private void SetUpActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }

    }

}