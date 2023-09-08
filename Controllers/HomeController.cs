using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MapLogger.Models;

namespace MapLogger.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public HomeController(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var cesiumToken = _configuration["Cesium:AccessToken"];
        ViewData["CesiumToken"] = cesiumToken;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public ActionResult CoordinateLogger(string type, double longitude, double latitude)
    {
        var timestamp = DateTime.UtcNow;
        var logString = $"{timestamp},{longitude},{latitude}\n";
        System.IO.File.AppendAllText(_env.ContentRootPath + "/Loggers" + $"/{type}.txt", logString);
        return Json(new { Success = true });
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
