using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Lists.Constants;
using Lists.Models.Data;
using Microsoft.Owin;
using Owin;
using System.Configuration;

namespace Lists
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfire(config =>
            {
                config.UseSqlServerStorage(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString);
                config.UseServer();
                JobStorage.Current = new SqlServerStorage(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString);
                SetupRecurringJobs();

                config.UseAuthorizationFilters(
                    new ListsAuthorizationFilter());
            });
        }

        public void SetupRecurringJobs()
        {
            RecurringJob.AddOrUpdate(() => QueuedEmail.ProcessQueuedEmails(), Cron.Minutely);
            RecurringJob.AddOrUpdate(() => QuickAddGroceryItem.UpdateQuickAddItems(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => AverageGroceryPrice.UpdateAverageGroceryPrices(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => List.DeleteExpiredGuestLists(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => AmazonProductAdvertiser.UpdateSavedProducts(), Cron.Hourly);
            //RecurringJob.AddOrUpdate(() => IpAddressSalesTax.UpdateIpAddressSalesTax(), Cron.Hourly);
        }
    }
}