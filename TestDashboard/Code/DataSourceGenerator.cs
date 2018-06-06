using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;

namespace TestDashboard
{
    public static class DataSourceGenerator
    {

        public static DashboardSqlDataSource CreateDefaultDataSource(byte moduloID)
        {

            DashboardSqlDataSource dashboardSqlDataSource1 = new DashboardSqlDataSource("DataSource", "DASHBOARDS_CR");
            dashboardSqlDataSource1.DataProcessingMode = DataProcessingMode.Client;
            dashboardSqlDataSource1.ComponentName = "dashboardSqlDataSource1";

            SelectQuery qPersonaJuridica = SelectQueryFluentBuilder
                .AddTable("_PersonaJuridica")
                .SelectAllColumns()
                .Build("PersonaJuridica");

            dashboardSqlDataSource1.Queries.Add(qPersonaJuridica);


            return dashboardSqlDataSource1;
        }

    }
}