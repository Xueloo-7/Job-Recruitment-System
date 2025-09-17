using Demo;
using Demo.Migrations;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Employer")]
public class JobController : BaseController
{
    private readonly DB db;
    private readonly Helper hp;

    public JobController(DB db, Helper hp)
    {
        this.db = db;
        this.hp = hp;
    }

    // Functions
    private JobDraft? GetUserDraft(Guid id)
    {
        var userId = User.GetUserId();
        var draft = db.JobDrafts.Find(id);
        if (draft == null || draft.UserId != userId) return null;
        return draft;
    }

    private void UpdateDraftFromVm(JobDraft draft, JobClassifyVM vm)
    {
        draft.Title = vm.Title;
        draft.Location = vm.Location;
        draft.WorkType = vm.WorkType;
        draft.CategoryId = vm.CategoryId;
        draft.PayType = vm.PayType;
        draft.SalaryMin = vm.SalaryMin;
        draft.SalaryMax = vm.SalaryMax;
        draft.UpdatedAt = DateTime.UtcNow;
    }

    private Job PublishOrUpdateJob(JobDraft draft)
    {
        // 如果 draft.JobId != null → 更新 Job
        if (draft.JobId != null)
        {
            var job = db.Jobs.Find(draft.JobId);
            if (job == null || job.UserId != draft.UserId)
            {
                throw new InvalidOperationException("Original job not found or not authorized.");
            }

            job.Title = draft.Title ?? "";
            job.Location = draft.Location ?? "";
            job.CategoryId = draft.CategoryId ?? "";
            job.PromotionId = draft.PromotionId ?? "";
            job.PayType = draft.PayType ?? PayType.Monthly;
            job.WorkType = draft.WorkType ?? WorkType.FullTime;
            job.SalaryMin = draft.SalaryMin ?? 0;
            job.SalaryMax = draft.SalaryMax ?? 0;
            job.Description = draft.Description ?? "";
            job.Summary = draft.Summary ?? "";
            job.UpdatedAt = DateTime.UtcNow;
            job.Status = JobStatus.Pending; // 回到待审核

            db.Jobs.Update(job);
            db.JobDrafts.Remove(draft);
            db.SaveChanges();
            return job;
        }
        else
        {
            // 新发布
            var job = new Job
            {
                Id = Helper.GenerateId(db.Jobs, "J"),
                UserId = draft.UserId,
                Title = draft.Title ?? "",
                Location = draft.Location ?? "",
                CategoryId = draft.CategoryId ?? "",
                PromotionId = draft.PromotionId ?? "",
                PayType = draft.PayType ?? PayType.Monthly,
                WorkType = draft.WorkType ?? WorkType.FullTime,
                SalaryMin = draft.SalaryMin ?? 0,
                SalaryMax = draft.SalaryMax ?? 0,
                Description = draft.Description ?? "",
                Summary = draft.Summary ?? "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = JobStatus.Pending
            };

            db.Jobs.Add(job);
            db.JobDrafts.Remove(draft);
            db.SaveChanges();
            return job;
        }
    }

    public IActionResult Draft(Guid? id)
    {
        var userId = User.GetUserId();
        JobDraft? draft;

        if (id == null)
        {
            // Create new draft
            draft = new JobDraft
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastStep = 1
            };

            db.JobDrafts.Add(draft);
            db.SaveChanges();
        }
        else
        {
            // Get existing draft
            draft = GetUserDraft(id.Value);
            if (draft == null) return NotFound();
        }

        // Go to "LastStep" page of the draft
        var actionName = draft.LastStep switch
        {
            1 => nameof(Classify),
            2 => nameof(Subscription),
            3 => nameof(Write),
            4 => nameof(PayAndPost),
            _ => nameof(Classify)
        };

