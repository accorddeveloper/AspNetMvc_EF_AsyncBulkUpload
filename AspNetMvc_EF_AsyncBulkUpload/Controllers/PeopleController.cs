using DAL;
using DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModel;

namespace MVC.Controllers
{
    /// <summary>
    /// People Controller
    /// </summary>
    [RoutePrefix("People")]
    public class PeopleController : Controller
    {
        /// <summary>
        /// Generic Unit Of Work class that will be injected
        /// </summary>
        private readonly IPersonService personService = null;
        private readonly IJobService jobService = null;
        private readonly IUploadRepository<person> uploader = null;

        public PeopleController(IPersonService personService, IJobService jobService, IUploadRepository<person> uploader)
        {
            this.personService = personService;
            this.jobService = jobService;
            this.uploader = uploader;
        }

        // GET: People
        public ActionResult Index()
        {
            var people = personService.GetPeople().ToList();
            return View(people);
        }

        // GET: People/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            person person = personService.GetPerson(id ?? 0);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            ViewBag.id_job = new SelectList(jobService.GetJobs().ToList(), "id", "name");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,sex,developer,description,id_job")] person person)
        {
            if (ModelState.IsValid)
            {
                personService.InsertPerson(person);

                return RedirectToAction("Index");
            }

            ViewBag.id_job = new SelectList(jobService.GetJobs().ToList(), "id", "name", person.id_job);
            return View(person);
        }

        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            person person = personService.GetPerson(id ?? 0);
            if (person == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_job = new SelectList(jobService.GetJobs().ToList(), "id", "name", person.id_job);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,sex,developer,description,id_job")] person person)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    personService.UpdatePerson(person);

                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            ViewBag.id_job = new SelectList(jobService.GetJobs().ToList(), "id", "name", person.id_job);
            return View(person);
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            person person = personService.GetPerson(id ?? 0);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            person person = personService.GetPerson(id);
            personService.DeletePerson(person);

            return RedirectToAction("Index");
        }
        [Route("Upload/CVS")]
        public ActionResult CVS()
        {
            return View("Upload", new FileUploadViewModel());
        }

        [HttpPost, Route("Upload/CVS")]
        public async Task<ActionResult> CVS(FileUploadViewModel model)
        {
            int t1 = Thread.CurrentThread.ManagedThreadId;
            List<person> people = await Task.Factory.StartNew<List<person>>
                (() => GetPeople(model));
            int t2 = Thread.CurrentThread.ManagedThreadId;
            uploader.Upload(people);
            return RedirectToAction("Index");
        }

        private List<person> GetPeople(FileUploadViewModel model)
        {
            List<person> people = new List<person>();
            StreamReader csvreader = new StreamReader(model.fileUpload.InputStream);
            // Uncomment if first line is header
            // csvreader.ReadLine(); 
            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(','); // Values are comma separated
                person p = new person();
                p.name = values[0];
                p.sex = values[1];
                p.id_job = int.Parse(values[2]);
                people.Add(p);
            }
            return people;
        }
    }
}
