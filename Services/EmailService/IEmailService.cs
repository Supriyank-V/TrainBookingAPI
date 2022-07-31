namespace TrainBookingAPI.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(Email request);
        Task SendEmailAsync(Email request);
    }
}