        return RedirectToAction(actionName, new { id = draft.Id });
    }


    public IActionResult Classify(Guid id)
    {
        // Get draft
        var draft = GetUserDraft(id);
        if (draft == null) return NotFound();

        var vm = new JobClassifyVM
        {
            JobId = draft.JobId, // null for new jobs, set for editing existing jobs
            DraftId = draft.Id,
            Title = draft.Title ?? "",
            Location = draft.Location ?? "",
            WorkType = draft.WorkType,
            CategoryId = draft.CategoryId,
            PayType = draft.PayType,
            SalaryMin = draft.SalaryMin ?? 0,
            SalaryMax = draft.SalaryMax ?? 0,
            WorkTypeOptions = SelectListHelper.ToSelectList<WorkType>(draft.WorkType),
            CategoryOptions = SelectListHelper.GetCategorySelectList(db),
            PayTypeOptions = SelectListHelper.ToSelectList<PayType>(draft.PayType),
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Classify(JobClassifyVM vm, string action)
    {
        // 保存/继续时都需要草稿
        var draft = GetUserDraft(vm.DraftId ?? Guid.Empty);
        if (draft == null) return NotFound();

        if (action == "save")
        {
            // 跳过验证，允许空字段
            ModelState.Clear();
        }
        else if (action == "continue" && !ModelState.IsValid)
        {
            // 校验失败 → 返回表单
            return View(vm);
        }

        // 映射 VM → Draft
        UpdateDraftFromVm(draft, vm);

        // 差异点：LastStep + Redirect
        if (action == "save")
        {
            draft.LastStep = 1;
            db.SaveChanges();
            SetFlashMessage(FlashMessageType.Success, "Draft Saved");
            return RedirectToAction("Classify", new { id = draft.Id });
        }
        else // continue
        {
            draft.LastStep = 2;
            db.SaveChanges();
            return RedirectToAction("Subscription", new { id = draft.Id });
        }
    }

    public IActionResult Subscription(Guid id)
    {
        // Get draft
        var draft = GetUserDraft(id);
        if (draft == null) return NotFound();

        var vm = new JobSubscriptionVM
        {
            JobId = draft.JobId, // null for new jobs, set for editing existing jobs
            DraftId = draft.Id,
            PromotionId = draft.PromotionId ?? "",
            Promotions = db.Promotions.ToList()
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Subscription(JobSubscriptionVM vm, string action)
    {
        var draft = GetUserDraft(vm.DraftId ?? Guid.Empty);
        if (draft == null) return NotFound();

        // Back 按钮：直接回到上一步，不需要验证
        if (action == "back")
        {
            draft.LastStep = 1;
            draft.UpdatedAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Classify", new { id = draft.Id });
        }

        // Save Draft：允许空字段
        if (action == "save")
        {
            ModelState.Clear(); // 跳过验证
        }
        else if (action == "continue" && !ModelState.IsValid)
        {
            // Continue 但验证失败 → 返回表单（保持 promotions 列表）
            vm.Promotions = db.Promotions.ToList();
            return View(vm);
        }

        // 映射 VM → Draft（无论 save/continue 都要保存 PromotionId）
        draft.PromotionId = vm.PromotionId;
        draft.UpdatedAt = DateTime.UtcNow;

        if (action == "save")
        {
            draft.LastStep = 2;
            db.SaveChanges();
            SetFlashMessage(FlashMessageType.Success, "Draft Saved");
            return RedirectToAction("Subscription", new { id = draft.Id });
        }
        else // continue
        {
            draft.LastStep = 3;
            db.SaveChanges();
            return RedirectToAction("Write", new { id = draft.Id });
        }
    }

    public IActionResult Write(Guid id)
    {
        // 读取草稿
        var draft = GetUserDraft(id);
        if (draft == null) return NotFound();

        var vm = new JobWriteVM
        {
            JobId = draft.JobId, // null for new jobs, set for editing existing jobs
            DraftId = draft.Id,
            Description = draft.Description ?? "",
            Summary = draft.Summary ?? ""
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Write(JobWriteVM vm, string action)
    {
        var draft = GetUserDraft(vm.DraftId ?? Guid.Empty);
        if (draft == null) return NotFound();

        // ===== 处理 Logo 上传 =====
        if (vm.Logo != null && vm.Logo.Length > 0)
        {
            var err = hp.ValidatePhoto(vm.Logo);
            if (!string.IsNullOrEmpty(err))
            {
                ModelState.AddModelError("Logo", err);
                return View(vm); // 验证失败，返回表单并显示错误
            }

            var fileName = hp.SavePhoto(vm.Logo, "uploads/logos");

            // 保存相对路径到 Draft
            draft.LogoImageUrl = $"/uploads/logos/{fileName}";
            vm.LogoImageUrl = draft.LogoImageUrl;
        }

        if (action == "back")
        {
            draft.LastStep = 2;
            draft.UpdatedAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Subscription", new { id = draft.Id });
        }

        if (action == "save")
        {
            ModelState.Clear();
        }
        else if ((action == "continue" || action == "done") && !ModelState.IsValid)
        {
            return View(vm);
        }

        draft.Description = vm.Description;
        draft.Summary = vm.Summary;
        draft.UpdatedAt = DateTime.UtcNow;

        if (action == "save")
        {
            draft.LastStep = 3;
            db.SaveChanges();
            SetFlashMessage(FlashMessageType.Success, "Draft Saved");
            return RedirectToAction("Write", new { id = draft.Id });
        }
        else if (action == "continue")
        {
            draft.LastStep = 4;
            db.SaveChanges();
            return RedirectToAction("PayAndPost", new { id = draft.Id });
        }
        else if (action == "done")
        {
            try
            {
                var job = PublishOrUpdateJob(draft);
                SetFlashMessage(FlashMessageType.Success, draft.JobId != null ? "Job Updated Successfully!" : "Job Posted Successfully!");
            }
            catch (InvalidOperationException ex)
            {
                SetFlashMessage(FlashMessageType.Danger, ex.Message);
            }

            return RedirectToAction("Index", "Employer");
        }

        return View(vm);
    }

    public IActionResult PayAndPost(Guid id)
    {
        var draft = GetUserDraft(id);
        if (draft == null) return NotFound();

        var promotion = db.Promotions.FirstOrDefault(p => p.Id == draft.PromotionId);
        if (promotion == null)
        {
            SetFlashMessage(FlashMessageType.Warning, "Please select a promotion plan.");
            return RedirectToAction("Subscription", new { id = draft.Id });
        }

        var vm = new JobPaymentVM
        {
            DraftId = draft.Id,
            Promotion = promotion,
            Subtotal = promotion.Price ?? 0m,
            TaxRate = 0.08m
        };

        vm.Tax = vm.Subtotal * vm.TaxRate;
        vm.Total = vm.Subtotal + vm.Tax;

        return View(vm);
    }

    [HttpPost]
    public IActionResult PayAndPost(JobPaymentVM vm, string action)
    {
        var draft = GetUserDraft(vm.DraftId ?? Guid.Empty);
        if (draft == null) return NotFound();

        if (action == "back")
        {
            draft.LastStep = 3;
            draft.UpdatedAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Write", new { id = draft.Id });
        }

        if (action == "continue")
        {
            try
            {
                var job = PublishOrUpdateJob(draft);
                SetFlashMessage(FlashMessageType.Success, draft.JobId != null ? "Job Updated Successfully!" : "Job Posted Successfully!");
            }
            catch (InvalidOperationException ex)
            {
                SetFlashMessage(FlashMessageType.Danger, ex.Message);
            }

            return RedirectToAction("Index", "Employer");
        }

        return View(vm);
    }


    public IActionResult DeleteDraft(Guid id)
    {
        var draft = GetUserDraft(id);
        if (draft == null) return NotFound();
        db.JobDrafts.Remove(draft);
        db.SaveChanges();
        SetFlashMessage(FlashMessageType.Success, "Draft Deleted");
        return RedirectToAction("Index", "Employer");
    }

    public IActionResult WithDraw(string id)
    {
        var job = db.Jobs.Find(id);
        if (job == null || job.UserId != User.GetUserId()) return NotFound();
        if (job.Status != JobStatus.Approved)
        {
            SetFlashMessage(FlashMessageType.Warning, "Only active jobs can be withdrawn.");
            return RedirectToAction("Index", "Employer");
        }

        // Withdraw the job
        job.Status = JobStatus.Withdrawn;
        job.UpdatedAt = DateTime.UtcNow;
        db.SaveChanges();

        // Copy a draft for future editing
        var draft = new JobDraft
        {
            JobId = job.Id, // Link to original job, if this draft is done, it can change the original job to pending again
            UserId = job.UserId,
            Title = job.Title,
            Location = job.Location,
            CategoryId = job.CategoryId,
            PromotionId = job.PromotionId,
            PayType = job.PayType,
            WorkType = job.WorkType,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            Description = job.Description,
            Summary = job.Summary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastStep = 1
        };
        db.JobDrafts.Add(draft);
        db.SaveChanges();

        SetFlashMessage(FlashMessageType.Success, "Job Withdrawn Successfully");
        return RedirectToAction("Index", "Employer", new { id = job.Id });
    }
}
