namespace FoodOrdering.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
    }
    public class UserCreateDTO
    {
        public string FullName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Staff";
    }
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string Username { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; }
    }
    public class LoginDTO
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class UsersQuery
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
    }
}
