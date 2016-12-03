using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using DevExpress.XtraEditors;

namespace SvApp
{
    public class SrvcApp : Iadm, IDisposable
    {
        
        SqlConnection conn;
        SqlCommand comm;
        SqlConnectionStringBuilder connStringBuilder;

        public void Dispose()
        {
            if (conn != null)
            {
                conn.Dispose();
                conn = null;
            }
            if (comm != null)
            {
                comm.Dispose();
                comm = null;
            }
        }

        private SrvcApp()
        {
            db_connect();
        }

        private void db_connect()
        {
            connStringBuilder = new SqlConnectionStringBuilder()
            {
               // DataSource = "DESKTOP-0SI1HN9",
                DataSource = "DESKTOP-1172569",
                InitialCatalog = "labdb",
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

        public string GetLogin(akun data)
        {
            string role = String.Empty;
            try
            {
                comm.CommandText = "SELECT status "+
                                   "FROM users "+
                                   "WHERE username = @username AND password = @password";
                comm.Parameters.AddWithValue("username", data.Username);
                comm.Parameters.AddWithValue("password", data.Password);
                comm.CommandType = CommandType.Text;

                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    role = Convert.ToString(reader[0]);
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
                                   "FROM jurusan ";
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
                                   "FROM angkatan ";
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
                comm.CommandText = "SELECT praktikan.nrp, " +
                                          "praktikan.nama, " +
                                          "praktikan.foto, " +
                                          "angkatan.tahun_angkatan, " +
                                          "jurusan.nama_jurusan " +
                                    "FROM angkatan " +
                                    "INNER JOIN praktikan ON angkatan.kode_angkatan = praktikan.kode_angkatan " +
                                    "INNER JOIN jurusan ON praktikan.kode_jurusan = jurusan.kode_jurusan " +
                                    "WHERE praktikan.kode_angkatan = @kode_angkatan AND praktikan.kode_jurusan =@kode_jurusan";
                comm.Parameters.AddWithValue("kode_jurusan", data.KodeJurusan);
                comm.Parameters.AddWithValue("kode_angkatan", data.KodeAngkatan);
                comm.CommandType = CommandType.Text;
                conn.Open();
                
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    praktikan user = new praktikan()
                    {
                        NRP = Convert.ToString(reader[0]).TrimEnd(),
                        Nama = Convert.ToString(reader[1]).TrimEnd(),
                        Foto = File.ReadAllBytes(Convert.ToString(reader[2])),
                        KodeAngkatan = Convert.ToString(reader[3]).TrimEnd(),
                        KodeJurusan = Convert.ToString(reader[4]).TrimEnd()
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

        public int InsertMultiplePraktikan(List<praktikan> data)
        {
            const string dir = @"D:\LIK\USER\";
            const string format = @".png";
            string[] loc = new string[data.Count];
            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    string location = string.Format(@"{0}{1}\{2}{3}", dir, data[i].NRP, data[i].Nama, format);
                    string foo = string.Format("{0}/{1}/", dir, data[i].NRP);
                    //cek folder user di server
                    if (Directory.Exists(foo)) //kalau folder sudah ada
                    {
                        File.WriteAllBytes(@foo + data[i].Nama + format, data[i].Foto); // simpan foto ke folder
                    }
                    else
                    {
                        loc[i] = @foo + data[i].Nama;
                        DirectoryInfo di = Directory.CreateDirectory(foo); // membuat direkt0ri
                        File.WriteAllBytes(@foo + data[i].Nama + format, data[i].Foto); // simpan foto ke folder
                    }
                }

                string[] Akun = new string[data.Count];
                string[] Praktikan = new string[data.Count];
                for (int x = 0; x < data.Count; x++)
                {
                    Akun[x] = String.Format("('{0}', '{1}', 'Praktikan')", data[x].NRP, data[x].NRP);
                    Praktikan[x] = string.Format("('{0}', '{1}', '{2}', '{3}', '{4}')", data[x].NRP, data[x].Nama, data[x].KodeJurusan, data[x].KodeAngkatan, String.Format("{0}{1}\\{2}{3}",dir, data[x].NRP, data[x].Nama, format));
                }
                string _Akun = String.Join(",", Akun);
                string _Praktikan = String.Join(",", Praktikan);
                Array.Clear(Akun, 0, Akun.Length);
                Array.Clear(Praktikan, 0, Praktikan.Length);

                string query = string.Format("INSERT INTO users (Username, Password, status) VALUES {0} " +
                                             "INSERT INTO praktikan (NRP, Nama, kode_jurusan, kode_angkatan, Foto) VALUES {1}", 
                                             _Akun, 
                                             _Praktikan);
                comm.CommandText = query;
                comm.CommandType = CommandType.Text;
                conn.Open();
                return comm.ExecuteNonQuery();
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
