using Microsoft.Data.SqlClient;
using System.Data;
using MvcCoreLinqToSql.Models;

namespace MvcCoreLinqToSql.Repositories
{
    public class RepositoryEnfermos
    {
        public DataTable tablaEnfermos;
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryEnfermos()
        {
            string connectionString = @"Data Source=LOCALHOST\DESARROLLO;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            string sql = "SELECT * FROM ENFERMO";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
            this.tablaEnfermos = new DataTable();
            adapter.Fill(this.tablaEnfermos);
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        public List<Enfermo> GetEnfermos() {
            var consulta = from datos in this.tablaEnfermos.AsEnumerable() select datos;
            
            if (consulta.Count() == 0)
            {
                return null;
            }

            List<Enfermo> enfermos = new List<Enfermo>();

            foreach (var row in consulta) { 
                Enfermo enf = new Enfermo();
                enf.Inscripcion = row.Field<string>("INSCRIPCION");
                enf.Apellido = row.Field<string>("APELLIDO");
                enf.Direccion = row.Field<string>("DIRECCION");
                enf.FechaNacimiento = row.Field<DateTime>("FECHA_NAC");
                //! a lo mejor tengo que cambiarlo por date o datetime
                enf.SexoRegistral = row.Field<string>("S");
                enf.NumSegSocial = row.Field<string>("NSS");
                enfermos.Add(enf);
            }

            return enfermos;
        }

        public Enfermo GetDetalleEnfermo(string inscripcion)
        {
            var consulta = from datos in this.tablaEnfermos.AsEnumerable()
                           where datos.Field<string>("INSCRIPCION") == inscripcion 
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }

            var row = consulta.First();
            Enfermo enf = new Enfermo();
            enf.Inscripcion = row.Field<string>("INSCRIPCION");
            enf.Apellido = row.Field<string>("APELLIDO");
            enf.Direccion = row.Field<string>("DIRECCION");
            enf.FechaNacimiento = row.Field<DateTime>("FECHA_NAC");
            enf.SexoRegistral = row.Field<string>("S");
            enf.NumSegSocial = row.Field<string>("NSS");
            return enf;
        }

        public async Task DeleteEnfermoAsync(string inscripcion) 
        {
            string sql = "delete from enfermo where inscripcion=@inscripcion";
            this.com.Parameters.AddWithValue("@inscripcion", inscripcion);
            this.com.CommandType = System.Data.CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }
    }
}
