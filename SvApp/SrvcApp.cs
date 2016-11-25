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
                DataSource = "DESKTOP-1172569",
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
                comm.CommandText = "SELECT Jurusan " +
                                   "FROM TJurusan ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    jurusan list = new jurusan()
                    {
                        Jurusan = Convert.ToString(reader[0]).TrimEnd()
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
                comm.CommandText = "SELECT Angkatan " +
                                   "FROM TAngkatan ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    angkatan list = new angkatan()
                    {
                        Angkatan = Convert.ToString(reader[0]).TrimEnd()
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
                                   "WHERE JurusanID=@JurusanID AND AngkatanID=@AngkatanID";
                comm.Parameters.AddWithValue("JurusanID", data.JurusanID);
                comm.Parameters.AddWithValue("AngkatanID", data.AngkatanID);
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    praktikan user = new praktikan()
                    {
                        NRP = Convert.ToString(reader[0]),
                        Nama = Convert.ToString(reader[1]),
                        JurusanID = Convert.ToInt32(reader[2]),
                        AngkatanID = Convert.ToInt32(reader[3]),
                        Foto = File.ReadAllBytes(Convert.ToString(reader[4])),
                        Notes = Convert.ToString(reader[5]),
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
