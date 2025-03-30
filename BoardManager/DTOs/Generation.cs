using BoardAccess.Models;

namespace BoardManager.DTOs
{
    public class Generation
    {
        public int Number { get; set; }
        public bool IsFinalState { get; set; }
        public bool[][] State { get; set; }
        public Board Board { get; set; }
    }
}
