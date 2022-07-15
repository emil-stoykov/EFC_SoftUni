using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            string inputJson = File.ReadAllText(@"C:\Users\Owner\source\repos\JSON_Exercises\CarDealer\CarDealer\Datasets\suppliers.json");
            string suppliers = ImportSuppliers(context, inputJson);

            inputJson = File.ReadAllText(@"C:\Users\Owner\source\repos\JSON_Exercises\CarDealer\CarDealer\Datasets\parts.json");
            string parts = ImportParts(context, inputJson);

            inputJson = File.ReadAllText(@"C:\Users\Owner\source\repos\JSON_Exercises\CarDealer\CarDealer\Datasets\cars.json");
            string cars = ImportCars(context, inputJson);

            inputJson = File.ReadAllText(@"C:\Users\Owner\source\repos\JSON_Exercises\CarDealer\CarDealer\Datasets\customers.json");
            string customers = ImportCustomers(context, inputJson);

            inputJson = File.ReadAllText(@"C:\Users\Owner\source\repos\JSON_Exercises\CarDealer\CarDealer\Datasets\sales.json");
            string sales = ImportSales(context, inputJson);

            //string ex14 = GetOrderedCustomers(context);
            //string ex15 = GetCarsFromMakeToyota(context);
            //string ex16 = GetLocalSuppliers(context);
            //string ex17 = GetCarsWithTheirListOfParts(context);
            //string ex18 = GetTotalSalesByCustomer(context);
            //string ex19 = GetSalesWithAppliedDiscount(context);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<CarDealerProfile>());

            var mapper = config.CreateMapper();

            List<int> suppliersId = context.Suppliers.Select(x => x.Id).ToList();

            IEnumerable<SupplierInputModel> dtoSupplier
                = JsonConvert.DeserializeObject<IEnumerable<SupplierInputModel>>(inputJson);

            IEnumerable<Supplier> suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSupplier);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";

        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<CarDealerProfile>());

            var mapper = config.CreateMapper();

            List<int> suppliersId = context.Suppliers.Select(x => x.Id).ToList();

            IEnumerable<PartInputModel> dtoParts
                = JsonConvert.DeserializeObject<IEnumerable<PartInputModel>>(inputJson)
                .Where(x => suppliersId.Contains(x.SupplierId));

            IEnumerable<Part> parts = mapper.Map<IEnumerable<Part>>(dtoParts);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<CarDealerProfile>());

            var mapper = config.CreateMapper();

            IEnumerable<CarInputModel> dtoCars
                = JsonConvert.DeserializeObject<IEnumerable<CarInputModel>>(inputJson);

            List<Car> cars = new List<Car>();

            foreach (var dtoCar in dtoCars)
            {
                Car newCar = new Car{
                    Make = dtoCar.Make,
                    Model = dtoCar.Model,
                    TravelledDistance = dtoCar.TravelledDistance
                };
                foreach(int partId in dtoCar.PartsId.Distinct())
                {
                    newCar.PartCars.Add(new PartCar { PartId = partId});
                }

                cars.Add(newCar);
            }

            //IEnumerable<Car> cars = mapper.Map<IEnumerable<Car>>(dtoCars);
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            var mapper = config.CreateMapper();

            IEnumerable<CustomerInputModel> dtoCustomers
                = JsonConvert.DeserializeObject<IEnumerable<CustomerInputModel>>(inputJson);

            IEnumerable<Customer> customers = mapper.Map<IEnumerable<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IEnumerable<SaleInputModel> dtoSales = JsonConvert.DeserializeObject<IEnumerable<SaleInputModel>>(inputJson);
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            var mapper = config.CreateMapper();

            IEnumerable<Sale> sales = mapper.Map<IEnumerable<Sale>>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers
                = context.Customers
                    .OrderBy(c => c.BirthDate)
                    .ThenBy(c => c.IsYoungDriver == true)
                    .Select(c => new
                    {
                        c.Name,
                        BirthDate = c.BirthDate.ToString("d"),
                        c.IsYoungDriver
                    })
                    .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars
                = context.Cars
                    .Where(c => c.Make.Equals("Toyota"))
                    .OrderBy(c => c.Model)
                    .ThenByDescending(c => c.TravelledDistance)
                    .Select(c => new
                    {
                        c.Id,
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    })
                    .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers
                = context.Suppliers
                    .Where(c => c.IsImporter == false)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        PartsCount = c.Parts.Count
                    })
                    .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars2
                = context.Cars
                    .Select(c => new
                    {
                        car = new
                        {
                            c.Make,
                            c.Model,
                            c.TravelledDistance
                        },
                        parts = c.PartCars.Select(p => new
                        {
                            Name = p.Part.Name,
                            Price = p.Part.Price.ToString("F2")
                        }
                    )
                    })
                    .ToList();

            return JsonConvert.SerializeObject(cars2, Formatting.Indented);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers
                = context.Customers
                    .Where(x => x.Sales.Any())
                    .Select(c => new
                    {
                        fullName = c.Name,
                        boughtCars = c.Sales.Count,
                        spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                    })
                    .OrderByDescending(c => c.spentMoney)
                    .ThenByDescending(c => c.boughtCars)
                    .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales
                = context.Sales
                    .Where(c => c.Discount > 0)
                    .Select(c => new
                    {
                        car = new
                        {
                            Make = c.Car.Make,
                            Model = c.Car.Model,
                            TravelledDistance = c.Car.TravelledDistance
                        },
                        customerName = c.Customer.Name,
                        Discount = c.Discount.ToString("F2"),
                        price = c.Car.PartCars.Sum(x => x.Part.Price).ToString("F2"),
                        priceWithDiscount = (c.Car.PartCars.Sum(p => p.Part.Price) - c.Car.PartCars.Sum(p => p.Part.Price) * c.Discount / 100).ToString("F2")
                    })
                    .Take(10)
                    .ToList();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}