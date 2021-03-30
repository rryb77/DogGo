using System.Collections.Generic;
using System.ComponentModel;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Neighborhood")]
        public int NeighborhoodId { get; set; }

        [DisplayName("Image Url")]
        public string ImageUrl { get; set; }

        public Neighborhood Neighborhood { get; set; }
    }
}
