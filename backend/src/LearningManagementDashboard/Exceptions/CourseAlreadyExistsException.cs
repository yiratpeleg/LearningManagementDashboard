namespace LearningManagementDashboard.Exceptions;

public class CourseAlreadyExistsException : Exception
{
    public CourseAlreadyExistsException(string name)
        : base($"A course with name '{name}' already exists.")
    { }
}
