using HostelFinder.Application.DTOs.ServiceCost.Request;
using HostelFinder.Application.DTOs.ServiceCost.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceCostController : ControllerBase
    {
        private readonly IServiceCostService _serviceCostService;

        public ServiceCostController(IServiceCostService serviceCostService)
        {
            _serviceCostService = serviceCostService;
        }

        [HttpGet]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetServiceCosts()
        {
            try
            {
                var response = await _serviceCostService.GetAllAsync();
                if (!response.Succeeded)
                {
                    return BadRequest(new Response<string>
                    {
                        Succeeded = false,
                        Message = response.Message
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetServiceCost(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new Response<ServiceCostResponseDto>
                    {
                        Succeeded = false,
                        Message = "Invalid ID",
                        Data = null
                    });
                }

                var response = await _serviceCostService.GetByIdAsync(id);
                if (!response.Succeeded || response.Data == null)
                {
                    return NotFound(new Response<ServiceCostResponseDto>
                    {
                        Succeeded = false,
                        Message = response.Message,
                        Data = null
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<ServiceCostResponseDto>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> CreateServiceCost([FromBody] CreateServiceCostDto serviceCostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid model state."
                });

            try
            {
                var response = await _serviceCostService.CreateServiceCost(serviceCostDto);
                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(new Response<string>
                {
                    Succeeded = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> UpdateServiceCost(Guid id, [FromBody] UpdateServiceCostDto serviceCostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _serviceCostService.UpdateAsync(id, serviceCostDto);
                if (!response.Succeeded)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> DeleteServiceCost(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid service cost ID");
                }

                var response = await _serviceCostService.DeleteAsync(id);
                if (!response.Succeeded)
                {
                    return NotFound(response.Message);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Landlord,Admin")]
        [Route("hostels")]
        public async Task<IActionResult> GetServiceCostsByHostel(Guid hostelId)
        {
            try
            {
                var response = await _serviceCostService.GetAllServiceCostByHostel(hostelId);

                if (!response.Succeeded || response.Data == null)
                {
                    return BadRequest(new Response<List<ServiceCostResponseDto>>
                    {
                        Succeeded = false,
                        Message = response.Message,
                        Data = new List<ServiceCostResponseDto>()
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

    }

}
