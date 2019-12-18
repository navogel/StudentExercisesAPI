--param query

--SELECT Id, FirstName, LastName, SlackHandle, CohortId FROM Student
--                                        WHERE 1=1 AND LastName = 'Jones';

--full SQL search with everything
-- SELECT c.Id AS CohortId, c.Name AS CohortName, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Id AS ExerciseId, e.Name, e.Language, i.Id AS InstructorId, i.[FirstName] AS InstructorFirst, i.LastName AS InstructorLast, st.FirstName AS StudentFirst, st.LastName AS StudentLast FROM Student s
--LEFT JOIN Instructor i ON s.CohortId = i.CohortId
--LEFT JOIN StudentExercise se ON se.StudentId = s.Id
--LEFT JOIN Exercise e ON e.Id = se.ExerciseId
--LEFT JOIN Cohort c ON s.CohortId = c.Id
--LEFT JOIN Student st ON c.Id = st.CohortId

--WHERE 1=1


--GROUP BY c.Id, c.Name, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Name, e.Language, i.Id, i.FirstName, i.LastName;

--SELECT s.Id AS StudentId, s.FirstName, s.LastName, s.SlackHandle, c.Id AS CohortId, c.Name AS CohortName, e.Id AS ExerciseId, e.Name AS ExerciseName, e.Language FROM Student s 
--                                        INNER JOIN Cohort c ON s.CohortId = c.Id
--                                        LEFT JOIN StudentExercise se ON s.Id = se.StudentId
--                                        LEFT JOIN Exercise e ON e.Id = se.ExerciseId 

--broken down query
-- SELECT c.Id AS CohortId, c.Name AS CohortName, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Id AS ExerciseId, e.Name, e.Language FROM Student s
--LEFT JOIN StudentExercise se ON se.StudentId = s.Id
--LEFT JOIN Exercise e ON e.Id = se.ExerciseId
--LEFT JOIN Cohort c ON s.CohortId = c.Id


--WHERE 1=1;

--instructor query
SELECT c.Id AS CohortId, c.Name AS CohortName, i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Specialty FROM Instructor i
                                       LEFT JOIN Cohort c ON i.CohortId = c.Id

                                        WHERE 1=1

--exercise query
--SELECT s.id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, e.[Name], e.Id as ExerciseId, e.Language
--                            FROM Student s
--                            LEFT JOIN StudentExercise se ON s.Id = se.StudentId
--                            LEFT JOIN Exercise e ON se.ExerciseId = e.Id