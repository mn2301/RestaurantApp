using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantApp
{
    [Table("clients")]
    public class UserData:BaseModel
    {
        [PrimaryKey("id")]
        public int id { get; set; }

        [Column("googletoken")]
        public string clientGoogleToken { get; set; }

        [Column("name")]
        public string clientName { get; set; }

        [Column("email")]
        public string clientEmail { get; set; }

        [Column("imageURL")]
        public string? clientImageURL { get; set; }

        [Column("phonenum")]
        public string? clientPhone { get; set; }

        [Column("address")]
        public string? clientAddress { get; set; }

        [Column("usertype")]
        public string clientType { get; set; }
    }
}
