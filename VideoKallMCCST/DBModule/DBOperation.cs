using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace VideoKallMCCST.DBModule
{
    class DBOperation :DbContext
    {
        string connectionString = "Data Source= SBCDB.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=netcore-cli
            optionsBuilder.UseSqlite(connectionString);

       }
    }
}
