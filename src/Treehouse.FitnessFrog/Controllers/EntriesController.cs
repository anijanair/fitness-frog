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

            //Extracted the validation code into its own method
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)

        {
            //Extracted the validation code into its own method
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the requested entry from the repository
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TODO Return a status of "Not Found" if the entry wasn't found
            if (entry == null)
            {
                return HttpNotFound();
            }

            SetupActivitiesSelectListItems();

            // TODO Pass the entry into View
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            // validate the entry
            ValidateEntry(entry);

            //  if the entry is valid
            
            if (ModelState.IsValid)
            {
                // 1. Use the repository to update the entry
                _entriesRepository.UpdateEntry(entry);

                // 2. Redirect the user to the 'Entries' list page
                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Retrieve entry for the provided id parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);
             // Return "Not Found" if an entry was not found
            if (entry == null)
            {
                return HttpNotFound();
            }
             // Pass the entry into view

            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Delete the entry
            _entriesRepository.DeleteEntry(id);

            // Redirect to "Entries" list page
            return RedirectToAction("Index");

        }

        private void ValidateEntry(Entry entry)
        {
            // validate to check if 'Duration' field is null or less than 0

            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

            //TODO populate the activities select list items ViewBag property
        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                Data.Data.Activities, "Id", "Name");
        }

    }
}