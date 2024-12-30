using DemoProgram.Models;
using ORMazing.Config;
using ORMazing;

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
            foreach (var user in users) {

                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }
        }
    }
}
