--param query

--SELECT Id, FirstName, LastName, SlackHandle, CohortId FROM Student
--                                        WHERE 1=1 AND LastName = 'Jones';

 SELECT c.Id AS CohortId, c.Name AS CohortName, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Name, e.Language, i.Id AS InstructorId, i.[FirstName] as InstructorFirst, i.LastName as InstructorLast FROM Student s
LEFT JOIN Instructor i ON s.CohortId = i.CohortId
LEFT JOIN StudentExercise se ON se.StudentId = s.Id
LEFT JOIN Exercise e ON e.Id = se.ExerciseId
LEFT JOIN Cohort c ON s.CohortId = c.Id

WHERE 1=1
GROUP BY c.Id, c.Name, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Name, e.Language, i.Id, i.FirstName, i.LastName;

