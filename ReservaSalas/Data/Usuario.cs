namespace ReservaSalas.Data
{


    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;




    namespace ReservaSalas.Models
    {
        // A classe herda de IdentityUser para adicionar campos customizados
        public class Usuario : IdentityUser
        {


            [Required(ErrorMessage = "O campo Nome é obrigatório.")]
            [StringLength(100)]
            public string Nome { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo Sobrenome é obrigatório.")]
            [StringLength(100)]
            public string Sobrenome { get; set; } = string.Empty;

            [Phone(ErrorMessage = "O Telefone não está em um formato válido.")]
            [Display(Name = "Telefone")]
            public string? Telefone { get; set; }

            [Required(ErrorMessage = "O campo Sexo é obrigatório.")]
            [StringLength(10)]
            public string Sexo { get; set; } = string.Empty;
        }
    }
}