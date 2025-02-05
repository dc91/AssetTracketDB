using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AssetTracketDB
{
    internal class MyDbContext : DbContext
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AssetTracker;Integrated Security=True";

        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Mobile> Mobiles { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Currencies> Currencies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder ModelBuilder)
        {
            // --- Offices ---
            ModelBuilder.Entity<Office>().HasData(
                new Office { Id = 1, Name = "SWE", Currency = "SEK" }
            );

            ModelBuilder.Entity<Office>().HasData(
                new Office { Id = 2, Name = "USA", Currency = "USD" }
            );

            ModelBuilder.Entity<Office>().HasData(
                new Office { Id = 3, Name = "FIN", Currency = "EUR" }
            );

            // --- Laptops ---
            // Office 1 (SWE)
            ModelBuilder.Entity<Laptop>().HasData(
                new Laptop
                {
                    Id = 1,
                    Manufacturer = "Apple",
                    ModelName = "MacBook",
                    Price = 19990,
                    Currency = "SEK",
                    PurchaseDate = new DateOnly(2021, 3, 9),
                    OfficeId = 1
                },
                new Laptop
                {
                    Id = 2,
                    Manufacturer = "Apple",
                    ModelName = "MacBook Pro",
                    Price = 21500,
                    Currency = "SEK",
                    PurchaseDate = new DateOnly(2021, 7, 29),
                    OfficeId = 1
                },
                new Laptop
                {
                    Id = 3,
                    Manufacturer = "Apple",
                    ModelName = "MacBook Air",
                    Price = 20899,
                    Currency = "SEK",
                    PurchaseDate = new DateOnly(2023, 7, 15),
                    OfficeId = 1
                }
            );

            // Office 2 (USA)
            ModelBuilder.Entity<Laptop>().HasData(
                new Laptop
                {
                    Id = 4,
                    Manufacturer = "Asus",
                    ModelName = "Vivobook",
                    Price = 1899,
                    Currency = "USD",
                    PurchaseDate = new DateOnly(2023, 12, 23),
                    OfficeId = 2
                },
                new Laptop
                {
                    Id = 5,
                    Manufacturer = "Asus",
                    ModelName = "Zenbook",
                    Price = 1299,
                    Currency = "USD",
                    PurchaseDate = new DateOnly(2023, 12, 21),
                    OfficeId = 2
                },
                new Laptop
                {
                    Id = 6,
                    Manufacturer = "Asus",
                    ModelName = "ROG",
                    Price = 2099,
                    Currency = "USD",
                    PurchaseDate = new DateOnly(2023, 8, 4),
                    OfficeId = 2
                }
            );

            // Office 3 (FIN)
            ModelBuilder.Entity<Laptop>().HasData(
                new Laptop
                {
                    Id = 7,
                    Manufacturer = "Apple",
                    ModelName = "MacBook Pro",
                    Price = 1900,
                    Currency = "EUR",
                    PurchaseDate = new DateOnly(2021, 3, 21),
                    OfficeId = 3
                },
                new Laptop
                {
                    Id = 8,
                    Manufacturer = "HP",
                    ModelName = "Spectre",
                    Price = 1300,
                    Currency = "EUR",
                    PurchaseDate = new DateOnly(2023, 4, 13),
                    OfficeId = 3
                },
                new Laptop
                {
                    Id = 9,
                    Manufacturer = "Dell",
                    ModelName = "Inspiron",
                    Price = 1100,
                    Currency = "EUR",
                    PurchaseDate = new DateOnly(2023, 5, 9),
                    OfficeId = 3
                }
            );

            // --- Mobiles ---
            // Office 1 (SWE)
            ModelBuilder.Entity<Mobile>().HasData(
                new Mobile
                {
                    Id = 10,
                    Manufacturer = "Apple",
                    ModelName = "iPhone 14",
                    Price = 12990,
                    Currency = "SEK",
                    PurchaseDate = new DateOnly(2023, 7, 2),
                    OfficeId = 1
                },
                new Mobile
                {
                    Id = 11,
                    Manufacturer = "Apple",
                    ModelName = "iPhone 13",
                    Price = 11490,
                    Currency = "SEK",
                    PurchaseDate = new DateOnly(2023, 10, 11),
                    OfficeId = 1
                },
                new Mobile
                {
                    Id = 12,
                    Manufacturer = "Samsung",
                    ModelName = "Galaxy S22",
                    Price = 9990,
                    Currency = "SEK",
                    PurchaseDate = new DateOnly(2023, 8, 3),
                    OfficeId = 1
                }
            );

            // Office 2 (USA)
            ModelBuilder.Entity<Mobile>().HasData(
                new Mobile
                {
                    Id = 13,
                    Manufacturer = "Apple",
                    ModelName = "iPhone 14 Pro",
                    Price = 999,
                    Currency = "USD",
                    PurchaseDate = new DateOnly(2023, 9, 15),
                    OfficeId = 2
                },
                new Mobile
                {
                    Id = 14,
                    Manufacturer = "Samsung",
                    ModelName = "Galaxy S22",
                    Price = 799,
                    Currency = "USD",
                    PurchaseDate = new DateOnly(2022, 4, 17),
                    OfficeId = 2
                },
                new Mobile
                {
                    Id = 15,
                    Manufacturer = "Google",
                    ModelName = "Pixel 7",
                    Price = 699,
                    Currency = "USD",
                    PurchaseDate = new DateOnly(2022, 9, 23),
                    OfficeId = 2
                }
            );

            // Office 3 (FIN)
            ModelBuilder.Entity<Mobile>().HasData(
                new Mobile
                {
                    Id = 16,
                    Manufacturer = "Apple",
                    ModelName = "iPhone 14",
                    Price = 1100,
                    Currency = "EUR",
                    PurchaseDate = new DateOnly(2023, 1, 25),
                    OfficeId = 3
                },
                new Mobile
                {
                    Id = 17,
                    Manufacturer = "Samsung",
                    ModelName = "Galaxy S22",
                    Price = 900,
                    Currency = "EUR",
                    PurchaseDate = new DateOnly(2021, 8, 21),
                    OfficeId = 3
                },
                new Mobile
                {
                    Id = 18,
                    Manufacturer = "Google",
                    ModelName = "Pixel 7",
                    Price = 800,
                    Currency = "EUR",
                    PurchaseDate = new DateOnly(2021, 3, 31),
                    OfficeId = 3
                }
            );
        }
    }
}
