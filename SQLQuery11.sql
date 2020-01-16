 SELECT 
                                        d.[Name],
                                        d.Budget,
                                        COUNT(e.DepartmentId) AS EmployeeCount
                                        FROM Department d 
LEFT JOIN Employee e ON e.DepartmentId = d.Id
                                GROUP BY d.[Name], d.Budget
                              
                                ORDER BY COUNT(e.DepartmentId)