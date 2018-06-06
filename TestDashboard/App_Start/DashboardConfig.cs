using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Hosting;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native.DashboardRestfulService;
using DevExpress.DataAccess.Sql;

namespace TestDashboard
{
    public class DashboardConfig
    {
        public static void RegisterService(RouteCollection routes)
        {
            //routes.MapDashboardRoute();

            routes.MapDashboardRoute("Tablero", "Tablero", new string[] { "TestDashboard" });

            //Revisar en caso que este dentro de un área el controlador
            //routes.MapDashboardRoute("DashboardDesigner", "DashboardDesigner", new string[] { "RDS.Areas.CR.Controllers.Reportes" }, "CR");

            //Definir donde se guardan los dashboard
            //DashboardConfigurator.Default.SetDashboardStorage(new DashboardFileStorage(@"~/App_Data/Dashboards"));
            //DataBaseEditableDashboardStorage dataBaseDashboardStorage = new DataBaseEditableDashboardStorage(connectionString, moduloID, sesionUsuarioID);
            //DashboardConfigurator.Default.SetDashboardStorage(dataBaseDashboardStorage);

            //Definir un datasource que se cargue por defecto en el dashboard
            //DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            //dataSourceStorage.RegisterDataSource(DataSourceGenerator.CreateDefaultDataSource().SaveToXml());
            //DashboardConfigurator.Default.SetDataSourceStorage(dataSourceStorage);

            //Estabecer datos de conexión a la base de datos
            //DashboardConfigurator.Default.SetConnectionStringsProvider(new ConnectionStringsProvider());

        }
    }


    public class CustomControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            
            if(controllerName.Equals("Tablero"))
            {
                short sesionUsuarioID = 1;
                byte moduloID = 5;
                string connectionString = ConfigurationManager.ConnectionStrings["DBConexion"].ConnectionString;
                bool editable = true;

                DashboardConfigurator dashboardConfigurator = new DashboardConfigurator();

                //Definir donde se guardan los dashboard
                //dashboardConfigurator.SetDashboardStorage(new DashboardFileStorage(@"~/App_Data/Dashboards"));
                DataBaseEditableDashboardStorage dataBaseDashboardStorage = new DataBaseEditableDashboardStorage(connectionString, moduloID, sesionUsuarioID, editable);
                dashboardConfigurator.SetDashboardStorage(dataBaseDashboardStorage);

                //Definir un datasource que se cargue por defecto en el dashboard
                DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
                dataSourceStorage.RegisterDataSource(DataSourceGenerator.CreateDefaultDataSource(moduloID).SaveToXml());
                dashboardConfigurator.SetDataSourceStorage(dataSourceStorage);

                //Estabecer datos de conexión a la base de datos
                //DashboardConfigurator.Default.SetConnectionStringsProvider(new ConnectionStringsProvider());

                //Establecer esquema para mostrar solo las tablas o vistas que se requiera
                dashboardConfigurator.SetDBSchemaProvider(new CustomDBSchemaProvider());

                dashboardConfigurator.ConfigureDataConnection += (s, e) =>
                {
                    if (e.DataSourceName == "DataSource")
                    {
                        string dsConnectionString = ConfigurationManager.ConnectionStrings["DASHBOARDS_CR"].ConnectionString;
                        e.ConnectionParameters = new CustomStringConnectionParameters(dsConnectionString);
                    }
                };
                return new DashboardController(dashboardConfigurator);
            }
            else
            {
                return base.CreateController(requestContext, controllerName);
            }
        }

        public override void ReleaseController(IController controller)
        {
            IDisposable dispose = controller as IDisposable;
            if (dispose != null)
            {
                dispose.Dispose();
            }
        }
    }
}