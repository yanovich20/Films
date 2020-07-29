using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Films.Models
{
    public class Film
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string Producer { get; set; }
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public string PosterFileNameOnDisk { get; set; }
        public string PosterFileNameOriginally { get; set; }
    }
}
