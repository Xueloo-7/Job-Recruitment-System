namespace Demo.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable warnings

// Show Job List
public class EmployerJobVM
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public JobStatus Status { get; set; }
    public int CandidatesCount { get; set; }   
    public int HiredCount { get; set; }      
}

// Show Draft List
public class EmployerDraftVM
{
    public string Id { get; set; }
    public string? JobId { get; set; } // Sometime a draft is already published
    public string Title { get; set; }
    public string Location { get; set; }
    //public int LastStep { get; set; }
}

// Show Job & Draft List and Summary
public class EmployerDashboardVM
{
    public string EmployerName { get; set; }

    // Summary
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public int TotalHires { get; set; }

    // Lists
    public List<EmployerJobVM> Jobs { get; set; }
    public List<EmployerDraftVM> Drafts { get; set; }

}

public class EmployerAccountVM
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }    
    public string CompanyName { get; set; } 
}