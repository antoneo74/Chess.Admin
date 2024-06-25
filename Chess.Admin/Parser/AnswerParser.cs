using Chess.Admin.Models;
using System.Collections.ObjectModel;

namespace Chess.Admin.Parser
{
    public static class AnswerParser
    {
        public static void Parse(string s, ref User user, ref ObservableCollection<Exercise> ListItems)
        {
            var array = s.Split('\n');

            user = new(array[0], array[1]);

            if (ListItems.Count != 0) ListItems.Clear();

            for (int i = 2, index = 1; i < array.Length; i++)
            {
                array[i] = array[i].Trim();

                if (array[i] != string.Empty)
                {
                    var substring = array[i].Split(' ');

                    var item = new Exercise
                    {
                        Id = index,

                        WhiteCapture = int.Parse(substring[0]),

                        WhiteWeakness = int.Parse(substring[1]),

                        BlackCapture = int.Parse(substring[2]),

                        BlackWeakness = int.Parse(substring[3]),

                        FenItem = substring[4]
                    };
                    ListItems.Add(item);

                    index++;
                }
            }
        }
    }
}
