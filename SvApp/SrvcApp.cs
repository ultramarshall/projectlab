using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace SvApp
{
    public class SrvcApp : Iadm
    {
        
        SqlConnection conn;
        SqlCommand comm;
        SqlConnectionStringBuilder connStringBuilder;

        private SrvcApp()
        {
            db_connect();
        }

        private void db_connect()
        {
            connStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = "DESKTOP-0SI1HN9",
                InitialCatalog = "LabKomputerDB",
                Encrypt = true,
                TrustServerCertificate = true,
                ConnectTimeout = 30,
                AsynchronousProcessing = true,
                MultipleActiveResultSets = true,
                IntegratedSecurity = true,
            };
            conn = new SqlConnection(connStringBuilder.ToString());
            comm = conn.CreateCommand();
        }

        public int GetLogin(akun data)
        {
            int role = 0;
            try
            {
                comm.CommandText = "SELECT PeranID "+
                                   "FROM TAkun "+
                                   "WHERE Username = @username AND Password = @password";
                comm.Parameters.AddWithValue("username", data.Username);
                comm.Parameters.AddWithValue("password", data.Password);
                comm.CommandType = CommandType.Text;

                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    role = Convert.ToInt32(reader[0]);
                }
                return role;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public List<jurusan> GetJurusan()
        {
            try
            {
                List<jurusan> jurusan = new List<jurusan>();
                comm.CommandText = "SELECT * " +
                                   "FROM TJurusan ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    jurusan list = new jurusan()
                    {
                        KodeJurusan = Convert.ToString(reader[0]).TrimEnd(),
                        NamaJurusan = Convert.ToString(reader[1]).TrimEnd()
                    };
                    jurusan.Add(list);
                }
                return jurusan;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public List<angkatan> GetAngkatan()
        {
            try
            {
                List<angkatan> angkatan = new List<angkatan>();
                comm.CommandText = "SELECT * " +
                                   "FROM TTahunAngkatan ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    angkatan list = new angkatan()
                    {
                        KodeAngkatan = Convert.ToString(reader[0]).TrimEnd(),
                        TahunAngkatan = Convert.ToString(reader[1]).TrimEnd()
                    };
                    angkatan.Add(list);
                }
                return angkatan;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public List<praktikan> GetPraktikan(praktikan data)
        {
            try
            {
                List<praktikan> users = new List<praktikan>();
                comm.CommandText = "SELECT * " +
                                   "FROM TPraktikan " +
                                   "WHERE KodeJurusan=@JurusanID AND KodeAngkatan=@AngkatanID";
                comm.Parameters.AddWithValue("KodeJurusan", data.KodeJurusan);
                comm.Parameters.AddWithValue("KodeAngkatan", data.KodeAngkatan);
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    praktikan user = new praktikan()
                    {
                        NRP = Convert.ToString(reader[0]).TrimEnd(),
                        Nama = Convert.ToString(reader[1]).TrimEnd(),
                        KodeJurusan = Convert.ToString(reader[2]).TrimEnd(),
                        KodeAngkatan = Convert.ToString(reader[3]).TrimEnd(),
                        Foto = File.ReadAllBytes(Convert.ToString(reader[4])),
                        Notes = Convert.ToString(reader[5]).TrimEnd()
                    };
                    users.Add(user);
                }
                return users;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
