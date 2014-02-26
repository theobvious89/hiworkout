using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using hiworkout.Model;

namespace hiworkout
{
    public partial class StoredWorkouts : PhoneApplicationPage
    {
        public StoredWorkouts()
        {
            InitializeComponent();

            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.ViewModel.AllWorkoutTemplates.Count == 0)
            {
                NoWorkoutsText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// When there are no workout templates, when the content grid is tapped navigate to the 
        /// Create Workout page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void CreateWorkout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //if there are no workout templates
            if (App.ViewModel.AllWorkoutTemplates.Count == 0)
            {
                NavigationService.Navigate(new Uri("/CreateWorkout.xaml?msg=BeginWorkout", UriKind.RelativeOrAbsolute));

            }
        }

        private void allWorkoutsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;

            //verifying our sender is actually a longlistselector
            if (selector == null)
                return;

            WorkoutTemplate data = selector.SelectedItem as WorkoutTemplate;

            //verfiying our send is actually a workout template
            if (data == null)
                return;

            NavigationService.Navigate(new Uri("/RecordWorkout.xaml?workoutTemplateID=" + data.ID, UriKind.RelativeOrAbsolute));
        }
    }
}