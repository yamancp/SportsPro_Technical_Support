using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using SportsPro.Models.DataLayer.SeedData;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.Extensions.Identity.Core;



namespace SportsPro.Models
{
    public class SportsProContext : IdentityDbContext<User>
    {

        public SportsProContext(DbContextOptions<SportsProContext> options)
            : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<CustProd> CustProds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
            modelBuilder.Entity<CustProd>()
                .HasKey(cp => new { cp.CustomerID, cp.ProductID });

          
            modelBuilder.Entity<CustProd>()
                .HasOne(cp => cp.Customer)
                .WithMany(c => c.CustProds)
                .HasForeignKey(cp => cp.CustomerID);

       
            modelBuilder.Entity<CustProd>()
                .HasOne(cp => cp.Product)
                .WithMany(p => p.CustProds)
                .HasForeignKey(cp => cp.ProductID);

            modelBuilder.ApplyConfiguration(new SeedProducts());

            modelBuilder.ApplyConfiguration(new SeedCustomers());

            modelBuilder.ApplyConfiguration(new SeedCountries());

            modelBuilder.ApplyConfiguration(new SeedTechnicians());
            modelBuilder.ApplyConfiguration(new SeedIncidents());

            modelBuilder.ApplyConfiguration(new SeedCustProds());
 
        }
       public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
          UserManager<User> userManager =
            serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string username = "admin";
            string password = "Sesame";
            string roleName = "Admin";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // if username doesn't exist, create it and add to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
        
        public static async Task CreateTechnicianRole(IServiceProvider serviceProvider)
        {
            
             UserManager<User> userManager =
            serviceProvider.GetRequiredService<UserManager<User>>();

            RoleManager<IdentityRole> roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] users = { "adiaz", "awilson", "gfiori", "gwendt", "jlee" };

            string userType = "Technician";

            // if role doesn't exist, create it
          if (await roleManager.FindByNameAsync(userType) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(userType));
            }

            foreach (var usr in users)
            {
                if (await userManager.FindByNameAsync(usr) == null)
                {
                    User user = new User { UserName = usr };
                    var result = await userManager.CreateAsync(user, "password");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, userType);
                    }

        }
        
    }
}
        }
    }


    
    


        
        
    
