using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAutomotriz.Models.Enums;

namespace SistemaAutomotriz.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingresa tu correo")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        [Display(Name = "Correo")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "Ingresa tu contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; } = "";

        [Display(Name = "Recordarme en este equipo")]
        public bool Recordarme { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Ingresa tu nombre completo")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "Ingresa tu correo")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        [StringLength(150, ErrorMessage = "El correo no puede superar 150 caracteres")]
        [Display(Name = "Correo")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "Ingresa una contraseña")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "La contraseña debe tener mayúscula, minúscula y número")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; } = "";

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [Compare(nameof(Clave), ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarClave { get; set; } = "";

        [Required(ErrorMessage = "Selecciona una sucursal")]
        [Display(Name = "Sucursal")]
        public int? SucursalId { get; set; }

        [Required(ErrorMessage = "Selecciona un rol")]
        [Display(Name = "Rol")]
        public RolUsuario? Rol { get; set; }

        public IEnumerable<SelectListItem> Sucursales { get; set; } = [];
        public IEnumerable<SelectListItem> Roles { get; set; } = [];
    }

    public record EncabezadoPaginaViewModel(
        string Titulo,
        string? Subtitulo = null,
        Func<object?, Microsoft.AspNetCore.Html.IHtmlContent>? Acciones = null);
}
