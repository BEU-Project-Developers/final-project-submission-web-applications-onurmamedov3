// File: TourManagementSystem/Controllers/AdminApiController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourManagementSystem.Services;
using TourManagementSystem.Models;
using System.Security.Claims; // For getting user ID if using Identity

// [Authorize(Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminApiController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IUserService _userService; // Assuming you have this for user ID retrieval for CreateHotel

    public AdminApiController(IHotelService hotelService, IUserService userService)
    {
        _hotelService = hotelService;
        _userService = userService;
    }

    [HttpGet("hotels")]
    public async Task<IActionResult> GetHotels()
    {
        var hotels = await _hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    [HttpGet("hotels/{id}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        if (hotel == null) return NotFound(new { message = "Hotel not found." });
        return Ok(hotel);
    }

    [HttpPost("hotels")]
    public async Task<IActionResult> CreateHotel([FromBody] HotelViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Placeholder for getting the logged-in admin's user ID.
        // Replace this with actual logic if you have authentication setup.
        // Example if using ASP.NET Core Identity and user is logged in:
        // var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentAdminUserId))
        // {
        //     return Unauthorized(new { message = "Admin user ID could not be determined." });
        // }
        int currentAdminUserId = 1; // <<< --- !!! Placeholder: Replace with actual admin ID logic !!!

        var result = await _hotelService.CreateHotelAsync(model, currentAdminUserId);

        if (result.Success && result.Hotel != null)
        {
            return CreatedAtAction(nameof(GetHotel), new { id = result.Hotel.Id }, result.Hotel);
        }
        return BadRequest(new { message = result.ErrorMessage ?? "Failed to create hotel." });
    }

    [HttpPut("hotels/{id}")]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] HotelViewModel model)
    {
        if (model.Id != 0 && id != model.Id) return BadRequest(new { message = "ID in URL does not match ID in body." });
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // If model.Id is 0 from the form, set it from the URL parameter for the service
        if (model.Id == 0) model.Id = id;

        var result = await _hotelService.UpdateHotelAsync(id, model); // result is a tuple (bool Success, string ErrorMessage)

        if (result.Success) // Access the 'Success' member of the tuple
        {
            return NoContent();
        }
        // If !result.Success
        if (result.ErrorMessage.Contains("not found") || result.ErrorMessage.Contains("deleted by another user"))
        {
            return NotFound(new { message = result.ErrorMessage });
        }
        return BadRequest(new { message = result.ErrorMessage ?? "Failed to update hotel." });
    }

    [HttpDelete("hotels/{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        bool success = await _hotelService.DeleteHotelAsync(id); // DeleteHotelAsync returns a bool
        if (!success) // Directly check the boolean
        {
            return NotFound(new { message = "Hotel not found or could not be deleted." });
        }
        return NoContent();
    }
}