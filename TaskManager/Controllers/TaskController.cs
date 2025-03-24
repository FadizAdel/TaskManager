using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TaskManager.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class TaskController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Get the current user ID from session
        int? userId = HttpContext.Session.GetInt32("UserId");
        
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get tasks for the current user
        var tasks = _context.Tasks.Where(t => t.UserId == userId.Value).ToList();
        return View(tasks);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(TaskItem task)
    {
        try
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Set the required properties
            task.UserId = userId.Value;
            task.CreatedDate = DateTime.Now;
            task.IsCompleted = false;
            
            // Add and save
            _context.Tasks.Add(task);
            var result = _context.SaveChanges();
            
            // Check if changes were saved
            if (result > 0)
            {
                TempData["SuccessMessage"] = "Task created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to save task.";
            }
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Log the exception details
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return View(task);
        }
    }

    public IActionResult Edit(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
        
        if (task == null)
        {
            return NotFound();
        }
        
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TaskItem taskModel)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }
    
        // Ensure we're editing the correct task
        if (id != taskModel.Id)
        {
            return NotFound();
        }
    
        // Make sure the task belongs to the current user
        var existingTask = _context.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
        
        if (existingTask == null)
        {
            return NotFound();
        }
    
        if (ModelState.IsValid)
        {
            try
            {
                // Update only the properties that should be editable
                existingTask.Title = taskModel.Title;
                existingTask.Description = taskModel.Description;
                existingTask.DueDate = taskModel.DueDate;
                existingTask.IsCompleted = taskModel.IsCompleted;
                
                // Save changes
                _context.SaveChanges();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Add error to model state
                ModelState.AddModelError("", $"Error updating task: {ex.Message}");
            }
        }
        
        return View(taskModel);
    }

    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
        
        if (task != null)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ToggleComplete(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);
        
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            _context.SaveChanges();
        }
        
        return RedirectToAction("Index");
    }
}
