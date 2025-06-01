//// File: TourManagementSystem/Services/BookingService.cs
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading.Tasks;
//using TourManagementSystem.Data;
//using TourManagementSystem.Models;

//namespace TourManagementSystem.Services
//{
//    public class BookingService : IBookingService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<BookingService> _logger;

//        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        public async Task<(bool Success, Booking? CreatedBooking, string ErrorMessage)> CreateBookingAsync(Booking booking)
//        {
//            if (booking == null)
//            {
//                _logger.LogWarning("CreateBookingAsync called with null booking object.");
//                return (false, null, "Booking data cannot be null.");
//            }

//            try
//            {
//                // Ensure BookingDate is set if not already (though constructor handles UTC now)
//                if (booking.BookingDate == default(DateTime))
//                {
//                    booking.BookingDate = DateTime.UtcNow;
//                }
//                // Default status if somehow not set (constructor handles Pending now)
//                if (booking.Status == default(BookingStatus) && !Enum.IsDefined(typeof(BookingStatus), booking.Status))
//                {
//                    booking.Status = BookingStatus.Pending;
//                }

//                _context.Bookings.Add(booking);
//                await _context.SaveChangesAsync();
//                _logger.LogInformation("Booking created successfully with ID: {BookingId} for OfferType: {OfferType}, OfferId: {OfferId}",
//                                       booking.Id, booking.OfferType, booking.OfferId);
//                return (true, booking, "Booking created successfully.");
//            }
//            catch (DbUpdateException dbEx) // More specific exception for DB errors
//            {
//                _logger.LogError(dbEx, "Database error creating booking for OfferType: {OfferType}, OfferId: {OfferId}. InnerException: {InnerEx}",
//                                 booking.OfferType, booking.OfferId, dbEx.InnerException?.Message);
//                return (false, null, $"A database error occurred while creating the booking. Please try again. Details: {dbEx.InnerException?.Message ?? dbEx.Message}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "General error creating booking for OfferType: {OfferType}, OfferId: {OfferId}",
//                                 booking.OfferType, booking.OfferId);
//                return (false, null, $"An unexpected error occurred while creating the booking: {ex.Message}");
//            }
//        }
//    }
//}