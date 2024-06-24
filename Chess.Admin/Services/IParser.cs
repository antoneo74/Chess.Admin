using Chess.Admin.Models;

namespace Chess.Admin.Services
{
    public interface IParser
    {
        public Board? Parse(string fen);
    }
}
