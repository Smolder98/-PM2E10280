using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace PM2E10280.Models
{
    public class Sitio
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public String descripcion { get; set; }
        public String pathImage { get; set; }
        public Byte[] image { get; set; }
    }
}
