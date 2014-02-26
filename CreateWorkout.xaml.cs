using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using hiworkout.Resources;
using hiworkout.Model;
using hiworkout.ViewModels;
using Coding4Fun.Toolkit.Controls;
using System.ComponentModel;
using System.Text;

namespace hiworkout
{
    public partial class CreateWorkout : PhoneApplicationPage
    {
        private string pageNavigatedFrom = "";

        public CreateWorkout()
        {
            InitializeComponent();

            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel;

            BuildLocalizedApplicationBar();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string msg = "";

            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
            {
                pageNavigatedFrom = msg;
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton addExerciseAppBar =
                new ApplicationBarIconButton();

            ApplicationBarIconButton createWorkoutAppBar =
                new ApplicationBarIconButton();

            addExerciseAppBar.IconUri = new Uri("/Assets/Images/add.png", UriKind.Relative);
            addExerciseAppBar.Text = AppResources.AppBarAddExercise;

            addExerciseAppBar.Click += addExerciseAppBar_Click;

            

            createWorkoutAppBar.IconUri = new Uri("/Assets/Images/save.png", UriKind.Relative);
            createWorkoutAppBar.Text = AppResources.AppBarCreateWorkout;

            createWorkoutAppBar.Click += createWorkoutAppBar_Click;

            ApplicationBar.Buttons.Add(createWorkoutAppBar);
            ApplicationBar.Buttons.Add(addExerciseAppBar);

            ApplicationBar.IsVisible = true;
        }

        private void createWorkoutAppBar_Click(object sender, EventArgs e)
        {
            //if there have been no exercises added to the template
            if (App.ViewModel.ExercisesForNewTemplate.Count == 0)
            {
                var noExercisesAddedPrompt = new MessagePrompt
                {
                    Title = "Error",
                    Message = "You have not added any exercises to your workout. Please add at least one."
                };
                noExercisesAddedPrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //noExercisesAddedPrompt.Completed += noExercisesAddedPrompt_Completed;
                noExercisesAddedPrompt.Show();
                return;
            }

            if (WorkoutName.Text.Equals(""))
            {
                var noWorkoutNamePrompt = new MessagePrompt
                {
                    Title = "Error",
                    Message = "Please enter a name for your workout."
                };
                noWorkoutNamePrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //noExercisesAddedPrompt.Completed += noExercisesAddedPrompt_Completed;
                noWorkoutNamePrompt.Show();
                return;
            }

            App.ViewModel.SubmitNewWorkoutTemplate(WorkoutName.Text);

            //checks to see what page we navigated from and if we orginally came from the begin workout
            //page, go straight to the record workout page. However if we came from the main menu "Create
            //workout page, go back to the main menu
            if (pageNavigatedFrom.Equals("BeginWorkout"))
            {
                NavigationService.Navigate(new Uri("/RecordWorkout.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
            
        }

        private void addExerciseAppBar_Click(object sender, EventArgs e)
        {
            TitlePanel.Opacity = 0.3;
            ContentPanel.Opacity = 0.3;
            ChooseExercisePopUp.IsOpen = true;
            ApplicationBar.IsVisible = false;
            this.Focus();

        }

        private void allExercisesSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            LongListSelector selector = sender as LongListSelector;

            //verifying our sender is actually a longlistselector
            if (selector == null)
                return;

            Exercise data = selector.SelectedItem as Exercise;

            //verfiying our send is actually an exercise
            if (data == null)
                return;

            
            bool exerciseAlreadyExists = false;
            
            foreach (Exercise exercise in App.ViewModel.ExercisesForNewTemplate)
            {
                if (exercise.ID == data.ID)
                {
                    exerciseAlreadyExists = true;
                }
            }

            if (exerciseAlreadyExists)
            {
                var exerciseAlreadyExistsPrompt = new MessagePrompt
                {
                    Title = "Error",
                    Message = "You cannot add the same exercise to a workout twice."
                };
                exerciseAlreadyExistsPrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                exerciseAlreadyExistsPrompt.Completed += exerciseAlreadyExistsPrompt_Completed;
                ChooseExercisePopUp.IsOpen = false;
                exerciseAlreadyExistsPrompt.Show();
            }
            else
            {
                App.ViewModel.ExercisesForNewTemplate.Add(data);

                if (App.ViewModel.ExercisesForNewTemplate.Count > 0)
                {
                    ExerciseHint.Visibility = Visibility.Collapsed;
                }
                ChooseExercisePopUp.IsOpen = false;
                ApplicationBar.IsVisible = true;
                TitlePanel.Opacity = 1;
                ContentPanel.Opacity = 1;
            }
            selector.SelectedItem = null;
            
        }

        private void exerciseAlreadyExistsPrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            ChooseExercisePopUp.IsOpen = true;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (ChooseExercisePopUp.IsOpen)
            {
                ChooseExercisePopUp.IsOpen = false;
                ApplicationBar.IsVisible = true;
                TitlePanel.Opacity = 1;
                ContentPanel.Opacity = 1;
                e.Cancel = true;
            }
        }
    }
}