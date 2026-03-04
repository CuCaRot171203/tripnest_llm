using APPLICATION.Embedding;
using APPLICATION.Interfaces.Admin;
using APPLICATION.Interfaces.Auth;
using APPLICATION.Interfaces.Booking;
using APPLICATION.Interfaces.Companies;
using APPLICATION.Interfaces.Embedding;
using APPLICATION.Interfaces.Itinerary;
using APPLICATION.Interfaces.Message;
using APPLICATION.Interfaces.Notification;
using APPLICATION.Interfaces.Payment;
using APPLICATION.Interfaces.Review;
using APPLICATION.Interfaces.Roles;
using APPLICATION.Interfaces.Rooms;
using APPLICATION.Interfaces.Search;
using APPLICATION.Interfaces.Upload;
using APPLICATION.Interfaces.Users;
using APPLICATION.Interfaces.Webhook;
using APPLICATION.Interfaces.Worker;
using APPLICATION.Services.Admin;
using APPLICATION.Services.Auth;
using APPLICATION.Services.Booking;
using APPLICATION.Services.Companies;
using APPLICATION.Services.Embedding;
using APPLICATION.Services.Itinerary;
using APPLICATION.Services.Message;
using APPLICATION.Services.Notification;
using APPLICATION.Services.Review;
using APPLICATION.Services.Roles;
using APPLICATION.Services.Rooms;
using APPLICATION.Services.S3FileStorage;
using APPLICATION.Services.Search;
using APPLICATION.Services.Upload;
using APPLICATION.Services.Users;
using APPLICATION.Services.Webhook;
using APPLICATION.Services.Worker;
using INFRASTRUCTURE.Interfaces.Admin;
using INFRASTRUCTURE.Interfaces.Auth;
using INFRASTRUCTURE.Interfaces.Bookings;
using INFRASTRUCTURE.Interfaces.Companies;
using INFRASTRUCTURE.Interfaces.IItinerary;
using INFRASTRUCTURE.Interfaces.Message;
using INFRASTRUCTURE.Interfaces.Notification;
using INFRASTRUCTURE.Interfaces.Reviews;
using INFRASTRUCTURE.Interfaces.Rooms;
using INFRASTRUCTURE.Interfaces.Search;
using INFRASTRUCTURE.Interfaces.Upload;
using INFRASTRUCTURE.Interfaces.Users;
using INFRASTRUCTURE.Interfaces.Webhook;
using INFRASTRUCTURE.Interfaces.Worker;
using INFRASTRUCTURE.Repositories.Admin;
using INFRASTRUCTURE.Repositories.Auth;
using INFRASTRUCTURE.Repositories.Booking;
using INFRASTRUCTURE.Repositories.Company;
using INFRASTRUCTURE.Repositories.Itinerary;
using INFRASTRUCTURE.Repositories.Message;
using INFRASTRUCTURE.Repositories.Notification;
using INFRASTRUCTURE.Repositories.Payment;
using INFRASTRUCTURE.Repositories.Review;
using INFRASTRUCTURE.Repositories.Role;
using INFRASTRUCTURE.Repositories.Rooms;
using INFRASTRUCTURE.Repositories.Search;
using INFRASTRUCTURE.Repositories.Upload;
using INFRASTRUCTURE.Repositories.User;
using INFRASTRUCTURE.Repositories.Webhook;

namespace API.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddAppAndReposervice(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Service
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IRoomsService, RoomsService>();
            services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IPaymentsService, PaymentsService>();
            //services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IItineraryService, ItineraryService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAdminService, AdminService>();
            //services.AddScoped<IUploadService, UploadService>();
            //services.AddScoped<IStorageService, S3FileStorageService>();
            services.AddScoped<IEmbeddingsService, EmbeddingsService>();
            services.AddScoped<IPaymentWebhookService, PaymentWebhookService>();
            services.AddScoped<IMapsCostService, MapsCostService>();
            //services.AddScoped<IWorkerService, WorkerService>();
            services.AddScoped<ISearchService, SearchService>();

            // Repository
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Users.IUsersRepository,
                INFRASTRUCTURE.Repositories.User.UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IRoomsRepository, RoomsRepository>();
            services.AddScoped<IRoomAvailabilityRepository, RoomAvailabilityRepository>();
            services.AddScoped<IRoomPricesRepository, RoomPricesRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            //services.AddScoped<INFRASTRUCTURE.Interfaces.Payment.IPaymentsRepository, 
            //    INFRASTRUCTURE.Repositories.Payment.PaymentsRepository>();
            services.AddSingleton<IEmbeddingProvider, OpenAiEmbeddingProvider>();
            services.AddScoped<IReviewsRepository, ReviewsRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IItineraryRepository, ItineraryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Admin.IEmbeddingsRepository,
                INFRASTRUCTURE.Repositories.Admin.EmbeddingsRepository>();
            services.AddScoped<IUploadRepository, UploadRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Embedding.IEmbeddingsRepository, 
                INFRASTRUCTURE.Repositories.Embedding.EmbeddingsRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Webhook.IPaymentsRepository,
                INFRASTRUCTURE.Repositories.Webhook.PaymentsRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Webhook.IBookingsRepository,
                INFRASTRUCTURE.Repositories.Webhook.BookingsRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Worker.IPropertiesRepository,
                INFRASTRUCTURE.Repositories.Worker.PropertiesRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Worker.IEmbeddingsRepository,
                INFRASTRUCTURE.Repositories.Worker.EmbeddingsRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Worker.IBookingsRepository,
                INFRASTRUCTURE.Repositories.Worker.BookingsRepository>();
            services.AddScoped<INFRASTRUCTURE.Interfaces.Worker.INotificationsRepository,
                INFRASTRUCTURE.Repositories.Worker.NotificationsRepository>();
            services.AddScoped<ISearchRepository, SearchRepository>();

            return services;
        }
    }
}
