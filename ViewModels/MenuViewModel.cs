using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hiworkout.ViewModels
{
    public class MenuViewModel
    {
        public Menu Welcome { get; set; }
        public Menu Progress { get; set; }
        public Menu Recent { get; set; }

        public bool IsDataLoaded { get; set; }

        public void LoadData()
        {
            //Load Data into model
            Welcome = CreateWelcomeMenu();

            IsDataLoaded = true;
        }

        private Menu CreateWelcomeMenu()
        {
            Menu data = new Menu();
            data.Title = "welcome";

            data.Items.Add(new MenuItem
            {
                Title = "Begin workout",
                HelpText = "Helptext",
                UriAddress = "StoredWorkouts.xaml"
            });

            data.Items.Add(new MenuItem
            {
                Title = "Create a new workout",
                HelpText = "Helptext",
                UriAddress = "CreateWorkout.xaml"
            });

            data.Items.Add(new MenuItem
            {
                Title = "Edit a stored workout",
                HelpText = "Helptext",
                UriAddress = "EditWorkout.xaml"
            });

            return data;
        }
    }
}
