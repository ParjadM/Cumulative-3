<<<<<<< HEAD
C# Cumulative Part 3

Extended From Cumulative part 1,2

=======
C# Cumulative Part 3 

This ASP.NET Core MVC web application implements the READ functionality for managing teacher/course/student data in the school database that was provided by the teacher. It provides API endpoints for retrieving teacher information and web pages for displaying teacher list and individual teacher details, and does the same for course and students. Important feature includes API endpoints at /api/teacher and /api/teacher/{id}, web page at /TeacherPage/List and /TeacherPage/Show/{id}, search functionality on the teacher list page, Entity Framework for Data Access and swagger for API documents. I'll be focusing on teacher API however the same is done with student and course API as well


to set up and run the application, you need visual studio code 2022 or similar IDE. the GITHUB clone repository is here "https://github.com/ParjadM/HTTP5125.git". Clone the repository using "https://github.com/ParjadM/HTTP5125.git", navigate to the project director with "HTTP5125/C1/Cumulative1/Cumulative1" and restore NuGet Packages using "dotnet restore". update the database connection string in program.cs and apply the necessary changes. build and run the application using "dotnet run" and access it. API documentation is available at /swagger. Testing details are in separate PDF document provided to the teacher. 


added and delete functionality for student, teacher, and course. extending this project to C# Cumulative Part 2 adding new features and functionality to work with. this feature works with deleting by ID and adding new features using information given on the input, confirm page to make sure it pops up correctly.


<<<<<<< HEAD
Author: Parjad Minooei
=======
-- Cumulative 3 --
added update(edit) functionality plus mandatory requirement for Cumulative 3 
(2 Marks) (Client) Error Handling on Update when the Teacher Name is empty
(2 Marks) (Server) Error Handling on Update when the Teacher Hire Date is in the future
(2 Marks) Update Functionality Students
(2 Marks) Update Functionality Courses

Author: Parjad Minooei

>>>>>>> c3a2a7c (Fnal product)
