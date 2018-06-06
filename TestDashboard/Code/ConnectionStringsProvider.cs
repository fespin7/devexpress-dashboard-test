using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Web;
using System.Configuration;

namespace TestDashboard
{
    public class ConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        Dictionary<string, string> connectionStrings;

        public ConnectionStringsProvider()
        {
            foreach (ConnectionStringSettings connectionStringSettings in ConfigurationManager.ConnectionStrings)
            {
                connectionStrings = new Dictionary<string, string>();
                connectionStrings.Add("DASHBOARDS_CR", ConfigurationManager.ConnectionStrings["DASHBOARDS_CR"].ConnectionString);
            }
        }

        Dictionary<string, string> IDataSourceWizardConnectionStringsProvider.GetConnectionDescriptions()
        {
            return connectionStrings.ToDictionary(k => k.Key, k => k.Key);
        }
        DataConnectionParametersBase IDataSourceWizardConnectionStringsProvider.GetDataConnectionParameters(string name)
        {
            return new CustomStringConnectionParameters(connectionStrings[name]);
        }
    }
}