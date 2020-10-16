using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBCDBModule.DB
{
    class DataContext :DbContext
    {
      public  DbSet<User> Users { get; set; }
      public DbSet<DeviceType> DeviceTypes { get; set; }
     protected override void  OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
            optionsBuilder.UseSqlite(SBCDB.ConnectionString);
       } 
    }
}
