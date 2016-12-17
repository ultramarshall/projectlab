using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

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
                DataSource = "DESKTOP-0SI1HN9\\SQLSERVER",
                //DataSource = "DESKTOP-3RKCN07",
                InitialCatalog = "labdb",Encrypt = true,
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
                comm.Parameters.AddWithValue("username", data.Username.TrimEnd());
                comm.Parameters.AddWithValue("password", data.Password.TrimEnd());
                comm.CommandType = CommandType.Text;

                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    role = Convert.ToString(reader[0]).TrimEnd();
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

        public List<jadwalPraktikan> getTimeLogin(jadwalPraktikan data)
        {
            try
            {
                List<jadwalPraktikan> result = new List<jadwalPraktikan>();
                comm.CommandText = "SELECT jadwal_umum.hari, shift.waktu " +
                                   "FROM jadwal_umum INNER JOIN " +
                                        "shift ON jadwal_umum.id_shift = shift.id_shift INNER JOIN " +
                                        "jadwal_praktikan ON jadwal_umum.id_jadwal_umum = jadwal_praktikan.id_jadwal_umum " +
                                   "WHERE jadwal_praktikan.nrp = @nrp";
                comm.Parameters.AddWithValue("nrp", data.nrp);
                comm.CommandType = CommandType.Text;
                conn.Open();
                Console.WriteLine(data.nrp);
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    jadwalPraktikan list = new jadwalPraktikan()
                    {
                        id_jadwal_umum = new jadwal_umum()
                        {
                            hari = reader[0].ToString().TrimEnd(),
                            fk_jadwalUmum_Shift = new Shift()
                            {
                                waktu = reader[1].ToString().TrimEnd()
                            }
                        }
                    };
                    result.Add(list);
                }
                return result;
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

        public matkul getPraktikanPraktikum(jadwalPraktikan data)
        {
            try
            {
                comm.CommandText = @"SELECT mata_kuliah.mata_kuliah " +
                                    "FROM jadwal_praktikan INNER JOIN " +
                                        "jadwal_umum ON jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum INNER JOIN " +
                                        "mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk INNER JOIN " +
                                        "shift ON jadwal_umum.id_shift = shift.id_shift " +
                                    "WHERE jadwal_praktikan.nrp = @nrp AND shift.waktu = @waktu";
                comm.Parameters.AddWithValue("nrp", data.nrp);
                comm.Parameters.AddWithValue("waktu", data.id_jadwal_umum.fk_jadwalUmum_Shift.waktu);


                comm.CommandType = CommandType.Text;
                conn.Open();
                
                SqlDataReader reader = comm.ExecuteReader();
                matkul mk = new matkul();
                while (reader.Read())
                {
                    mk.mata_kuliah = reader[0].ToString().TrimEnd();
                }
                return mk;
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

        public praktikan getProfilePraktikan(praktikan data)
        {
            try
            {
                comm.CommandText = "SELECT praktikan.nrp, praktikan.nama, praktikan.foto, jurusan.nama_jurusan, angkatan.tahun_angkatan " +
                                    "FROM praktikan INNER JOIN " +
                                         "jurusan ON praktikan.kode_jurusan = jurusan.kode_jurusan INNER JOIN " +
                                         "angkatan ON praktikan.kode_angkatan = angkatan.kode_angkatan " +
                                    "WHERE praktikan.nrp = @nrp";
                comm.Parameters.AddWithValue("nrp", data.NRP);
                comm.CommandType = CommandType.Text;
                conn.Open();
                praktikan Profile = new praktikan();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Profile.NRP = Convert.ToString(reader[0]).TrimEnd();
                    Profile.Nama = Convert.ToString(reader[1]).TrimEnd();
                    Profile.Foto = File.ReadAllBytes(reader[2].ToString().TrimEnd());
                    Profile.jurusan = new jurusan() { NamaJurusan = Convert.ToString(reader[3]).TrimEnd() };
                    Profile.angkatan = new angkatan() { TahunAngkatan = Convert.ToString(reader[4]).TrimEnd() };
                }
                return Profile;

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

        public List<matkul> GetMatKul()
        {
            try
            {
                List<matkul> jurusan = new List<matkul>();
                comm.CommandText = "SELECT * " +
                                   "FROM mata_kuliah ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    matkul list = new matkul()
                    {
                        kode_mk = Convert.ToString(reader[0]).TrimEnd(),
                        mata_kuliah = Convert.ToString(reader[1]).TrimEnd()
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

        public List<kelas> GetKelas()
        {
            try
            {
                List<kelas> _kelas = new List<kelas>();
                comm.CommandText = "SELECT * " +
                                   "FROM kelas ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    kelas list = new kelas()
                    {
                        id_kelas = Convert.ToInt32(reader[0]),
                        Kelas = Convert.ToString(reader[1]).TrimEnd()
                    };
                    _kelas.Add(list);
                }
                return _kelas;
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

        public List<Shift> GetShift()
        {
            try
            {
                List<Shift>_Shift = new List<Shift>();
                comm.CommandText = "SELECT * " +
                                   "FROM shift ";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Shift list = new Shift()
                    {
                        id_shift = Convert.ToString(reader[0]).TrimEnd(),
                        waktu = Convert.ToString(reader[1]).TrimEnd()
                    };
                    _Shift.Add(list);
                }
                return _Shift;
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
                                    "WHERE praktikan.kode_angkatan = @kode_angkatan AND praktikan.kode_jurusan = @kode_jurusan";
                comm.Parameters.AddWithValue("kode_angkatan", data.angkatan.KodeAngkatan);
                comm.Parameters.AddWithValue("kode_jurusan", data.jurusan.KodeJurusan);
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
                        angkatan = new angkatan() { TahunAngkatan =Convert.ToString(reader[3]).TrimEnd() },
                        jurusan = new jurusan() { NamaJurusan = Convert.ToString(reader[4]).TrimEnd() }
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
                    Praktikan[x] = string.Format("('{0}', '{1}', '{2}', '{3}', '{4}')", data[x].NRP, data[x].Nama, data[x].jurusan.KodeJurusan, data[x].angkatan.KodeAngkatan, String.Format("{0}{1}\\{2}{3}",dir, data[x].NRP, data[x].Nama, format));
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

        public List<jadwal_umum> ViewJadwalUmum(jadwal_umum data)
        {
            try  
            {
                List<jadwal_umum> jadwal_umum = new List<jadwal_umum>();
                comm.CommandText = @"SELECT jadwal_umum.hari, shift.id_shift, shift.waktu, mata_kuliah.mata_kuliah, kelas.kelas " +
                                    "FROM jadwal_umum " +
                                    "INNER JOIN periode ON jadwal_umum.id_periode = periode.id_periode " +
                                    "INNER JOIN mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk " +
                                    "INNER JOIN kelas ON jadwal_umum.id_kelas = kelas.id_kelas " +
                                    "INNER JOIN shift ON jadwal_umum.id_shift = shift.id_shift " +
                                    "WHERE CONVERT(nchar(35), periode.awalSemester, 100) LIKE '%'+@awalSemester+'%' AND " +
                                            "CONVERT(nchar(35), periode.akhirSemester, 100) LIKE '%'+@akhirSemester+'%' AND " +
                                            "periode.semester = @semester";
                comm.Parameters.AddWithValue("awalSemester", data.fk_jadwalUmum_periode.awalSemester.ToString("yyyy"));
                comm.Parameters.AddWithValue("akhirSemester", data.fk_jadwalUmum_periode.akhirSemester.ToString("yyyy"));
                comm.Parameters.AddWithValue("semester", data.fk_jadwalUmum_periode.semester);
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    jadwal_umum list = new jadwal_umum()
                    {
                        hari = Convert.ToString(reader[0]).TrimEnd(),
                        fk_jadwalUmum_Shift = new Shift()
                                            {
                                                id_shift = reader[1].ToString().TrimEnd(),
                                                waktu = reader[2].ToString().TrimEnd()
                                            },
                        fk_jadwalUmum_matakuliah = new matkul()
                                            {
                                                mata_kuliah = reader[3].ToString().TrimEnd(),
                                            },
                        fk_jadwalUmum_kelas = new kelas()
                                            {
                                                Kelas = reader[4].ToString().TrimEnd(),
                                            }
                    };
                    jadwal_umum.Add(list);
                }
                return jadwal_umum;
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

        public int InsertJadwal(List<jadwal_umum> data)
        {
            try
            {
                string[] hari = new string[data.Count];
                int[] id_kelas = new int[data.Count];
                int[] id_periode = new int[data.Count];
                string[] id_shift = new string[data.Count];
                string[] kode_mk = new string[data.Count];
                for (int i = 0; i < data.Count; i++)
                {
                    hari[i] = data[i].hari;
                    id_kelas[i] = data[i].id_kelas;
                    id_periode[i] = data[i].id_periode;
                    id_shift[i] = data[i].id_shift;
                    kode_mk[i] = data[i].kode_mk;
                }
                string[] values = new string[data.Count];
                for (int i = 0; i < hari.Length; i++)
                {
                    values[i] = String.Format("('{0}', {1}, {2}, '{3}', '{4}')",
                                                hari[i], id_kelas[i], id_periode[i], id_shift[i], kode_mk[i]);
                }
                string query = string.Format("INSERT INTO jadwal_umum (hari, id_kelas, id_periode, id_shift, kode_mk) VALUES {0}", String.Join(",", values));
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

        public int DeleteJadwal(jadwal_umum data)
        {
            try
            {
                comm.CommandText = @"DELETE jadwal_umum FROM jadwal_umum " +
                                    "INNER JOIN periode ON jadwal_umum.id_periode = periode.id_periode " +
                                    "WHERE CONVERT(nchar(35), periode.awalSemester, 100) LIKE '%'+ @awalSemester +'%' AND " +
                                          "CONVERT(nchar(35), periode.akhirSemester, 100) LIKE '%'+ @akhirSemester +'%' AND " +
                                          "periode.semester = @semester";
                comm.Parameters.AddWithValue("semester", data.fk_jadwalUmum_periode.semester);
                comm.Parameters.AddWithValue("awalSemester", data.fk_jadwalUmum_periode.awalSemester.ToString("yyyy"));
                comm.Parameters.AddWithValue("akhirSemester", data.fk_jadwalUmum_periode.akhirSemester.ToString("yyyy"));
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
        
        public List<Staff> getStaffID()
        {
            try
            {
                List<Staff> StaffID = new List<Staff>();
                comm.CommandText = "SELECT staff.id_staff, staff.nama " +
                                   "FROM users INNER JOIN " +
                                        "staff ON users.username = staff.id_staff " +
                                   "WHERE users.status = 'Asisten'";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Staff list = new Staff()
                    {
                        id_staff = reader[0].ToString().TrimEnd(),
                        nama = reader[1].ToString().TrimEnd()
                    };
                    StaffID.Add(list);
                }
                return StaffID;
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

        public Staff getProfileStaff(Staff data)
        {
            try
            {
                comm.CommandText = "SELECT id_staff, nama, foto, no_hp, alamat " +
                                    "FROM staff " +
                                    "WHERE id_staff = @id_staff AND id_staff NOT LIKE '%ADM%'";
                comm.Parameters.AddWithValue("id_staff", data.id_staff);
                comm.CommandType = CommandType.Text;
                conn.Open();
                Staff profileStaff = new Staff();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    profileStaff.id_staff = Convert.ToString(reader[0]).TrimEnd();
                    profileStaff.nama = Convert.ToString(reader[1]).TrimEnd();
                    profileStaff.foto = File.ReadAllBytes(reader[2].ToString().TrimEnd());
                    profileStaff.no_hp = Convert.ToString(reader[3]).TrimEnd();
                    profileStaff.alamat = Convert.ToString(reader[4]).TrimEnd();
                }
                return profileStaff;

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

        public List<jadwalStaff> GetStaffJadwal(string data)
        {
            try
            {
                List<jadwalStaff> Jadwal_Staff = new List<jadwalStaff>();
                comm.CommandText = "SELECT jadwal_umum.hari, mata_kuliah.mata_kuliah, kelas.kelas, shift.id_shift, shift.waktu " +
                                    "FROM jadwal_staff " +
                                    "INNER JOIN jadwal_umum ON jadwal_staff.id_jadwal_umum = jadwal_umum.id_jadwal_umum " +
                                    "INNER JOIN mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk " +
                                    "INNER JOIN shift ON jadwal_umum.id_shift = shift.id_shift " +
                                    "INNER JOIN kelas ON jadwal_umum.id_kelas = kelas.id_kelas " +
                                    "WHERE jadwal_staff.id_staff = @id_staff";
                comm.Parameters.AddWithValue("id_staff", data);
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    //jadwalStaff list = new jadwalStaff()
                    //{
                    //    hari = Convert.ToString(reader[0]).TrimEnd(),
                    //    mata_kuliah = Convert.ToString(reader[1]).TrimEnd(),
                    //    kelas = Convert.ToString(reader[2]).TrimEnd(),
                    //    shift = Convert.ToString(reader[3]).TrimEnd(),
                    //    waktu = Convert.ToString(reader[4]).TrimEnd(),
                    //};
                    //Jadwal_Staff.Add(list);
                }
                return Jadwal_Staff;
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

        public List<jadwalStaff> jadwalUmumStaff(periode data)
        {
            try
            {
                List<jadwalStaff> jadwal = new List<jadwalStaff>();

                comm.CommandText = "SELECT jadwal_umum.hari, shift.id_shift, shift.waktu, mata_kuliah.mata_kuliah, staff.nama " +
                                    "FROM jadwal_staff INNER JOIN " +
                                        "jadwal_umum ON jadwal_staff.id_jadwal_umum = jadwal_umum.id_jadwal_umum INNER JOIN " +
                                        "mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk INNER JOIN " +
                                        "shift ON jadwal_umum.id_shift = shift.id_shift INNER JOIN " +
                                        "periode ON jadwal_umum.id_periode = periode.id_periode INNER JOIN " +
                                        "staff ON jadwal_staff.id_staff = staff.id_staff " +
                                    "WHERE CONVERT(varchar, periode.awalSemester, 100) LIKE '%'+@awalSemester+'%' AND " +
                                          "CONVERT(varchar, periode.akhirSemester, 100) LIKE '%'+@akhirSemester+'%' AND " +
                                          "periode.semester = 'ganjil'";
                comm.Parameters.AddWithValue("awalSemester", data.awalSemester.ToString("yyyy"));
                comm.Parameters.AddWithValue("akhirSemester", data.akhirSemester.ToString("yyyy"));
                comm.Parameters.AddWithValue("semester", data.semester);
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    jadwalStaff list = new jadwalStaff()
                    {
                        jadwal_umum = new jadwal_umum()
                                            {
                                                hari = reader[0].ToString().TrimEnd(),
                                                fk_jadwalUmum_Shift = new Shift() {  id_shift = reader[1].ToString().TrimEnd(), waktu = reader[2].ToString().TrimEnd()},
                                                fk_jadwalUmum_matakuliah = new matkul() { mata_kuliah = reader[3].ToString().TrimEnd() }
                                            },
                        staff = new Staff() { nama = reader[4].ToString().TrimEnd()}
                    };
                    jadwal.Add(list);
                }
                return jadwal;
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

        public int updateJadwalStaff(jadwalStaff data)
        {
            return comm.ExecuteNonQuery();
        }

        public int addPeriode(periode data)
        {
            try
            {
                comm.CommandText = "INSERT INTO periode (semester,awalSemester,akhirSemester) " +
                                   "VALUES (@semester, @awalSemester, @akhirSemester)";
                comm.Parameters.AddWithValue("semester", data.semester);
                comm.Parameters.AddWithValue("awalSemester", data.awalSemester);
                comm.Parameters.AddWithValue("akhirSemester", data.akhirSemester);
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

        public List<periode> viewPeriode()
        {
            try
            {
                List<periode> periode = new List<periode>();
                comm.CommandText = "SELECT semester, awalSemester, akhirSemester, id_periode " +
                                   "FROM periode";

                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    periode list = new periode()
                    {
                        semester = Convert.ToString(reader[0]).TrimEnd(),
                        awalSemester = Convert.ToDateTime(reader[1]),
                        akhirSemester = Convert.ToDateTime(reader[2]),
                        id_periode = Convert.ToInt32(reader[3])
                    };
                    periode.Add(list);
                }
                return periode;
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

