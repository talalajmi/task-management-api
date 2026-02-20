public class AppUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation property - one user has many projects
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    
    // Computed property - no database column needed
    public string FullName => $"{FirstName} {LastName}";
}