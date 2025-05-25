namespace LearningManagementDashboard.Exceptions;

public class EnrolmentAlreadyExistsException : Exception
{
    public EnrolmentAlreadyExistsException(Guid courseId, Guid studentId)
        : base($"Enrolment already exists for Course '{courseId}', Student '{studentId}'")
    { }
}
