using Swift.Extensibility.Services.Profile;

namespace Swift.Extensibility.Internal
{
    public interface ILoginDialog
    {
        UserProfile UserProfile { get; }

        LoginResult ShowLoginDialog();
    }

    public enum LoginResult
    {
        Successful,
        Failed,
        Aborted
    }
}
