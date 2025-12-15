using Microsoft.AspNetCore.Identity;
using ReservaSalas.Data;
using ReservaSalas.Data.ReservaSalas.Models;
using ReservaSalas.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReservaSalas.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Sala é obrigatório.")]
        public int SalaId { get; set; }

        public string? UsuarioId { get; set; } 
        public Sala? Sala { get; set; } 
        public Usuario? Usuario { get; set; }


        [Required(ErrorMessage = "O campo Data da Reserva é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataReserva { get; set; }

        [Required(ErrorMessage = "O campo Hora Início é obrigatório.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "O campo Hora Fim é obrigatório.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraFim { get; set; }
    }
}
