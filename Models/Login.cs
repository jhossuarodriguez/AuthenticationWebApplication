using System.ComponentModel.DataAnnotations;

namespace AuthenticationWebApplication.Models;
public class Login
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener entre 4 y 20 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "El nombre de usuario solo puede contener letras, números y guiones bajos.")]
    public string? Username { get; set; } = null;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "La contraseña debe contener al menos una letra y un número.")]
    public string Password { get; set; } = null!;

    public string? RememberMe { get; set; } = null;

}
