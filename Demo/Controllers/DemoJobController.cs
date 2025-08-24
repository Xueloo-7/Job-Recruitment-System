using Demo;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
      
        return !db.Jobs.Any(j => j.Id == id);
    }

    // Get method used for jump to insert page 
    public IActionResult Insert1()
    {
        ViewBag.Joblist = new SelectList(db.Jobs, "PayType", "WorkType", "Promotion"); // TODO

        ViewBag.PayTypes = Enum.GetValues(typeof(PayType))// for listing payment method 
       .Cast<PayType>()
       .Select(e => new SelectListItem
       {
           Value = e.ToString(),
           Text = e.ToString()
       });

        ViewBag.WorkTypes = Enum.GetValues(typeof(WorkType))// for listing payment method 
      .Cast<WorkType>()
      .Select(e => new SelectListItem
      {
          Value = e.ToString(),
          Text = e.ToString()
      });

        var insert1 = TempData.Get<JobVM>("Step1") ?? new JobVM();
        return View(insert1);

    }

    [HttpPost] // Post: for inserting new job
    public IActionResult Insert1(JobVM vm, IFormFile? LogoFile)
    {
        string newid = null;
        Job job = null; // declare job object

        bool TitleDuplicate = db.Jobs.Any(j => j.Title == vm.Title && j.CompanyName == vm.CompanyName);

        if (TitleDuplicate)
        {
            ModelState.AddModelError("Title", "A job with this title already exists in the same company.");
            return View(vm); 
        }

        //if (ModelState.IsValid("LogoFile"))
        //{
        //    // TODO
        //    var e = hp.ValidatePhoto(vm.LogoFile);
        //    if (e != "") ModelState.AddModelError("LogoFile", e);
        //}

        //if (ModelState.IsValid) {
        //    var jobId = db.Jobs
        //                 .Where(j => j.Id.StartsWith("L")) // filtering
        //                 .OrderByDescending(j => j.Id) // sorting
        //                 .FirstOrDefault(); //  to get the largest number then + 1 

        //    int nextNumber = 1; // default value (if have not any record)
        //    if (jobId != null)
        //    {
        //        nextNumber = int.Parse(jobId.Id.Substring(1)) + 1; // remove "L" and parse the number
        //    }

        //    string newId = "L" + nextNumber.ToString("D3"); // for generate L001, L002 and so on

        //}
            //job = new Job // create a object
            //{
            //    Id = newId,
            //    Title = vm.Title,
            //    CompanyName = vm.CompanyName,
            //    Location=vm.Location,
            //    PayType= vm.PayType,
            //    WorkType = vm.WorkType,
            //    SalaryMax= vm.SalaryMax,
            //    SalaryMin= vm.SalaryMin,
            //    Description = vm.Description,
            //    Summary = vm.Summary,
            //    LogoImageUrl = hp.SavePhoto(vm.LogoFile, "~/images/uploads/logo"),
            //    CreatedAt = DateTime.Now,
            //    UpdatedAt = DateTime.Now,

            //};

            //db.Jobs.Add(job);
            //db.SaveChanges();
    //        TempData.Put("Insert1", vm);
    //        return RedirectToAction("Insert2"); // jumping 
            
    //    }
    //    else
    //    {
    //        return View(vm);  // return view with error msg
    //    }
    //}

    //public IActionResult Insert2()
    //{
    //    var insert2 = TempData.Get<JobVM2>("Insert2") ?? new JobVM2();
    //    TempData.Keep("Insert1");

    //    // Promotion Options
    //    var promotions = db.Promotions.ToList();
    //    insert2.PromotionOptions = promotions.Select(p => new SelectListItem
    //    {
    //        Value = p.Id,
    //        Text = p.Name
    //    }).ToList();

       return View();
    }

    //[HttpPost]
    //public IActionResult Insert2(JobVM2 vm)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        TempData.Put("Insert2", vm);
    //        return RedirectToAction("Insert3");
    //    }
    //    else
    //    {
    //        TempData.Keep("Insert1");
    //        return View(vm);
    //    }

    //}

    //public IActionResult Insert3()
    //{
    //    var insert3 = TempData.Get<JobVM3>("Insert3") ?? new JobVM3();
    //    TempData.Keep("Insert1");
    //    TempData.Keep("Insert2");
    //    return View(insert3);
    //}

   // [HttpPost]
   // public IActionResult Insert3(JobVM3 vm)
   // {
   //     if (!ModelState.IsValid) {
   //         TempData.Keep("Insert1");
   //         TempData.Keep("Insert2");
   //         return View(vm);
   //     }

   //     //var job = new
   //}
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

