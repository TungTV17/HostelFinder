using HostelFinder.Application.DTOs.Report;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
     [Route("api/reports")]
     [ApiController]
     public class ReportController : ControllerBase
     {
          private readonly IRevenueReportService _revenueReportService;

          public ReportController(IRevenueReportService revenueReportService)
          {
               _revenueReportService = revenueReportService;
          }

          [HttpPost("yearly-revenue")]
          [Authorize(Roles = "Landlord,Admin")]
          public async Task<IActionResult> GetYearlyRevenueReport(YearlyRevenueReportRequest request)
          {
               try
               {
                    var response = await _revenueReportService.GetYearlyRevenueReportByHostel(request.HostelId, request.Year);
                    if (!response.Succeeded)
                    {
                         return BadRequest(response);
                    }

                    return Ok(response);
               }
               catch (Exception ex)
               {
                    return BadRequest(new { Error = ex.Message });
               }
          }

          [HttpPost("monthly-revenue")]
          [Authorize(Roles = "Landlord,Admin")]
          public async Task<IActionResult> GetMonthlyRevenueReport(MonthlyRevenueReportRequest request)
          {
               try
               {
                    var response = await _revenueReportService.GetMonthlyRevenueReportByHostel(request.HostelId, request.Month, request.Year);
                    if (!response.Succeeded)
                    {
                         return BadRequest(response);
                    }

                    return Ok(response);
               }
               catch (Exception ex)
               {
                    return BadRequest(new { Error = ex.Message });
               }
          }
          
          //chi phí sửa chữa
          [HttpGet("maintenance-cost")]
          [Authorize(Roles = "Landlord,Admin")]
          public async Task<IActionResult> GetMaintenanceCostReport(Guid hostelId, int year)
          {
               try
               {
                    var response = await _revenueReportService.GetTotalCostOfMaintenanceRecordInYearAsync(hostelId, year);
                    if (!response.Succeeded)
                    {
                         return BadRequest(response);
                    }

                    return Ok(response);
               }
               catch (Exception ex)
               {
                    return BadRequest(new { Error = ex.Message });
               }
          }
     }
}