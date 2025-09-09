using Demo;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
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
        //ViewBag.Joblist = new SelectList(db.Jobs, "PayType", "WorkType", "Promotion"); // TODO

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

        var insert1 = TempData.Get<JobVM1>("Insert1") ?? new JobVM1();
        return View(insert1);

    }

    [HttpPost] // Post: for inserting new job
    public IActionResult Insert1(JobVM1 vm) //IFormFile? LogoFile)
    {
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

        if (ModelState.IsValid)
        {
            var jobId = db.Jobs
            .Where(j => j.Id.StartsWith("L")) // filtering
            .OrderByDescending(j => j.Id) // sorting
            .FirstOrDefault(); //  to get the largest number then + 1 

            int nextNumber = (jobId != null) ? int.Parse(jobId.Id.Substring(1)) + 1 : 1;
            vm.Id = "L" + nextNumber.ToString("D3");

            //job = new Job // create a object
            //{
            //    Id = newId,
            //    Title = vm.Title,
            //    CompanyName = vm.CompanyName,
            //    Location = vm.Location,
            //    PayType = vm.PayType,
            //    WorkType = vm.WorkType,
            //    SalaryMax = vm.SalaryMax,
            //    SalaryMin = vm.SalaryMin,
            //    Description = vm.Description,
            //    Summary = vm.Summary,
            //    LogoImageUrl = hp.SavePhoto(vm.LogoFile, "~/images/uploads/logo"),
            //    CreatedAt = DateTime.Now,
            //    UpdatedAt = DateTime.Now,

            //};

            //db.Jobs.Add(job);
            //db.SaveChanges();
            TempData.Put("Insert1", vm);
            return RedirectToAction("Insert2"); // jumping 
        }

        DebugModelStateErrors();
        
       return View(vm);  // return view with error msg
       
    }

    public IActionResult Insert2()
    { 
        // Get method for second step
        var insert1 = TempData.Get<JobVM1>("Insert1"); // keep Insert1 data
        TempData.Keep("Insert1");

        var insert2 = TempData.Get<JobVM2>("Insert2") ?? new JobVM2();
       

        // Promotion Options
        var promotions = db.Promotions.ToList();
        insert2.PromotionOptions = promotions.Select(p => new SelectListItem
        {
            Value = p.Id,
            Text = p.Name
        }).ToList();


        return View(insert2);
    }


    [HttpPost]
    public IActionResult Insert2(JobVM2 vm)
    {
        if (ModelState.IsValid)
        {
            TempData.Put("Insert2", vm);
            TempData.Keep("Insert1");
            return RedirectToAction("Insert3");
        }

        TempData.Keep("Insert1");
        return View(vm);
    }

    public IActionResult Insert3()
    {
        var insert3 = TempData.Get<JobVM3>("Insert3") ?? new JobVM3();
        TempData.Keep("Insert1");   // keep the data in TempData
        TempData.Keep("Insert2"); // keep the data in TempData
        return View(insert3);
    }

    [HttpPost]
    public IActionResult Insert3(JobVM3 vm)
    {
        if (!ModelState.IsValid)
        {
            TempData.Keep("Insert1"); 
            TempData.Keep("Insert2"); 
            return View(vm);
        }

        var Job = new CombineJobVM // combine data as one object
        {
            Insert1 = TempData.Get<JobVM1>("Insert1"),
            Insert2 = TempData.Get<JobVM2>("Insert2"),
            Insert3 = vm
        };

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        db.Jobs.Add(new Job
        {
            Id = "L" + (db.Jobs.Count() + 1).ToString("D3"),
            UserId = userId!,
            Title = Job.Insert1.Title,
            CompanyName = Job.Insert1.CompanyName,
            Location = Job.Insert1.Location,
            PayType = Job.Insert1.PayType.Value,
            WorkType = Job.Insert1.WorkType.Value,
            SalaryMax = Job.Insert1.SalaryMax,
            SalaryMin = Job.Insert1.SalaryMin,
            Description = Job.Insert2.Description,
            Summary = Job.Insert2.Summary,
            PromotionId = Job.Insert2.PromotionId, // TODO: set PromotionId
            LogoImageUrl = (vm.LogoFile != null) ? hp.SavePhoto(vm.LogoFile, "images/uploads/logo") : null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        });

        db.SaveChanges();


        TempData.Remove("Insert1");
        TempData.Remove("Insert2");

        TempData["Info"] = "Job Inserted successfully.";
        return RedirectToAction ("Index");
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

    private void DebugModelStateErrors()
    {
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
            }
        }
    }
}

