using System.ComponentModel.DataAnnotations;

namespace BoardAccess.Models
{
    public class Generation : DbEntity
    {
        [Required]
        public Board Board { get; set; }
        [Required]
        public Guid BoardId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public bool IsFinalState { get; set; }
    }
}
