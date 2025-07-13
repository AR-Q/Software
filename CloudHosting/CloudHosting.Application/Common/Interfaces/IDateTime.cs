namespace CloudHosting.Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }
        DateOnly Today { get; }
    }
}