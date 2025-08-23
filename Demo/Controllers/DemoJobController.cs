using Demo;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class DemoJobController : Controller
{
    private readonly DB db;
    private readonly IWebHostEnvironment en; 
    private readonly Helper hp;

    public DemoJobController(DB db, IWebHostEnvironment en, Helper hp)
    {
        this.db = db;
        this.en = en;
        this.hp = hp;
    }

    // Get: for listing job that u created
    public IActionResult Index()
    {
        var model = db.Jobs;
        return View(model);
    }

    public IActionResult Detail(string ? id)
    {
        var model = db.Jobs.Find(id);

        if(model == null)
        {
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public bool CheckId(string id) // check the data redandency
    {
        // TODO
        return !db.Jobs.Any(j => j.Id == id);
    }

   
    [HttpPost] // Post: for inserting new job
    public IActionResult Insert(JobVM vm, IFormFile photo)
    {
        string newid = null;
        Job job = null; // declare job object

        if (ModelState.IsValid("LogoFile"))
        {
            // TODO
            var e = hp.ValidatePhoto(vm.LogoFile);
            if (e != "") ModelState.AddModelError("Photo", e);
        }

        if (ModelState.IsValid) { 
           var jobId = db.Jobs
                        .Where(j => j.Id.StartsWith("L")) // filtering
                        .OrderByDescending(j => j.Id) // sorting
                        .FirstOrDefault(); //  to get the largest number then + 1 

            int nextNumber = 1; // default value (if have not any record)
            if(jobId != null)
            {
                nextNumber = int.Parse(jobId.Id.Substring(1)) +1; // remove "L" and parse the number
            }

            string newId = "L" + nextNumber.ToString("D3"); // for generate L001, L002 and so on


            job = new Job // create a object
            {
                Id = newId,
                Title = vm.Title,
                CompanyName = vm.CompanyName,
                Location=vm.Location,
                PayType= vm.PayType,
                WorkType = vm.WorkType,
                SalaryMax= vm.SalaryMax,
                SalaryMin= vm.SalaryMin,
                Description = vm.Description,
                Summary = vm.Summary,

            };

            db.Jobs.Add(job);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        else
        {
            return View(vm);  // return view with error msg
        }
    }
    //=======================================================================
    public IActionResult Subscription()
    {
        var promotions = db.Promotions.ToList();

        if (Request.isAjax())
        {
            return PartialView("_PromotionCard", promotions);
        }

        return View(promotions);
    }

    public IActionResult Details(string id)
    {
        var job = db.Jobs
            .Include(j => j.Category)
            .Include(j => j.User)
            .FirstOrDefault(j => j.Id == id);

        if (job == null) return NotFound();

        return PartialView("_JobDetails", job);
    }
    //===========================================================================
}

