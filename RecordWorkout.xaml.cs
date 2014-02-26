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
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace hiworkout
{
    public partial class RecordWorkout : PhoneApplicationPage
    {
        private Grid exercisesGrid;
        private bool recordExerciseStatsGridIsOpen = false;
        private DateTime workoutStartTime;
        private WorkoutTemplate CurrentWorkoutTemplate;
        private Dictionary<string, Storyboard> storyBoardDictionary;
        private const int EXERCISEGRIDROW_PIXEL_HEIGHT = 80;
        public RecordWorkout()
        {
            InitializeComponent();

            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel;
            

            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string stringWorkoutTemplateID = "";
            //get the workout template for this workout and then get a list of all exercises in that workout based
            //on the that workout template id
            if (NavigationContext.QueryString.TryGetValue("workoutTemplateID", out stringWorkoutTemplateID))
            {
                int intWorkoutTemplateID = Convert.ToInt32(stringWorkoutTemplateID);
                CurrentWorkoutTemplate = App.ViewModel.GetExercisesFromWorkoutTemplate(intWorkoutTemplateID);
            }
            CreateExercisesGrid();
            workoutStartTime = DateTime.Now;
        }

        /// <summary>
        /// Creates the exercise Grid and all the exercises that are in the workout
        /// </summary>
        private void CreateExercisesGrid()
        {
            //create the exercisesGrid
            exercisesGrid = new Grid();
            ContentPanel.Children.Clear();

            ContentPanel.Children.Add(exercisesGrid);
            storyBoardDictionary = new Dictionary<string, Storyboard>();
            int currentGridRow = 0;
            //for each exercise in the exercises in this workout
            foreach(Exercise exercise in App.ViewModel.ExercisesForGivenWorkoutTemplate)
            {
                //define a new grid row of EXERCISEGRIDROW_PIXEL_HEIGHT pixels
                exercisesGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(EXERCISEGRIDROW_PIXEL_HEIGHT, GridUnitType.Pixel) });

                //add a new exercise heading Grid that will contain the exercise name
                exercisesGrid.Children.Add(new Grid
                {
                    Name = "Grid" + Convert.ToString(exercise.ID),
                    Background = new SolidColorBrush(Colors.Black)
                });

                Grid exerciseHeadingGrid = exercisesGrid.Children.ElementAt(currentGridRow) as Grid;
                
                //add the exercise text block to the heading Grid
                exerciseHeadingGrid.Children.Add(new TextBlock
                {
                    Name = Convert.ToString(exercise.ID),
                    Text = exercise.Name,
                    Padding = new Thickness(12,0,12,0),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Style = Resources["PhoneTextExtraLargeStyle"] as Style
                });

                //create down arrow icon for each exercise
                Grid downArrowIcon = new Grid
                {
                    Name = "downArrowIcon",
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    Height = 40,
                    Width = 40,
                    Margin = new Thickness(0, 6, 6, 0)
                };
                downArrowIcon.Children.Add(new Ellipse
                {
                    Stroke = (Brush)Application.Current.Resources["PhoneForegroundBrush"],
                    StrokeThickness = 3
                });
                Image downArrowIconImage = new Image
                {
                    Source = new BitmapImage(new Uri("/Assets/Images/next.png", UriKind.Relative))
                };
                downArrowIconImage.RenderTransformOrigin=new Point(0.5, 0.5); 
                downArrowIconImage.RenderTransform = new CompositeTransform{
                    Rotation = 90
                };
                downArrowIcon.Children.Add(downArrowIconImage);
                //add the down arrow icon to the heading
                exerciseHeadingGrid.Children.Add(downArrowIcon);

                exerciseHeadingGrid.SetValue(Canvas.ZIndexProperty, 99);
                exerciseHeadingGrid.RenderTransform = new CompositeTransform();

                //create the animation for moving a heading down
                Storyboard downStoryBoard = new Storyboard();
                
                DoubleAnimation movement = new DoubleAnimation();
                movement.From = 0;
                movement.To = EXERCISEGRIDROW_PIXEL_HEIGHT * 3;
                movement.Duration = new Duration(TimeSpan.Parse("0:0:0.3"));
                Storyboard.SetTarget(movement, exerciseHeadingGrid);
                Storyboard.SetTargetProperty(movement, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
                downStoryBoard.Children.Add(movement);
                LayoutRoot.Resources.Add("StoryDown" + exerciseHeadingGrid.Name, downStoryBoard);
                storyBoardDictionary.Add("StoryDown" + exerciseHeadingGrid.Name, downStoryBoard);

                //create the animation for moving a heading up
                Storyboard upStoryBoard = new Storyboard();

                DoubleAnimation movement2 = new DoubleAnimation();
                movement2.From = EXERCISEGRIDROW_PIXEL_HEIGHT * 3;
                movement2.To = 0;
                movement2.Duration = new Duration(TimeSpan.Parse("0:0:0.3"));
                Storyboard.SetTarget(movement2, exerciseHeadingGrid);
                Storyboard.SetTargetProperty(movement2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
                upStoryBoard.Children.Add(movement2);
                LayoutRoot.Resources.Add("StoryUp" + exerciseHeadingGrid.Name, upStoryBoard);
                storyBoardDictionary.Add("StoryUp" + exerciseHeadingGrid.Name, upStoryBoard);

                //create the animation for rotating the down arrow to up
                Storyboard rotateArrowUp = new Storyboard();

                DoubleAnimation rotate = new DoubleAnimation();
                rotate.From = 90;
                rotate.To = -90;
                rotate.Duration = new Duration(TimeSpan.Parse("0:0:0.3"));
                Storyboard.SetTarget(rotate, downArrowIconImage);
                Storyboard.SetTargetProperty(rotate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));
                rotateArrowUp.Children.Add(rotate);
                LayoutRoot.Resources.Add("RotateArrowUp" + exerciseHeadingGrid.Name, rotateArrowUp);
                storyBoardDictionary.Add("RotateArrowUp" + exerciseHeadingGrid.Name, rotateArrowUp);

                //create the animation for rotating the down arrow back down
                Storyboard rotateArrowDown = new Storyboard();

                DoubleAnimation rotate2 = new DoubleAnimation();
                rotate2.From = -90;
                rotate2.To = 90;
                rotate2.Duration = new Duration(TimeSpan.Parse("0:0:0.3"));
                Storyboard.SetTarget(rotate2, downArrowIconImage);
                Storyboard.SetTargetProperty(rotate2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));
                rotateArrowDown.Children.Add(rotate2);
                LayoutRoot.Resources.Add("RotateArrowDown" + exerciseHeadingGrid.Name, rotateArrowDown);
                storyBoardDictionary.Add("RotateArrowDown" + exerciseHeadingGrid.Name, rotateArrowDown);

                //define an event handler for each exercise textblock so they can react to taps
                exercisesGrid.Children.ElementAt(currentGridRow).Tap += new EventHandler<GestureEventArgs>(Exercise_Tap);
                //set the row in the grid of the exercise textblock
                exercisesGrid.Children.ElementAt(currentGridRow).SetValue(Grid.RowProperty, currentGridRow);
                currentGridRow++;
            }

            //add these row definitions at the end so there is a place for the drop down fill out area to be in
            exercisesGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(EXERCISEGRIDROW_PIXEL_HEIGHT, GridUnitType.Pixel) });
            exercisesGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(EXERCISEGRIDROW_PIXEL_HEIGHT, GridUnitType.Pixel) });
            exercisesGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(EXERCISEGRIDROW_PIXEL_HEIGHT, GridUnitType.Pixel) });

            exercisesGrid.Children.Add(recordExerciseStatsGrid);

        }

        /// <summary>
        /// When an exercise gets tapped on, open a pop out recording area for that exercise below it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exercise_Tap(object sender, GestureEventArgs e)
        {
            Grid exerciseHeadingTappedOn = sender as Grid;

            //verifying our sender is actually a Grid
            if (exerciseHeadingTappedOn == null)
                return;

            //add a new event handler, so that we can hide the pop out after it is completed moving back up
            MoveRecordExerciseStatsUp.Completed += new EventHandler(MoveRecordExerciseStatsUp_Completed);

            if (recordExerciseStatsGridIsOpen)
            {   
                if (MoveRecordExerciseStatsUp != null)
                {
                    //move the starting position of MoveRecordExerciseStatsUp animation down based on which exercise
                    //was tapped on
                    MoveStatsDoubleAnimationUp.SetValue(DoubleAnimation.FromProperty,
                        0.0 +
                        (int)exerciseHeadingTappedOn.GetValue(Grid.RowProperty) * EXERCISEGRIDROW_PIXEL_HEIGHT);

                    //move the starting position of MoveRecordExerciseStatsUp animation down based on which exercise
                    //was tapped on
                    MoveStatsDoubleAnimationUp.SetValue(DoubleAnimation.ToProperty,
                        -240.0 +
                        (int)exerciseHeadingTappedOn.GetValue(Grid.RowProperty) * EXERCISEGRIDROW_PIXEL_HEIGHT);

                }
                //move the other exercise headings back to the original position and close the popup
                foreach (object item in exercisesGrid.Children)
                {
                    Grid exerciseHeading = item as Grid;

                    //if the item in the grid is an exerciseBLock and this block is in a row greater than the one that was tapped on
                    if (exerciseHeading != null && (int)exerciseHeading.GetValue(Grid.RowProperty) > (int)exerciseHeadingTappedOn.GetValue(Grid.RowProperty))
                    {
                        //start the animation to move this heading back up
                        Storyboard storyBoardToStart;
                        if (storyBoardDictionary.TryGetValue("StoryUp" + exerciseHeading.Name, out storyBoardToStart))
                        {
                            storyBoardToStart.Begin();
                        }
                    }
                }
                //move the pop out back up
                MoveRecordExerciseStatsUp.Begin();

                //start the animation to rotate the arrow in the heading
                Storyboard rotateArrowDownStoryboard;
                if (storyBoardDictionary.TryGetValue("RotateArrowDown" + exerciseHeadingTappedOn.Name, out rotateArrowDownStoryboard))
                {
                    rotateArrowDownStoryboard.Begin();
                }

                recordExerciseStatsGridIsOpen = false;
            }
            else
            {
                //open up the recordExerciseStatsGrid pop out below the selected exercise
                recordExerciseStatsGrid.Visibility = Visibility.Visible;
                if (MoveRecordExerciseStatsDown != null)
                {
                    //move the starting position of MoveRecordExerciseStatsDown animation down based on which exercise
                    //was tapped on
                    MoveStatsDoubleAnimationDown.SetValue(DoubleAnimation.FromProperty,
                        -240.0 +
                        (int)exerciseHeadingTappedOn.GetValue(Grid.RowProperty) * EXERCISEGRIDROW_PIXEL_HEIGHT);

                    //move the starting position of MoveRecordExerciseStatsDown animation down based on which exercise
                    //was tapped on
                    MoveStatsDoubleAnimationDown.SetValue(DoubleAnimation.ToProperty,
                        0.0 +
                        (int)exerciseHeadingTappedOn.GetValue(Grid.RowProperty) * EXERCISEGRIDROW_PIXEL_HEIGHT);
                    
                }
                //move the other exercise headings down below the pop out grid
                foreach (object item in exercisesGrid.Children)
                {
                    Grid exerciseHeading= item as Grid;

                    //if the item in the grid is an exerciseBLock and this block is in a row greater than the one that was tapped on
                    if (exerciseHeading != null && (int)exerciseHeading.GetValue(Grid.RowProperty) > (int)exerciseHeadingTappedOn.GetValue(Grid.RowProperty))
                    {
                        //start the animation to move this heading back up
                        Storyboard storyBoardToStart;
                        if (storyBoardDictionary.TryGetValue("StoryDown" + exerciseHeading.Name, out storyBoardToStart))
                        {
                            storyBoardToStart.Begin();
                        }
                    }
                }
                //move the pop out grid down
                MoveRecordExerciseStatsDown.Begin();

                //start the animation to rotate the arrow in the heading
                Storyboard rotateArrowUpStoryboard;
                if (storyBoardDictionary.TryGetValue("RotateArrowUp" + exerciseHeadingTappedOn.Name, out rotateArrowUpStoryboard))
                {
                    rotateArrowUpStoryboard.Begin();
                }

                recordExerciseStatsGridIsOpen = true;
            }
        }

        //when the exercise stats drop down has finished moving back up, make it invisible again
        private void MoveRecordExerciseStatsUp_Completed(object sender, EventArgs e)
        {
            recordExerciseStatsGrid.Visibility = Visibility.Collapsed;
        }
        
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton saveWorkoutAppBar =
                new ApplicationBarIconButton();

            saveWorkoutAppBar.IconUri = new Uri("/Assets/Images/save.png", UriKind.Relative);
            saveWorkoutAppBar.Text = AppResources.AppBarCreateWorkout;

            saveWorkoutAppBar.Click += saveWorkoutAppBar_Click;

            ApplicationBar.Buttons.Add(saveWorkoutAppBar);

            ApplicationBar.IsVisible = true;
        }

        private void saveWorkoutAppBar_Click(object sender, EventArgs e)
        {
            TimeSpan lengthOfWorkout = DateTime.Now - workoutStartTime;

            DateTime dt = new DateTime(2000, 01, 01);
            dt = dt + lengthOfWorkout;

            App.ViewModel.AddWorkout(new Workout { DateTimeOfWorkout = DateTime.Now, LengthOfWorkout = dt, WorkoutTemplate = CurrentWorkoutTemplate });
        }
        DateTime start;
        private void StartTime_Tap(object sender, GestureEventArgs e)
        {
            // creating timer instance
            DispatcherTimer newTimer = new DispatcherTimer();
            // timer interval specified as 1 second
            newTimer.Interval = TimeSpan.FromSeconds(1);
            // Sub-routine OnTimerTick will be called at every 1 second
            newTimer.Tick += OnTimerTick;
            // starting the timer
            newTimer.Start();
            DateTime start = DateTime.Now;
        }


        void OnTimerTick(Object sender, EventArgs args)
        {
            // text box property is set to current system date.
            // ToString() converts the datetime value into text
            TimeSpan currentTime = (DateTime.Now - start);
            TULTextBox.Text = currentTime.ToString();
        }

    }
}