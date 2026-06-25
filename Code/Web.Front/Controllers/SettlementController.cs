using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Web.Front.Controllers;

public class SettlementController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SettlementController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Submit(SettlementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", request);
        }

        var selectedCity = Request.Form["City"];
        string apiUrl;

        switch (selectedCity)
        {
            case "CITYA":
                apiUrl = $"{_configuration["ServiceUrls:ServiceCityA"]}/api/settlement/submit";
                break;
            case "CITYB":
                apiUrl = $"{_configuration["ServiceUrls:ServiceCityB"]}/api/settlement/submit";
                break;
            case "CITYC":
                apiUrl = $"{_configuration["ServiceUrls:ServiceCityC"]}/api/settlement/submit";
                break;
            default:
                ModelState.AddModelError("City", "请选择地市");
                return View("Index", request);
        }

        var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        try
        {
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<SettlementResponse>>(responseContent);

            ViewBag.Result = result;
            ViewBag.City = selectedCity;
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
        }

        return View("Result");
    }

    [HttpGet]
    public async Task<IActionResult> Query(string recordNo)
    {
        if (string.IsNullOrEmpty(recordNo))
        {
            return View();
        }

        var cities = new[] { "CITYA", "CITYB", "CITYC" };
        Result<SettlementRecord>? result = null;

        foreach (var city in cities)
        {
            var apiUrl = $"{_configuration[$"ServiceUrls:Service{city}"]}/api/settlement/{recordNo}";
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<Result<SettlementRecord>>(responseContent);
                    if (result?.Success == true && result.Data != null)
                    {
                        break;
                    }
                }
            }
            catch
            {
                continue;
            }
        }

        ViewBag.Result = result;
        return View();
    }
}