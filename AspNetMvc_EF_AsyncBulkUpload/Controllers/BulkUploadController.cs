using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModel;

namespace AspNetMvc_EF_AsyncBulkUpload.Controllers
{
    public class BulkUploadController : AsyncController
    {
        private readonly IUploadRepository<person> uploader = null;

        public BulkUploadController(IUploadRepository<person> uploader)
        {
            this.uploader = uploader;
        }

        public ActionResult Index()
        {
            return View(new FileUploadViewModel());
        }

        public async Task<ActionResult> Upload(FileUploadViewModel model)
        {
            int t1 = Thread.CurrentThread.ManagedThreadId;
            List<person> people = await Task.Factory.StartNew<List<person>>
                (() => GetPeople(model));
            int t2 = Thread.CurrentThread.ManagedThreadId;
            uploader.Upload(people);
            return RedirectToAction("Index", "People");
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