using DemoProgram.Models;
using ORMazing;
using ORMazing.Config;
using ORMazing.Core.Common;
using ORMazing.Core.Models.Condition;
using ORMazing.Core.Models.Expressions;

namespace DemoProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost,1433;Database=DemoDB;User Id=sa;Password=Password123;TrustServerCertificate=True;";
            ORMazingProvider.Configure(connectionString, DatabaseType.SQLServer, new ORMazingConfig { LoggingConsole = true });
            if (!ORMazingProvider.DB.TestConnection())
            {
                Console.WriteLine("Failed to connect to the database.");
                return;
            }

            var userRepository = ORMazingProvider.DB.GetRepository<User>();

            Console.WriteLine("\n===TEST1: Add new user");

            var users = userRepository.GetAll();
            Console.WriteLine("Before adding a new user:");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }
            var newUser = new User
            {
                Id = users[users.Count - 1].Id + 1,
                Name = "John",
                Age = 30
            };
            userRepository.Add(newUser);

            users = userRepository.GetAll();
            Console.WriteLine("After adding a new user:");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            Console.WriteLine("\n===TEST2: Update user with Id = 1");
            var updatedUser = new User
            {
                Id = 1,
                Name = "Updated",
                Age = 35
            };
            userRepository.Update(updatedUser);
            users = userRepository.GetAll();

            Console.WriteLine("\nAfter updating user with Id = 1:");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            Console.WriteLine("\n===TEST3: Delete user with Id = 2");
            var deletedUser = new User
            {
                Id = 2,
                Name = "Bob",
                Age = 25
            };
            userRepository.Delete(deletedUser);
            users = userRepository.GetAll();
            Console.WriteLine("\nAfter deleting user:");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            Console.WriteLine("\n===TEST4: Get users with WHERE, GROUP BY, HAVING, ORDER BY");
            var res = userRepository.Get(
                selector: o => new {
                    CustomerId = o.Id,
                    CustomerName = o.Name,
                    CustomerName2 = o.Name + " custom",
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
                Console.WriteLine($"Id: {res[i].CustomerId}, Name: {res[i].CustomerName}, Name2: {res[i].CustomerName2}");
            }

            var res2 = userRepository.Get(
                columns: [ "Id", AggregateFunction.Min("Age", alias: "MinAge")],
                whereCondition: Condition<User>.And(
                    Condition<User>.GreaterThan(o => o.Age, 36),
                    Condition<User>.LessThan(o => o.Age, 39)
                ),
                groupByColumns: [ "Id", "Age" ]
            );
            for (int i = 0; i < res2.Count; i++)
            {
                Console.WriteLine($"Id: {res2[i]["Id"]}, MinAge: {res2[i]["MinAge"]}");
            }

            var res3 = userRepository.Get(
                columnSelectors: [ o => o.Id, o => o.Name],
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
