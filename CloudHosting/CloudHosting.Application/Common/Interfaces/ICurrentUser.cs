namespace CloudHosting.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        Guid? Id { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}