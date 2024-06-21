using System.ComponentModel.DataAnnotations;

namespace ChessDB.Model
{
    public class Fen
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
                
        public int Strategy { get; set; }

        public int Tactics { get; set; }

        public int Score { get; set; }

        public int Technique { get; set; }

        public int Grade { get; set; }
    }
}
