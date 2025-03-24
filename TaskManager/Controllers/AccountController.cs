using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using TaskManager.Models;
using System;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string userName, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == userName && u.PasswordHash == password);

        if (user == null)
        {
            ViewBag.Error = "Invalid user or password";
            return View();
        }

        // Store user ID in session
        HttpContext.Session.SetInt32("UserId", user.Id);

        return RedirectToAction("Index", "Task");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string userName, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Username and password are required";
            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match";
            return View();
        }

        if (_context.Users.Any(u => u.Username == userName))
        {
            ViewBag.Error = "Username already exists";
            return View();
        }

        var user = new User
        {
            Username = userName,
            PasswordHash = password // Note: In production, this should be hashed
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login");
    }
}
