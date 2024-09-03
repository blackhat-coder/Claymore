using Bogus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Services.TextGeneration;

public class DataGenerator(ILogger<DataGenerator> _logger) : IDataGenerator
{
    private readonly Random _random = new Random();
    public Task<string> GenerateEmail()
    {
        // Example implementation for email generation
        var faker = new Faker("en");
        var email = faker.Internet.Email();

        return Task.FromResult(email);
    }

    public Task<string> GenerateName()
    {
        /*var names = new[] { "Alice", "Bob", "Charlie", "Diana" };
        var name = names[_random.Next(names.Length)];
*/
        var faker = new Faker("en");
        var name = faker.Name.FirstName();

        _logger.LogInformation($"Generated Name: {name}");
        return Task.FromResult(name);
    }

    public Task<int> GenerateNumber(int length)
    {
        if (length <= 0) {
            _logger.LogError("GenerateNumber: Lenth must be positive");
            throw new ArgumentException("Length must be positive.");
        }

        var min = (int)Math.Pow(10, length - 1);
        var max = (int)Math.Pow(10, length) - 1;

        var number = _random.Next(min, max + 1);
        _logger.LogInformation($"Generated Number: {number.ToString()}");
        return Task.FromResult(number);
    }

    public Task<string> GenerateString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[_random.Next(chars.Length)];
        }
        var data = new String(stringChars);
        _logger.LogInformation($"Generated String: {data}");
        return Task.FromResult(data);
    }
}
