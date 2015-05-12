using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.ItemModel
{
    public class Result
    {
        public int Id { get; private set; }
        public string DisplayTitle { get; private set; }
        public string DisplaySubtitle { get; private set; }

        private Result()
        {

        }

        public static Result Create(IPlugin plugin, string displayTitle, string displaySubtitle = "", int Id = 0)
        {

        }
    }
}
