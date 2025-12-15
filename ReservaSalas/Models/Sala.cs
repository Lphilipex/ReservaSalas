using System.ComponentModel.DataAnnotations;

namespace ReservaSalas.Models
{
    public class Sala
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Localizacao { get; set; }

        public int Capacidade { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}