using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Directive for the data model.
using hiworkout.Model;
using System.Text;
using System;


namespace hiworkout.ViewModels
{
    public class WorkoutViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.
        private hiworkoutDatabaseContext workoutDB;

        // Class constructor, create the data context object.
        public WorkoutViewModel()
        {
            workoutDB = new hiworkoutDatabaseContext(hiworkoutDatabaseContext.ConnectionString);
        }

        // All workouts
        private ObservableCollection<Workout> _allWorkouts;
        public ObservableCollection<Workout> AllWorkouts
        {
            get { return _allWorkouts; }
            set
            {
                _allWorkouts = value;
                NotifyPropertyChanged("AllWorkouts");
            }
        }

        // All Workout templates
        private ObservableCollection<WorkoutTemplate> _allWorkoutTemplates;
        public ObservableCollection<WorkoutTemplate> AllWorkoutTemplates
        {
            get { return _allWorkoutTemplates; }
            set
            {
                _allWorkoutTemplates = value;
                NotifyPropertyChanged("AllWorkoutTemplates");
            }
        }

        // All exercises
        private ObservableCollection<Exercise> _allExercises;
        public ObservableCollection<Exercise> AllExercises
        {
            get { return _allExercises; }
            set
            {
                _allExercises = value;
                NotifyPropertyChanged("AllExercises");
            }
        }

        // exercises for a new workout template
        private ObservableCollection<Exercise> _exercisesForNewTemplate;
        public ObservableCollection<Exercise> ExercisesForNewTemplate
        {
            get { return _exercisesForNewTemplate; }
            set
            {
                _exercisesForNewTemplate = value;
                NotifyPropertyChanged("ExercisesForNewTemplate");
            }
        }

        // All exercises
        private ObservableCollection<Exercise> _exercisesForGivenWorkoutTemplate;
        public ObservableCollection<Exercise> ExercisesForGivenWorkoutTemplate
        {
            get { return _exercisesForGivenWorkoutTemplate; }
            set
            {
                _exercisesForGivenWorkoutTemplate = value;
                NotifyPropertyChanged("ExercisesForGivenWorkoutTemplate");
            }
        }

        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all workouts in the database.
            var workoutsInDB = from Workout workout in workoutDB.Workout
                               select workout;

            // Query the database and load all workouts
            AllWorkouts = new ObservableCollection<Workout>(workoutsInDB);

            // Specify the query for all workout templates in the database.
            var workoutTemplatesInDB = from WorkoutTemplate workoutTemplate in workoutDB.WorkoutTemplate
                                       select workoutTemplate;

            AllWorkoutTemplates = new ObservableCollection<WorkoutTemplate>(workoutTemplatesInDB);

            // Specify the query for all exercises in the database.
            var exercisesInDB = from Exercise exercise in workoutDB.Exercise
                               select exercise;

            foreach (Exercise exercise in exercisesInDB)
            {
                var test = exercise.Name;
            }
            // Query the database and load all exercises.
            AllExercises = new ObservableCollection<Exercise>(exercisesInDB);

            ExercisesForNewTemplate = new ObservableCollection<Exercise>();
            ExercisesForGivenWorkoutTemplate = new ObservableCollection<Exercise>();

        }

        public void AddExerciseToTemplate(Exercise newExercise)
        {
            // Add a workout to the "all" observable collection.
            ExercisesForNewTemplate.Add(newExercise);
        }

        public void DeleteExerciseFromTemplate(Exercise newExercise)
        {
            // Add a workout to the "all" observable collection.
            ExercisesForNewTemplate.Remove(newExercise);
        }

        /// <summary>
        /// Submits a new workout template that has been created by the user
        /// to the database.
        /// </summary>
        /// <param name="name">the name of the workout template</param>
        public void SubmitNewWorkoutTemplate(string name)
        {
            //add new workout template to database
            WorkoutTemplate newWorkoutTemplate = new WorkoutTemplate { Name = name, ListOfExercises = serializeListOfExercises() };
            workoutDB.WorkoutTemplate.InsertOnSubmit(newWorkoutTemplate);

            // Save changes to the database.
            workoutDB.SubmitChanges();

            //add new workout template to the "all" observable collection
            AllWorkoutTemplates.Add(newWorkoutTemplate);
        }

        private string serializeListOfExercises()
        {
            //create comma seperated string of exercise ids associated with workout template
            StringBuilder sb = new StringBuilder();
            foreach (Exercise exercise in ExercisesForNewTemplate)
            {
                sb.Append(exercise.ID + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// Adds all the exercises associated with a given workout template ID to the ExercisesForGivenWorkoutTemplate
        /// collection
        /// </summary>
        /// <param name="givenWorkoutTemplateID"></param>
        public WorkoutTemplate GetExercisesFromWorkoutTemplate(int givenWorkoutTemplateID)
        {
            //get rid of any old exercises in the collection
            if (ExercisesForGivenWorkoutTemplate != null)
            {
                ExercisesForGivenWorkoutTemplate.Clear();
            }

            // Ger the workout template for the given ID
            var workoutTemplateInDB = from WorkoutTemplate workoutTemplate in workoutDB.WorkoutTemplate
                                      where workoutTemplate.ID == givenWorkoutTemplateID
                                      select workoutTemplate;

            //create an array of exercise IDs from the Workout tempaltes list of exercises
            string[] ListOfExercisesIDs = workoutTemplateInDB.First().ListOfExercises.Split(',');

            //for each exercise ID find that exercise in the database and add it to the 
            //ExercisesForGivenWorkoutTemplate collection.
            foreach (string exerciseID in ListOfExercisesIDs)
            {
                int intExerciseID = Convert.ToInt32(exerciseID);
                var exerciseInDB = from Exercise exercise in workoutDB.Exercise
                                   where exercise.ID == intExerciseID
                                   select exercise;
                ExercisesForGivenWorkoutTemplate.Add(exerciseInDB.First());
            }

            return workoutTemplateInDB.First();
        }

        // Add a workout to the database and collections.
        public void AddWorkout(Workout newWorkout)
        {
            // Add a workout to the data context.
            workoutDB.Workout.InsertOnSubmit(newWorkout);

            // Save changes to the database.
            workoutDB.SubmitChanges();

            // Add a workout to the "all" observable collection.
            AllWorkouts.Add(newWorkout);
        }

        // Remove a workout from the database and collections.
        public void DeleteWorkout(Workout workoutForDelete)
        {

            // Remove the workout from the "all" observable collection.
            AllWorkouts.Remove(workoutForDelete);

            // Remove the workout from the data context.
            workoutDB.Workout.DeleteOnSubmit(workoutForDelete);

            
            // Save changes to the database.
            workoutDB.SubmitChanges();
        }

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            workoutDB.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
