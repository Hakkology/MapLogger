using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MapLogger.Models;

namespace MapLogger.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly RabbitMqService _rabbitMqService;


    public HomeController(IConfiguration configuration, RabbitMqService rabbitMqService)
    {
        _configuration = configuration;
        _rabbitMqService = rabbitMqService;
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
        try
        {
            _rabbitMqService.SendLogToRabbitMQ(type, longitude, latitude);
            return Json(new { Success = true });
        }
        catch (Exception ex)
        {
            return Json(new { Success = false, Message = ex.Message });
        }
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
