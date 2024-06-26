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

        public int CaptureError {  get; set; }

        public int WeaknessError { get; set; }
    }
}
