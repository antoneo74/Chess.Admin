namespace Chess.Admin.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        
        public string FenItem {  get; set; } = string.Empty;

        public int WhiteCapture { get; set; }

        public bool WCError {  get; set; } = false;

        public int WhiteWeakness {  get; set; }

        public bool WWError { get; set; } = false ;

        public int BlackCapture {  get; set; }

        public bool BCError { get; set; } = false;

        public int BlackWeakness { get;set; }

        public bool BWError { get; set; } = false;

        public bool IsDone { get; set; } = false;
    }
}
