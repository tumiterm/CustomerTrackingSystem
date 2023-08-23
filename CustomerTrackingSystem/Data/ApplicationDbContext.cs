using CustomerTrackingSystem.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore;

namespace CustomerTrackingSystem.Data
{
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }
            public DbSet<Customer> Customers { get; set; }
            public DbSet<Address> Addresses { get; set; }
            public DbSet<Contact> Contacts { get; set; }

        }
    }

