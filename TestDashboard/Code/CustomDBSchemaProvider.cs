using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using System.Collections.Specialized;
using System.Web.Routing;
using System.Linq;


namespace TestDashboard
{
    public class CustomDBSchemaProvider : IDBSchemaProviderEx
    {
        DBSchemaProviderEx provider;
        public CustomDBSchemaProvider()
        {
            this.provider = new DBSchemaProviderEx();
        }

        public DBTable[] GetTables(SqlDataConnection connection, params string[] tableList)
        {

            return provider.GetTables(connection, tableList)
                .Where(table => table.Name.Equals("_PersonaJuridica"))
                .ToArray();
        }

        public DBTable[] GetViews(SqlDataConnection connection, params string[] viewList)
        {
            DBTable[] views = new DBTable[0];
            return views;
        }

        public DBStoredProcedure[] GetProcedures(SqlDataConnection connection, params string[] procedureList)
        {
            DBStoredProcedure[] storedProcedures = new DBStoredProcedure[0];
            return storedProcedures;
        }

        public void LoadColumns(SqlDataConnection connection, params DBTable[] tables)
        {
            provider.LoadColumns(connection, tables);
        }


    }
}