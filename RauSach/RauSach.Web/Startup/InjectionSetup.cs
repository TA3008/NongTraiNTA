using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Core.Repositories;
using RauSach.Core.Services;
using RauSach.Database.Repositories;
using RauSach.Infrastructure.File;
using RauSach.Application.Services;
using RauSach.Database;
using RauSach.Infrastructure.Mail;
using RauSach.Web.Helpers;

namespace RauSach.Web.Startup
{
    public static class InjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailTemplate, EmailTemplate>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<ISystemParameters, SystemParameters>();
            services.AddTransient<IDeliveryService, DeliveryService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IFileService, StorageAccount>();
            services.AddTransient<IUserGroupManager, UserGroupManager>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IUserGroupRepository, UserGroupRepository>();
            services.AddTransient<IGardenRepository, GardenRepository>();
            services.AddTransient<IVegetableRepository, VegetableRepository>();
            services.AddTransient<IVegetableComboRepository, VegetableComboRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IDeliveryRepository, DeliveryRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<IVoucherRepository, VoucherRepository>();
            services.AddTransient<IGeneralItemRepository, GeneralItemRepository>();

            services.RegisterJobAsync().GetAwaiter().GetResult();

            return services;
        }
    }
}
