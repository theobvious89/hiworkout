using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hiworkout.ViewModels
{
    public class Menu
    {
        public Menu()
        {
            Items = new List<MenuItem>();
        }

        public List<MenuItem> Items { get; set; }
        public string Title { get; set; }
    }
}