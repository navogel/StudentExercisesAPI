using System.Collections.Generic;


namespace StudentExercisesAPI.Models
{
    public class Cohort
    {

        //constructor-bot
        //public Cohort(string name)
        //{
        //    Name = name;
        //}

        //properties
        public string Name { get; set; }

        public int Id { get; set; }


        public List<Student> StudentsInCohort { get; set; }
        public List<Instructor> InstructorsInCohort { get; set; }

        //public Student AddNewStudent(string first, string last, string slack) 
        //{
        //var student = new Student(first, last, slack);
        //StudentsInCohort.Add(student);
        //return student;

        //}

        //public Instructor AddNewInstructor(string first, string last, string slack, string specialty) 
        //{
        //var instructor = new Instructor(first, last, slack, specialty);
        //InstructorsInCohort.Add(instructor);
        //return instructor;
        //}

        //public void AssignExercise(Exercise exercise)
        //{
        //    foreach (var student in StudentsInCohort)
        //    {
        //        student.StudentsExercises.Add(exercise);
        //    }
        //}
    }

}