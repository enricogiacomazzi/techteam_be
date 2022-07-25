namespace Domain.Entities;

public class User: Entity<Guid>
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    // public string Taxcode { get; set; }
    public List<Address> Addresses { get; set; }
    
}