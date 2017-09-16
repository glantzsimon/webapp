using Autofac;
using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Database;
using K9.Base.DataAccessLayer.Helpers;
using K9.Base.DataAccessLayer.Respositories;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.DataSets;
using K9.Base.WebApplication.Helpers;
using K9.Base.WebApplication.Services;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Data.Entity;
using System.IO;

namespace K9.Base.WebApplication
{
    public static class Startup
    {
        public static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<Db>().As<DbContext>().InstancePerRequest();
            builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>().SingleInstance();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();
            builder.RegisterGeneric(typeof(DataTableAjaxHelper<>)).As(typeof(IDataTableAjaxHelper<>))
                .InstancePerRequest();
            builder.RegisterType<ColumnsConfig>().As<IColumnsConfig>().SingleInstance();
            builder.RegisterType<DataSetsHelper>().As<IDataSetsHelper>().InstancePerRequest();
            builder.RegisterType<DataSets.DataSets>().As<IDataSets>().SingleInstance();
            builder.RegisterType<Users>().As<IUsers>().InstancePerRequest();
            builder.RegisterType<Roles>().As<IRoles>().InstancePerRequest();
            builder.RegisterType<Mailer>().As<IMailer>().InstancePerRequest();
            builder.RegisterType<PostedFileHelper>().As<IPostedFileHelper>().InstancePerRequest();
            builder.RegisterType<FileSourceHelper>().As<IFileSourceHelper>().InstancePerRequest();
            builder.RegisterGeneric(typeof(ControllerPackage<>)).As(typeof(IControllerPackage<>)).InstancePerRequest();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerRequest();
        }

        public static void RegisterStaticTypes()
        {
            HtmlHelpers.SetIgnoreColumns(new ColumnsConfig());
        }

        public static void RegisterConfiguration(ContainerBuilder builder)
        {
            var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json"));

            builder.Register(c => ConfigHelper.GetConfiguration<SmtpConfiguration>(json)).SingleInstance();
            builder.Register(c => ConfigHelper.GetConfiguration<DatabaseConfiguration>(json)).SingleInstance();

            var websiteConfig = ConfigHelper.GetConfiguration<WebsiteConfiguration>(json);
            builder.Register(c => websiteConfig).SingleInstance();
            WebsiteConfiguration.Instance = websiteConfig.Value;
        }
    }
}
