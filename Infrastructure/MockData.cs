using Bogus;
using Bogus.DataSets;
using Bogus.Extensions.Italy;
using Domain.Entities;
using Address = Domain.Entities.Address;


namespace Infrastructure;

public class MockData
{
    
    public List<User> Users { get; set; }
    
    private const int USER_COUNT = 20;

    internal void Init()
    {
        var addressMock = new Faker<Address>()
            .StrictMode(true)
            .RuleFor(a => a.Street, f => f.Address.StreetAddress())
            .RuleFor(a => a.Zipcode, f => f.Address.ZipCode())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.Province, f => f.Address.Country())
            .RuleFor(a => a.Country, f => f.Address.State());

        var userMock = new Faker<User>()
            .StrictMode(false)
            .RuleFor(u => u.Id, () => Guid.NewGuid())
            .RuleFor(u => u.Firstname, f => f.Person.FirstName)
            .RuleFor(u => u.Lastname, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.BirthDate, f => f.Person.DateOfBirth)
            .RuleFor(u => u.Gender, f => f.Person.Gender.ToString())
            .RuleFor(u => u.Addresses, f => addressMock.Generate(f.Random.Int(1, 5)));

        Users = userMock.Generate(USER_COUNT);

    }
}