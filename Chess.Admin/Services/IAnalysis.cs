using Chess.Admin.Models;

namespace Chess.Admin.Services
{
    public interface IAnalysis
    {
        public Board Analysis(Board board);
    }
}
