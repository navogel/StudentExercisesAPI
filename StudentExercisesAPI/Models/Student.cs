using System.Collections.Generic;


namespace StudentExercisesAPI.models
{
    public class Student 
    {

        //public Student(string first, string last, string slack)
        //{
        //    FirstName = first;
        //    LastName = last;
        //    SlackHandle = slack;


        //}

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }

        public int Id { get; set; }

        public int CohortId { get; set; }



        //public Cohort StudentCohort { get; set; }



        collection of excercises
        public List<Exercise> StudentsExercises = new List<Exercise>();

    }

}