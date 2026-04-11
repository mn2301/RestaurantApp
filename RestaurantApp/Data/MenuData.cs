using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantApp
{
    // Model for menu table
    [Table("menu")]
    public class MenuData : BaseModel
    {
        [PrimaryKey("id")]
        public int id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("imageUrl")]
        public string ImageUrl { get; set; }

        [Column("price")]
        public float Price { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("characteristics")]
        public List<string> Characteristics { get; set; }


        [Column("availability")]
        public string Availability { get; set; }
    }
}
