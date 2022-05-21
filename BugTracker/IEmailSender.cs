namespace BugTracker
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
