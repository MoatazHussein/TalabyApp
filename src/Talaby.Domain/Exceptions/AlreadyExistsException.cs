namespace Talaby.Domain.Exceptions;

public class AlreadyExistsException(string resourceType)
    : Exception(resourceType + " Already Exists")
{
}
