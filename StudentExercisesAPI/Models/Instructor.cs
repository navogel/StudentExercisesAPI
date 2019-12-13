using System.Collections.Generic;

namespace StudentExercisesAPI.models

{
    public class Instructor : NSSPerson
    {

        public int CohortId {get; set;}
    
        public string Specialty { get; set; }
        public Cohort InstructorCohort { get; set; }

        public int Id { get; set; }

        

        

    }

}