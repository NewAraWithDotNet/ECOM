using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace OnlineOrderingSystem.ViewModels
{
    public class SliderModel : PageModel
    {
        public List<Slide> Slides { get; set; } = new List<Slide>();

        public void OnGet()
        {
            Slides = new List<Slide>
            {
                new Slide { Id = 1, Title = "Summer Sale Collections", Description = "Sale! Up to 50% off!", Img = "https://images.pexels.com/photos/1926769/pexels-photo-1926769.jpeg?auto=compress&cs=tinysrgb&w=800", Url = "/", Bg = "bg-gradient-to-r from-yellow-50 to-pink-50" },
                new Slide { Id = 2, Title = "Winter Sale Collections", Description = "Sale! Up to 50% off!", Img = "https://images.pexels.com/photos/1021693/pexels-photo-1021693.jpeg?auto=compress&cs=tinysrgb&w=800", Url = "/", Bg = "bg-gradient-to-r from-pink-50 to-blue-50" },
                new Slide { Id = 3, Title = "Spring Sale Collections", Description = "Sale! Up to 50% off!", Img = "https://images.pexels.com/photos/1183266/pexels-photo-1183266.jpeg?auto=compress&cs=tinysrgb&w=800", Url = "/", Bg = "bg-gradient-to-r from-blue-50 to-yellow-50" }
            };
        }
    }

    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public string Url { get; set; }
        public string Bg { get; set; }
    }
}
