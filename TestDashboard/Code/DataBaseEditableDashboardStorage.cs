using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardWeb;
using System.Web.Mvc;

namespace TestDashboard
{
    public class DataBaseEditableDashboardStorage : IEditableDashboardStorage, IDashboardStorage
    {
        string connectionString;
        byte moduloId;
        short usuarioID;
        bool editable;

        public DataBaseEditableDashboardStorage(string connectionString, byte moduloId, short usuarioID, bool editable)
            : base()
        {
            this.connectionString = connectionString;
            this.moduloId = moduloId;
            this.usuarioID = usuarioID;
            this.editable = editable;
        }

        string IEditableDashboardStorage.AddDashboard(XDocument document, string dashboardName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                SqlCommand InsertCommand = new SqlCommand(
                    "INSERT INTO [dbo].[Dashboard]([ModuloID],[DirectorioID],[Codigo],[Nombre],[DashboardStream],[Ingreso],[Estado],[UsuarioID]) " +
                    "output INSERTED.DashboardID " +
                    "VALUES (@ModuloID,@DirectorioID,@Codigo,@Nombre,@DashboardStream,getdate(),'V',@UsuarioID)");
                InsertCommand.Parameters.Add("ModuloID", SqlDbType.TinyInt).Value = moduloId;
                InsertCommand.Parameters.Add("DirectorioID", SqlDbType.Int).Value = 3;
                //InsertCommand.Parameters.Add("DirectorioID", SqlDbType.Int).Value = 2;
                InsertCommand.Parameters.Add("Codigo", SqlDbType.VarChar).Value = "";
                InsertCommand.Parameters.Add("Nombre", SqlDbType.VarChar).Value = dashboardName;
                InsertCommand.Parameters.Add("DashboardStream", SqlDbType.VarBinary).Value = stream.ToArray();
                InsertCommand.Parameters.Add("UsuarioID", SqlDbType.SmallInt).Value = usuarioID;
                InsertCommand.Connection = connection;
                string ID = InsertCommand.ExecuteScalar().ToString();
                connection.Close();
                return ID;
            }
        }

        [HttpGet]
        XDocument IDashboardStorage.LoadDashboard(string dashboardID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand GetCommand = new SqlCommand("SELECT  DashboardStream FROM [dbo].[Dashboard] WHERE DashboardID=@ID");
                GetCommand.Parameters.Add("ID", SqlDbType.Int).Value = Convert.ToInt32(dashboardID);
                GetCommand.Connection = connection;
                SqlDataReader reader = GetCommand.ExecuteReader();
                reader.Read();
                byte[] data = reader.GetValue(0) as byte[];
                MemoryStream stream = new MemoryStream(data);
                connection.Close();
                return XDocument.Load(stream);
            }
        }

        IEnumerable<DashboardInfo> IDashboardStorage.GetAvailableDashboardsInfo()
        {

            List<DashboardInfo> list = new List<DashboardInfo>();

            string query = "SELECT DashboardID, Nombre FROM [dbo].[Dashboard] WHERE Estado = 'V' AND DirectorioID <> 2";

            if (!editable)
                query = "SELECT DashboardID, Nombre FROM [dbo].[Dashboard] WHERE Estado = 'V' AND DirectorioID = 2";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand GetCommand = new SqlCommand(query);
                GetCommand.Connection = connection;
                SqlDataReader reader = GetCommand.ExecuteReader();
                while (reader.Read())
                {
                    string ID = reader.GetInt32(0).ToString();
                    string Caption = reader.GetString(1);
                    list.Add(new DashboardInfo() { ID = ID, Name = Caption });
                }
                connection.Close();
            }
            return list;
        }

        void IDashboardStorage.SaveDashboard(string dashboardID, XDocument document)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                SqlCommand InsertCommand = new SqlCommand(
                    "UPDATE [dbo].[Dashboard] Set DashboardStream = @DashboardStream " +
                    "WHERE DashboardID = @ID");
                InsertCommand.Parameters.Add("ID", SqlDbType.Int).Value = Convert.ToInt32(dashboardID);
                InsertCommand.Parameters.Add("DashboardStream", SqlDbType.VarBinary).Value = stream.ToArray();
                InsertCommand.Connection = connection;
                InsertCommand.ExecuteNonQuery();

                connection.Close();
            }

        }

    }
}