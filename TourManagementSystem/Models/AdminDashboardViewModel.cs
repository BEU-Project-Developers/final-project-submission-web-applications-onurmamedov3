// File: TourManagementSystem/ViewModels/AdminDashboardViewModel.cs
using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models // Or YourProject.Models if that's where ViewModels live
{
    public class AdminDashboardViewModel
    {
        public string? UserName { get; set; }
        public int TotalHotelsCount { get; set; }
        public int TotalFlightsCount { get; set; }
        public int TotalCarsCount { get; set; } // Added based on typical needs
        public int TotalTripsCount { get; set; }
        public int TotalCruisesCount { get; set; }
        public int TotalActivitiesCount { get; set; }
        public int TotalBookingsCount { get; set; }
        public int TotalUsersCount { get; set; }
        public int PendingBookingsCount { get; set; }
        public int NewUsersTodayCount { get; set; }

        public List<ActivityLogViewModel> RecentActivities { get; set; } = new List<ActivityLogViewModel>();

        // For Booking Trends Chart
        public List<string> BookingTrendsLabels { get; set; } = new List<string>(); // e.g., Dates or Months
        public List<int> BookingTrendsData { get; set; } = new List<int>();   // e.g., Number of bookings
    }

    public class ActivityLogViewModel // Simple model for recent activities display
    {
        public string Action { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty; // User who performed action or related to action
        public DateTime Timestamp { get; set; }
        public string? DetailsUrl { get; set; } // Optional link to view more details
    }
}