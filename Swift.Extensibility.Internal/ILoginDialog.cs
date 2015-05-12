using Swift.Extensibility.Services;

namespace Swift.AppServices.Extensibility
{
    public interface ILoginDialog
    {
        IUserProfile UserProfile { get; }

        LoginResult ShowLoginDialog();
    }

    public enum LoginResult
    {
        Successful,
        Failed,
        Aborted
    }
}
