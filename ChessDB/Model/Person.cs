using System.ComponentModel.DataAnnotations;

namespace ChessDB.Model
{
    public class Person
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public int TotalExercises {  get; set; }

        public int Success {  get; set; }

        public int Failure { get; set; }
    }
}
