// File: TourManagementSystem/Models/ErrorViewModel.cs
namespace TourManagementSystem.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}