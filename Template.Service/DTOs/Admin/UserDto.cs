namespace Template.Service.DTOs.Admin
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
    }
}
