using DemoProgram.Models;
using ORMazing;
using ORMazing.Config;
using ORMazing.Core.Common;
using ORMazing.Core.Models.Condition;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.Factories;
using ORMazing.DataAccess.QueryBuilders;
using System.Linq.Expressions;

namespace DemoProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost,1433;Database=DemoDB;User Id=sa;Password=Password123;TrustServerCertificate=True;";
            ORMazingProvider.Configure(connectionString, DatabaseType.SQLServer);
            if (!ORMazingProvider.DB.TestConnection())
            {
                Console.WriteLine("Failed to connect to the database.");
                return;
            }
            var DB = ORMazingProvider.DB;
            var userRepository = DB.GetRepository<User>();
            //var users = userRepository.GetAll();
            ////Add a user
            //Console.WriteLine("Before adding a new user:");
            //foreach (var user in users)
            //{
            //    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            //}

            //var newUser = new User
            //{
            //    Id = users[users.Count - 1].Id + 1,
            //    Name = "John",
            //    Age = 30
            //};
            //userRepository.Add(newUser);
            //users = userRepository.GetAll();

            //Console.WriteLine("After adding a new user:");
            //foreach (var user in users)
            //{
            //    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            //}
            ////Update a User (by Id)
            //var updatedUser = new User
            //{
            //    Id = 1,
            //    Name = "Updated",
            //    Age = 35
            //};
            //userRepository.Update(updatedUser);
            //users = userRepository.GetAll();
            //Console.WriteLine("\nAfter updating user with Id = 1:");
            //foreach (var user in users)
            //{
            //    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            //}

            ////Delete a User 
            //var deletedUser = new User
            //{
            //    Id = 2,
            //    Name = "Bob",
            //    Age = 25
            //};
            //userRepository.Delete(deletedUser);
            //users = userRepository.GetAll();
            //Console.WriteLine("\nAfter deleting user:");
            //foreach (var user in users)
            //{
            //    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            //}

            //// Get with Id,Name
            //var selectedColumns = new List<string> { "Id", "Name" };
            //var usersWithCondition = userRepository.GetWithCondition(whereCondition: "Age > 25");
            //var usersWithSelectedColumns = userRepository.GetWithCondition(selectedColumns);
            //Console.WriteLine("\nUsers (Id and Name only):");
            //foreach (var user in usersWithSelectedColumns)
            //{
            //    Console.WriteLine($"Id: {user["Id"]}, Name: {user["Name"]}");
            //}

            //// Get with Id,Name with Where
            //var usersWithCondition1 = userRepository.GetWithCondition(selectedColumns = new List<string> { "Id", "Name" }, whereCondition: "Age > 25");
            //Console.WriteLine("\nUsers (Age > 25, Id and Name only):");
            //foreach (var user in usersWithCondition)
            //{
            //    Console.WriteLine($"Id: {user["Id"]}, Name: {user["Name"]}");
            //}

            ////Get all with Where
            //Console.WriteLine("\nUsers with Age > 25:");
            //usersWithCondition = userRepository.GetWithCondition(whereCondition: "Age > 25");
            //foreach (var user in usersWithCondition)
            //{
            //    Console.WriteLine($"Id:  {user["Id"]}, Name:  {user["Name"]}, Age:  {user["Age"]}");
            //}

            ////Get Age, Count(*) with Group By Age
            //var usersGroupedByAge = userRepository.GetWithCondition(
            //    selectedColumns: new List<string> { "Age", "COdgfadfgUNT(*) AS UserCount" },
            //    groupByColumns: new List<string> { "Age" }
            //);
            //Console.WriteLine("\nUsers grouped by Age:");
            //foreach (var user in usersGroupedByAge)
            //{
            //    Console.WriteLine($"Age: {user["Age"]}, UserCount: {user["UserCount"]}");
            //}

            ////Get * with GroupBy, Having
            //var usersHavingCondition = userRepository.GetWithCondition(
            //    selectedColumns: new List<string> { "Age", "COUNT(*) AS UserCount" },
            //    groupByColumns: new List<string> { "Age" },
            //    havingCondition: "COUNT(*) > 1"
            //);
            //Console.WriteLine("\nUsers having COUNT(*) > 1:");
            //foreach (var user in usersHavingCondition)
            //{
            //    Console.WriteLine($"Age: {user["Age"]}, UserCount: {user["UserCount"]}");
            //}

            ////Get with Order
            //var usersOrderedByName = userRepository.GetWithCondition(orderByColumns: "Name ASC");
            //Console.WriteLine("\nUsers ordered by Name ASC:");
            //foreach (var user in usersOrderedByName)
            //{
            //    Console.WriteLine($"Id:  {user["Id"]}, Name:  {user["Name"]}, Age:  {user["Age"]}");
            //}


            //var queryBuilder = new SqlQueryBuilder<User>();

            var res = userRepository.Get(
                selector: o => new {
                    CustomerId = o.Id,
                    CustomerName = o.Name,
                    CustomerName2 = o.Name + " custom",
                    CustomerAge = o.Age,
                },
                whereCondition: Condition<User>.And(
                    Condition<User>.GreaterThan(o => o.Age, 20),
                    Condition<User>.LessThan(o => o.Age, 40)
                ),
                groupByColumns: [ "Name", "Id", "Age" ],
                havingCondition: Condition<User>.GreaterThan(o => o.Age, 30),
                orderBySelectors: [(o => o.Age, OrderType.Descending), (o => o.Id, OrderType.Ascending)]
            );

            for (int i = 0; i < res.Count; i++)
            {
                Console.WriteLine($"Id: {res[i].CustomerId}, Name: {res[i].CustomerName}, Name2: {res[i].CustomerName2}, Age: {res[i].CustomerAge}");
            }

            Console.WriteLine("==========\n");

            var res2 = userRepository.Get(
                columns: [ "Id", "Name" ],
                whereCondition: Condition<User>.And(
                    Condition<User>.GreaterThan(o => o.Age, 36),
                    Condition<User>.LessThan(o => o.Age, 39)
                )
            );

            for (int i = 0; i < res2.Count; i++)
            {
                Console.WriteLine($"Id: {res2[i]["Id"]}, Name: {res2[i]["Name"]}");
            }

            Console.WriteLine("=========\n");

            var res3 = userRepository.Get(
                columns: ["Id", "Name"],
                columnSelectors: [ o => o.Id, o => o.Name ],
                whereCondition: Condition<User>.Or(
                    Condition<User>.LessThanOrEqual(o => o.Age, 30),
                    Condition<User>.GreaterThanOrEqual(o => o.Age, 39)
                )
            );

            for (int i = 0; i < res3.Count; i++)
            {
                Console.WriteLine($"Id: {res3[i]["Id"]}, Name: {res3[i]["Name"]}");
            }
        }
    }
}
