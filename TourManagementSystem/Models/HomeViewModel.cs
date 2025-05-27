// File: TourManagementSystem/Models/HomeViewModel.cs
using System.Collections.Generic;

namespace TourManagementSystem.Models
{
    public class HomeViewModel
    {
        public List<HotelViewModel> IntroTours { get; set; } = new List<HotelViewModel>();
        public List<HotelViewModel> CtaOffers { get; set; } = new List<HotelViewModel>();
        public List<HotelViewModel> BestRoomOffers { get; set; } = new List<HotelViewModel>();
        public List<HotelViewModel> TrendingNowOffers { get; set; } = new List<HotelViewModel>();
    }
}