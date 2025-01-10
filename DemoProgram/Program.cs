using DemoProgram.Models;
using ORMazing;
using ORMazing.Config;

namespace DemoProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=MSI\\SQLEXPRESS11;Initial Catalog=DemoDB;Integrated Security=True;TrustServerCertificate=True";
            ORMazingProvider.Configure(connectionString, DatabaseType.SQLServer);
            if (!ORMazingProvider.DB.TestConnection())
            {
                Console.WriteLine("Failed to connect to the database.");
                return;
            }
            var DB = ORMazingProvider.DB;
            var userRepository = DB.GetRepository<User>();
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

            var updatedUser = new User
            {
                Id = 12,
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
            var deletedUser = new User
            {
                Id = 10,
                Name = "John",
                Age = 30
            };
            userRepository.Delete(deletedUser);
            users = userRepository.GetAll();
            Console.WriteLine("\nAfter deleting user:");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            Console.WriteLine("\n=== Test GetWithCondition ===");
            var usersWithCondition = userRepository.GetWithCondition("Age > 25");
            Console.WriteLine("\nUsers with Age > 25:");
            foreach (var user in usersWithCondition)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            var usersGroupedByAge = userRepository.GetWithCondition(groupByColumns: "Age");
            Console.WriteLine("\nUsers grouped by Age:");
            foreach (var user in usersGroupedByAge)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            var usersHavingCondition = userRepository.GetWithCondition(
                groupByColumns: "Age",
                havingCondition: "COUNT(*) > 1"
            );
            Console.WriteLine("\nUsers having COUNT(*) > 1:");
            foreach (var user in usersHavingCondition)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

            var usersOrderedByName = userRepository.GetWithCondition(orderByColumns: "Name ASC");
            Console.WriteLine("\nUsers ordered by Name ASC:");
            foreach (var user in usersOrderedByName)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }

        }
    }
}
