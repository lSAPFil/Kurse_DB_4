using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Project1_01._08._2022.Models
{
    public class Log
    {
        public int IP { get; set; }
        public string Date { get; set; }
        public string Filters { get; set; }
    }

    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options)
       : base(options)
        {
            Logs = Set<Log>();
        }
        public DbSet<Log> Logs { get; set; }
    }
}
