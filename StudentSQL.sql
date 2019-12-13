SELECT d.Id, d.DeptName, e.FirstName, e.LastName, e.DepartmentId
FROM Department d
LEFT JOIN Employee e ON d.Id = e.DepartmentId