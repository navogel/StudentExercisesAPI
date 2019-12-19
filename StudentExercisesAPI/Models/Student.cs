using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Student
    {

        //public Student(string first, string last, string slack)
        //{
        //    FirstName = first;
        //    LastName = last;
        //    SlackHandle = slack;


        //}
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "YO YOU GOT THE WRONG FIGS")]
        public string SlackHandle { get; set; }

        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]

        public int CohortId { get; set; }

        public Cohort Cohort {get; set;}

        //public Student()
        //{
        //    Cohort = new Cohort();

        //}


        //public Cohort StudentCohort { get; set; }



        //collection of excercises
        public List<Exercise> StudentsExercises = new List<Exercise>();

    }

}