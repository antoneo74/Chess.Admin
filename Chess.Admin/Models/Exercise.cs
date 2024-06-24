using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Admin.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        
        public string FenItem {  get; set; } = string.Empty;

        public int WhiteCapture { get; set; }

        public int WhiteWeakness {  get; set; }

        public int BlackCapture {  get; set; }

        public int BlackWeakness { get;set; }

        public bool IsDone {  get; set; }
    }
}
