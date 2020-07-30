using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Films.Models
{
    public class Film
    {
        public long Id { get; set; }
        [Display(Name="Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Год выпуска")]
        public int Year { get; set; }
        [Display(Name = "Режиссер")]
        public string Producer { get; set; }
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public string PosterFileNameOnDisk { get; set; }
        public string PosterFileNameOriginally { get; set; }
    }
}
