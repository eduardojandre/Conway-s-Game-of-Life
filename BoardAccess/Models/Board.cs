using System.ComponentModel.DataAnnotations;

namespace BoardAccess.Models
{
    public class Board : DbEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string InitialState { get; set; }
    }
}
