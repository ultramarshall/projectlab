using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using static System.IO.Directory;
using static System.String;

namespace SvApp
{
    public class SrvcApp : Iadm, IDisposable
    {
        SqlConnection _conn;
        SqlCommand _comm;
        SqlConnectionStringBuilder _connStringBuilder;

        public void Dispose()
        {
            if (_conn != null)
            {
                _conn.Dispose();
                _conn = null;
            }
            if (_comm == null) return;
            _comm.Dispose();
            _comm = null;
        }

        private SrvcApp()
        {
            db_connect();
        }

        private void db_connect()
        {
            _connStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = "DESKTOP-Q65MMH5",
                InitialCatalog = "labdb",
                Encrypt = true,
                TrustServerCertificate = true,
                ConnectTimeout = 30,
                AsynchronousProcessing = true,
                MultipleActiveResultSets = true,
                IntegratedSecurity = true,
            };
            _conn = new SqlConnection(_connStringBuilder.ToString());
            _comm = _conn.CreateCommand();
        }

        public string GetLogin(akun data)
        {
            string role = Empty;
            try
            {
                _comm.CommandText = "SELECT status " +
                                    "FROM users " +
                                    "WHERE username = @username AND password = @password";
                _comm.Parameters.AddWithValue("username", data.Username.TrimEnd());
                _comm.Parameters.AddWithValue("password",data.Password.TrimEnd() ) ;
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read()) role = Convert.ToString(reader[0]).TrimEnd();
                return role;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<jadwalPraktikan> getTimeLogin(jadwalPraktikan data)
        {
            try
            {
                List<jadwalPraktikan> result = new List<jadwalPraktikan>();
                _comm.CommandText = "SELECT jadwal_umum.hari, shift.waktu " +
                                    "FROM jadwal_umum INNER JOIN " +
                                    "shift ON jadwal_umum.id_shift = shift.id_shift INNER JOIN " +
                                    "jadwal_praktikan ON jadwal_umum.id_jadwal_umum = jadwal_praktikan.id_jadwal_umum " +
                                    "WHERE jadwal_praktikan.nrp = @nrp";
                _comm.Parameters.AddWithValue("nrp", data.nrp);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                Console.WriteLine(data.nrp);
                SqlDataReader reader = _comm.ExecuteReader();
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
            finally
            {
                _conn?.Close();
            }
        }

        public List<jadwalPraktikan> getPraktikanPraktikum(string nrp, string shift, int periode)
        {
            try
            {
                _comm.CommandText =
                    "SELECT jadwal_umum.hari, mata_kuliah.mata_kuliah, jadwal_praktikan.id_jadwal_praktikan, shift.mulai, shift.selesai " +
                    "FROM jadwal_umum INNER JOIN " +
                    "jadwal_praktikan ON jadwal_umum.id_jadwal_umum = jadwal_praktikan.id_jadwal_umum AND jadwal_praktikan.nrp = '" +
                    nrp + "' INNER JOIN " +
                    "mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk INNER JOIN " +
                    "shift ON jadwal_umum.id_shift = shift.id_shift " +
                    "WHERE jadwal_umum.id_shift = '" + shift + "' AND jadwal_umum.id_periode = '" + periode + "'";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                List<jadwalPraktikan> jadwal = new List<jadwalPraktikan>();
                while (reader.Read())
                {
                    jadwalPraktikan list = new jadwalPraktikan()
                    {
                        id_jadwal_umum = new jadwal_umum()
                        {
                            hari = reader[0].ToString().TrimEnd(),
                            fk_jadwalUmum_matakuliah = new matkul()
                            {
                                mata_kuliah = reader[1].ToString().TrimEnd()
                            },
                            fk_jadwalUmum_Shift = new Shift()
                            {
                                mulai = Convert.ToDateTime(reader[3].ToString().TrimEnd()),
                                selesai = Convert.ToDateTime(reader[4].ToString().TrimEnd())
                            }
                        },
                        id_jadwal_praktikan = Convert.ToInt32(reader[2]),
                    };
                    jadwal.Add(list);
                }
                return jadwal;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public praktikan getProfilePraktikan(praktikan data)
        {
            try
            {
                _comm.CommandText = @"SELECT    praktikan.nrp, 
                                                praktikan.nama, 
                                                praktikan.foto, 
                                                jurusan.nama_jurusan, 
                                                angkatan.tahun_angkatan, 
                                                users.password, 
                                                users.status
                                      FROM      praktikan INNER JOIN
                                                jurusan ON praktikan.kode_jurusan = jurusan.kode_jurusan INNER JOIN
                                                angkatan ON praktikan.kode_angkatan = angkatan.kode_angkatan INNER JOIN
                                                users ON praktikan.nrp = users.username
                                      WHERE     (praktikan.nrp = @nrp)";
                _comm.Parameters.AddWithValue("nrp", data.NRP);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                praktikan profile = new praktikan();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    profile = new praktikan()
                    {
                        NRP = Convert.ToString(reader[0]).TrimEnd(),
                        Nama = Convert.ToString(reader[1]).TrimEnd(),
                        Foto = File.ReadAllBytes(reader[2].ToString().TrimEnd()),
                        jurusan = new jurusan() {NamaJurusan = Convert.ToString(reader[3]).TrimEnd()},
                        angkatan = new angkatan() {TahunAngkatan = Convert.ToString(reader[4]).TrimEnd()},
                        Users = new Users()
                        {
                            password = reader[5].ToString().TrimEnd(),
                            status = reader[6].ToString().TrimEnd()
                        }
                    };

                    //profile.NRP = Convert.ToString(reader[0]).TrimEnd();
                    //profile.Nama = Convert.ToString(reader[1]).TrimEnd();
                    //profile.Foto = File.ReadAllBytes(reader[2].ToString().TrimEnd());
                    //profile.jurusan = new jurusan() { NamaJurusan = Convert.ToString(reader[3]).TrimEnd() };
                    //profile.angkatan = new angkatan() { TahunAngkatan = Convert.ToString(reader[4]).TrimEnd() };
                    //profile.Users.password = 
                }
                return profile;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int AddPraktikan(praktikan data)
        {
            try
            {
                const string dir = @"C:\LIK\USER\";
                const string format = @".png";
                var foo = dir + data.NRP + @"\";
                if (Exists(foo)) //kalau folder sudah ada
                    File.WriteAllBytes(foo + data.Nama + format, data.Foto); // simpan foto ke folder
                else
                {
                    CreateDirectory(foo);
                    File.WriteAllBytes(foo + data.Nama + format, data.Foto); // simpan foto ke folder
                }
                _comm.CommandText = @"INSERT INTO users 
                                      VALUES (@nrp, @password, @status); 
                                      INSERT INTO praktikan
                                      VALUES (@nrp, @nama, @foto, @kode_jurusan, @kode_angkatan)";
                _comm.Parameters.AddWithValue("nrp", data.NRP);
                _comm.Parameters.AddWithValue("password", data.Users.password);
                _comm.Parameters.AddWithValue("status", data.Users.status);
                _comm.Parameters.AddWithValue("nama", data.Nama);
                _comm.Parameters.AddWithValue("foto", @"C:\LIK\USER\" + data.NRP + @"\" + data.Nama + ".png");
                _comm.Parameters.AddWithValue("kode_angkatan", data.angkatan.KodeAngkatan);
                _comm.Parameters.AddWithValue("kode_jurusan", data.jurusan.KodeJurusan);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int EditPraktikan(string nrp, praktikan data)
        {
            try
            {
                const string dir = @"C:\LIK\USER\";
                const string format = @".png";
                var foo = dir + data.NRP + @"\";
                if (Exists(foo)) //kalau folder sudah ada
                    File.WriteAllBytes(foo + data.Nama + format, data.Foto); // simpan foto ke folder
                else
                {
                    CreateDirectory(foo);
                    File.WriteAllBytes(foo + data.Nama + format, data.Foto); // simpan foto ke folder
                }
                _comm.CommandText = @"UPDATE    users 
                                      SET       username        = @new_nrp, 
                                                password        = @password, 
                                                status          = @status
                                      WHERE     username        = @nrp; 
                                      UPDATE    praktikan
                                      SET       nrp             = @new_nrp,
                                                nama            = @nama,
                                                foto            = @foto,
                                                kode_jurusan    = @kode_jurusan, 
                                                kode_angkatan   = @kode_angkatan
                                      WHERE     nrp             = @nrp";
                _comm.Parameters.AddWithValue("nrp", nrp);
                _comm.Parameters.AddWithValue("new_nrp", data.NRP);
                _comm.Parameters.AddWithValue("password", data.Users.password);
                _comm.Parameters.AddWithValue("status", data.Users.status);
                _comm.Parameters.AddWithValue("nama", data.Nama);
                _comm.Parameters.AddWithValue("foto", @"C:\LIK\USER\" + data.NRP + @"\" + data.Nama + ".png");
                _comm.Parameters.AddWithValue("kode_angkatan", data.angkatan.KodeAngkatan);
                _comm.Parameters.AddWithValue("Kode_jurusan", data.jurusan.KodeJurusan);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<jurusan> GetJurusan()
        {
            try
            {
                List<jurusan> jurusan = new List<jurusan>();
                _comm.CommandText = "SELECT * " +
                                    "FROM jurusan ";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
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
            finally
            {
                _conn?.Close();
            }
        }

        public List<matkul> GetMatKul()
        {
            try
            {
                List<matkul> jurusan = new List<matkul>();
                _comm.CommandText = "SELECT * " +
                                    "FROM mata_kuliah ";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
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
            finally
            {
                _conn?.Close();
            }
        }

        public List<angkatan> GetAngkatan()
        {
            try
            {
                List<angkatan> angkatan = new List<angkatan>();
                _comm.CommandText = "SELECT * " +
                                    "FROM angkatan ";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
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
            finally
            {
                _conn?.Close();
            }
        }

        public List<kelas> GetKelas()
        {
            try
            {
                List<kelas> kelas = new List<kelas>();
                _comm.CommandText = "SELECT * " +
                                    "FROM kelas ";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    kelas list = new kelas()
                    {
                        id_kelas = Convert.ToInt32(reader[0]),
                        Kelas = Convert.ToString(reader[1]).TrimEnd()
                    };
                    kelas.Add(list);
                }
                return kelas;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<Shift> GetShift()
        {
            try
            {
                List<Shift> shift = new List<Shift>();
                _comm.CommandText = "SELECT id_shift, mulai, selesai " +
                                    "FROM shift ";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    Shift list = new Shift()
                    {
                        id_shift = Convert.ToString(reader[0]).TrimEnd(),

                        //waktu = Convert.ToString(reader[1]).TrimEnd()
                        mulai = Convert.ToDateTime(reader[1].ToString()),
                        selesai = Convert.ToDateTime(reader[2].ToString())
                    };
                    shift.Add(list);
                }
                return shift;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<praktikan> GetPraktikan(praktikan data)
        {
            try
            {
                List<praktikan> users = new List<praktikan>();
                _comm.CommandText = "SELECT praktikan.nrp, " +
                                    "praktikan.nama, " +
                                    "praktikan.foto, " +
                                    "angkatan.tahun_angkatan, " +
                                    "jurusan.nama_jurusan " +
                                    "FROM angkatan " +
                                    "INNER JOIN praktikan ON angkatan.kode_angkatan = praktikan.kode_angkatan " +
                                    "INNER JOIN jurusan ON praktikan.kode_jurusan = jurusan.kode_jurusan " +
                                    "WHERE praktikan.kode_angkatan = @kode_angkatan AND praktikan.kode_jurusan = @kode_jurusan";
                _comm.Parameters.AddWithValue("kode_angkatan", data.angkatan.KodeAngkatan);
                _comm.Parameters.AddWithValue("kode_jurusan", data.jurusan.KodeJurusan);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    praktikan user = new praktikan()
                    {
                        NRP = Convert.ToString(reader[0]).TrimEnd(),
                        Nama = Convert.ToString(reader[1]).TrimEnd(),
                        Foto = File.ReadAllBytes(Convert.ToString(reader[2])),
                        angkatan = new angkatan() {TahunAngkatan = Convert.ToString(reader[3]).TrimEnd()},
                        jurusan = new jurusan() {NamaJurusan = Convert.ToString(reader[4]).TrimEnd()}
                    };
                    users.Add(user);
                }
                return users;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int InsertMultiplePraktikan(List<praktikan> data)
        {
            const string dir = @"C:\LIK\USER\";
            const string format = @".png";
            var loc = new string[data.Count];
            try
            {
                for (var i = 0; i < data.Count; i++)
                {
                    string foo = $"{dir}/{data[i].NRP}/";

                    //cek folder user di server
                    if (Exists(foo)) //kalau folder sudah ada
                        File.WriteAllBytes(@foo + data[i].Nama + format, data[i].Foto); // simpan foto ke folder
                    else
                    {
                        loc[i] = @foo + data[i].Nama;
                        CreateDirectory(foo);
                        File.WriteAllBytes(@foo + data[i].Nama + format, data[i].Foto); // simpan foto ke folder
                    }
                }
                string[] akun = new string[data.Count];
                string[] praktikan = new string[data.Count];
                for (int x = 0; x < data.Count; x++)
                {
                    akun[x] = $"('{data[x].NRP}', '{data[x].NRP}', 'Praktikan')";
                    praktikan[x] = $"('{data[x].NRP}', '{data[x].Nama}', '{data[x].jurusan.KodeJurusan}', '{data[x].angkatan.KodeAngkatan}', '{$"{dir}{data[x].NRP}\\{data[x].Nama}{format}"}')";
                }
                //Array.Clear(akun, 0, akun.Length);
                //Array.Clear(praktikan, 0, praktikan.Length);
                string query = $"INSERT INTO users (Username, Password, status) VALUES {Join(",", akun)}" +
                               $"INSERT INTO praktikan (NRP, Nama, kode_jurusan, kode_angkatan, Foto) VALUES {Join(",", praktikan)}";
                _comm.CommandText = query;
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<jadwal_umum> ViewJadwalUmum(jadwal_umum data)
        {
            try
            {
                List<jadwal_umum> jadwalUmum = new List<jadwal_umum>();
                _comm.CommandText =
                    @"SELECT jadwal_umum.hari, shift.id_shift, shift.mulai, shift.selesai, mata_kuliah.mata_kuliah, kelas.kelas, jadwal_umum.id_jadwal_umum " +
                    "FROM jadwal_umum " +
                    "INNER JOIN periode ON jadwal_umum.id_periode = periode.id_periode " +
                    "INNER JOIN mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk " +
                    "INNER JOIN kelas ON jadwal_umum.id_kelas = kelas.id_kelas " +
                    "INNER JOIN shift ON jadwal_umum.id_shift = shift.id_shift " +
                    "WHERE jadwal_umum.id_periode = @id_periode";
                _comm.Parameters.AddWithValue("id_periode", data.id_periode);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    jadwal_umum list = new jadwal_umum()
                    {
                        hari = Convert.ToString(reader[0]).TrimEnd(),
                        fk_jadwalUmum_Shift = new Shift()
                        {
                            id_shift = reader[1].ToString().TrimEnd(),

                            //waktu = reader[2].ToString().TrimEnd()
                            mulai = Convert.ToDateTime(reader[2].ToString()),
                            selesai = Convert.ToDateTime(reader[3].ToString())
                        },
                        fk_jadwalUmum_matakuliah = new matkul()
                        {
                            mata_kuliah = reader[4].ToString().TrimEnd(),
                        },
                        fk_jadwalUmum_kelas = new kelas()
                        {
                            Kelas = reader[5].ToString().TrimEnd(),
                        },
                        id_jadwal_umum = Convert.ToInt16( reader[6] )
                    };
                    jadwalUmum.Add(list);
                }
                return jadwalUmum;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int InsertJadwal(List<jadwal_umum> data)
        {
            try
            {
                var hari = new string[data.Count];
                var idKelas = new int[data.Count];
                var idPeriode = new int[data.Count];
                var idShift = new string[data.Count];
                var kodeMk = new string[data.Count];
                for (var i = 0; i < data.Count; i++)
                {
                    hari[i] = data[i].hari;
                    idKelas[i] = data[i].id_kelas;
                    idPeriode[i] = data[i].id_periode;
                    idShift[i] = data[i].id_shift;
                    kodeMk[i] = data[i].kode_mk;
                }
                var values = new string[data.Count];
                for (var i = 0; i < hari.Length; i++)
                    values[i] = $"('{hari[i]}', {idKelas[i]}, {idPeriode[i]}, '{idShift[i]}', '{kodeMk[i]}')";
                string query =
                    $"INSERT INTO jadwal_umum (hari, id_kelas, id_periode, id_shift, kode_mk) VALUES {Join(",", values)}";
                _comm.CommandText = query;
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int DeleteJadwal(jadwal_umum data)
        {
            try
            {
                _comm.CommandText = @"DELETE jadwal_umum FROM jadwal_umum " +
                                    "WHERE jadwal_umum.id_periode = @id_periode";
                _comm.Parameters.AddWithValue("id_periode", data.id_periode);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<Staff> getStaffID()
        {
            try
            {
                var staffId = new List<Staff>();
                _comm.CommandText = @"SELECT    staff.id_staff, staff.nama, users.status 
                                      FROM      users INNER JOIN
                                                staff ON users.username = staff.id_staff
                                      WHERE     users.status = 'Asisten' OR users.status = 'Koordinator'";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                var reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    var list = new Staff()
                    {
                        id_staff = reader[0].ToString().TrimEnd(),
                        nama = reader[1].ToString().TrimEnd(),
                        users = new Users() {status = reader[2].ToString().TrimEnd()}
                    };
                    staffId.Add(list);
                }
                return staffId;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public Staff getProfileStaff(Staff data)
        {
            try
            {
                var p = new Staff();
                _comm.CommandText =
                    @"SELECT    users.password, users.status, staff.id_staff, staff.nama, staff.foto, staff.alamat, staff.no_hp
                                      FROM      staff INNER JOIN
                                                users ON staff.id_staff = users.username
                                      WHERE     (staff.id_staff = @id_staff)";
                _comm.Parameters.AddWithValue("id_staff", data.id_staff);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    p = new Staff()
                    {
                        users =
                            new Users()
                            {
                                password = reader[0].ToString().TrimEnd(),
                                status = reader[1].ToString().TrimEnd()
                            },
                        id_staff = reader[2].ToString().TrimEnd(),
                        nama = reader[3].ToString().TrimEnd(),
                        foto = File.ReadAllBytes(reader[4].ToString()),
                        alamat = reader[5].ToString().TrimEnd(),
                        no_hp = reader[6].ToString().TrimEnd(),
                    };
                }
                return p;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<jadwalStaff> GetStaffJadwal(jadwalStaff data)
        {
            try
            {
                List<jadwalStaff> jadwalStaff = new List<jadwalStaff>();
                _comm.CommandText =
                    "SELECT jadwal_umum.hari, mata_kuliah.mata_kuliah, kelas.kelas, shift.id_shift, shift.mulai, shift.selesai " +
                    "FROM jadwal_staff " +
                    "INNER JOIN jadwal_umum ON jadwal_staff.id_jadwal_umum = jadwal_umum.id_jadwal_umum " +
                    "INNER JOIN mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk " +
                    "INNER JOIN shift ON jadwal_umum.id_shift = shift.id_shift " +
                    "INNER JOIN kelas ON jadwal_umum.id_kelas = kelas.id_kelas " +
                    "INNER JOIN periode ON jadwal_umum.id_periode = periode.id_periode " +
                    "WHERE jadwal_staff.id_staff = @id_staff AND periode.id_periode = @id_periode";
                _comm.Parameters.AddWithValue("id_staff", data.staff.id_staff);
                _comm.Parameters.AddWithValue("id_periode", data.jadwal_umum.id_periode);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    jadwalStaff list = new jadwalStaff()
                    {
                        jadwal_umum = new jadwal_umum()
                        {
                            hari = reader[0].ToString().TrimEnd(),
                            fk_jadwalUmum_matakuliah = new matkul()
                            {
                                mata_kuliah = reader[1].ToString().TrimEnd()
                            },
                            fk_jadwalUmum_kelas = new kelas()
                            {
                                Kelas = reader[2].ToString().TrimEnd()
                            },
                            fk_jadwalUmum_Shift = new Shift()
                            {
                                id_shift = reader[3].ToString().TrimEnd(),
                                mulai = Convert.ToDateTime(reader[4].ToString()),
                                selesai = Convert.ToDateTime(reader[5].ToString())
                            }
                        },
                    };
                    jadwalStaff.Add(list);
                }
                return jadwalStaff;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<jadwalStaff> jadwalUmumStaff(periode data)
        {
            try
            {
                List<jadwalStaff> jadwal = new List<jadwalStaff>();
                _comm.CommandText = @"SELECT jadwal_umum.hari, 
                                             shift.id_shift, 
                                             shift.mulai, 
                                             shift.selesai, 
                                             mata_kuliah.mata_kuliah, 
                                             staff.nama
                                      FROM   jadwal_staff INNER JOIN
                                             jadwal_umum ON jadwal_staff.id_jadwal_umum = jadwal_umum.id_jadwal_umum INNER JOIN 
                                             mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk INNER JOIN 
                                             shift ON jadwal_umum.id_shift = shift.id_shift INNER JOIN 
                                             periode ON jadwal_umum.id_periode = periode.id_periode INNER JOIN 
                                             staff ON jadwal_staff.id_staff = staff.id_staff 
                                      WHERE  jadwal_umum.id_periode = @id_periode";
                _comm.Parameters.AddWithValue("id_periode", data.id_periode);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    jadwalStaff list = new jadwalStaff()
                    {
                        jadwal_umum = new jadwal_umum()
                        {
                            hari = reader[0].ToString().TrimEnd(),
                            fk_jadwalUmum_Shift = new Shift()
                            {
                                id_shift = reader[1].ToString().TrimEnd(),
                                mulai = Convert.ToDateTime(reader[2].ToString()),
                                selesai = Convert.ToDateTime(reader[3].ToString()),
                            },
                            fk_jadwalUmum_matakuliah = new matkul()
                            {
                                mata_kuliah = reader[4].ToString().TrimEnd()
                            }
                        },
                        staff = new Staff()
                        {
                            nama = reader[5].ToString().TrimEnd()
                        }
                    };
                    jadwal.Add(list);
                }
                return jadwal;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int updateJadwalStaff(string id, jadwalStaff data)
        {
            try
            {
                _comm.CommandText = @"UPDATE jadwal_staff
                                     SET    id_staff = @replace_id_staff
                                     FROM   jadwal_staff INNER JOIN
                                            jadwal_umum ON jadwal_staff.id_jadwal_umum = jadwal_umum.id_jadwal_umum AND jadwal_staff.id_staff = @id_staff INNER JOIN
                                            mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk AND mata_kuliah.mata_kuliah = @mata_kuliah INNER JOIN
                                            periode ON jadwal_umum.id_periode = periode.id_periode AND periode.id_periode = @id_periode INNER JOIN
                                            shift ON jadwal_umum.id_shift = shift.id_shift AND shift.id_shift = @id_shift";
                _comm.Parameters.AddWithValue("replace_id_staff", id);
                _comm.Parameters.AddWithValue("id_staff", data.staff.id_staff);
                _comm.Parameters.AddWithValue("id_periode", data.jadwal_umum.fk_jadwalUmum_periode.id_periode);
                _comm.Parameters.AddWithValue("id_shift", data.jadwal_umum.fk_jadwalUmum_Shift.id_shift);
                _comm.Parameters.AddWithValue("mata_kuliah", data.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int addPeriode(periode data)
        {
            try
            {
                _comm.CommandText = "INSERT INTO periode (semester,awalSemester,akhirSemester) " +
                                    "VALUES (@semester, @awalSemester, @akhirSemester)";
                _comm.Parameters.AddWithValue("semester", data.semester);
                _comm.Parameters.AddWithValue("awalSemester", data.awalSemester);
                _comm.Parameters.AddWithValue("akhirSemester", data.akhirSemester);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<periode> viewPeriode()
        {
            try
            {
                List<periode> periode = new List<periode>();
                _comm.CommandText = "SELECT semester, awalSemester, akhirSemester, id_periode " +
                                    "FROM periode";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
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
            finally
            {
                _conn?.Close();
            }
        }

        public List<pertemuan> GetPertemuan(jadwalPraktikan data)
        {
            try
            {
                List<pertemuan> pertemuan = new List<pertemuan>();
                _comm.CommandText = "SELECT id_pertemuan, id_jenis_pertemuan " +
                                    "FROM pertemuan " +
                                    "WHERE id_pertemuan NOT IN ( SELECT id_pertemuan " +
                                    "FROM absensi_praktikan " +
                                    "WHERE id_jadwal_praktikan = " + data.id_jadwal_praktikan + ")";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    pertemuan list = new pertemuan()
                    {
                        id_pertemuan = Convert.ToInt32(reader[0]),
                        id_jenis_pertemuan = reader[1].ToString().TrimEnd()
                    };
                    pertemuan.Add(list);
                }
                return pertemuan;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int PostAbsenPraktikan(AbsensiPraktikan data)
        {
            try
            {
                _comm.CommandText = "INSERT INTO absensi_praktikan (id_pertemuan, id_jadwal_praktikan, waktu_absensi) " +
                                    "VALUES (@id_pertemuan, @id_jadwal_praktikan, getdate())";
                _comm.Parameters.AddWithValue("id_pertemuan", data.Pertemuan.id_pertemuan);
                _comm.Parameters.AddWithValue("id_jadwal_praktikan", data.JadwalPraktikan.id_jadwal_praktikan);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<pertemuan> ListPertemuan()
        {
            try
            {
                List<pertemuan> pertemuan = new List<pertemuan>();
                _comm.CommandText = "SELECT id_pertemuan, id_jenis_pertemuan " +
                                    "FROM pertemuan";
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    pertemuan list = new pertemuan()
                    {
                        id_pertemuan = Convert.ToInt32(reader[0]),
                        id_jenis_pertemuan = reader[1].ToString().TrimEnd()
                    };
                    pertemuan.Add(list);
                }
                return pertemuan;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public List<praktikan> GetAbsensiPraktikans(AbsensiPraktikan data)
        {
            try
            {
                var praktikan = new List<praktikan>();
                _comm.CommandText = @"SELECT	praktikan.foto, praktikan.nrp, praktikan.nama, absensi_praktikan.nilai
                                      FROM      absensi_praktikan 
		                                        INNER JOIN pertemuan 
		                                        ON absensi_praktikan.id_pertemuan = pertemuan.id_pertemuan 
		                                        AND pertemuan.id_jenis_pertemuan = @jenis_pertemuan 
		                                        INNER JOIN jadwal_praktikan 
		                                        ON absensi_praktikan.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan 
		                                        INNER JOIN jadwal_umum 
		                                        ON jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum 
		                                        INNER JOIN periode 
		                                        ON jadwal_umum.id_periode = periode.id_periode 
		                                        AND periode.id_periode = @id_periode 
		                                        INNER JOIN shift 
		                                        ON jadwal_umum.id_shift = shift.id_shift 
		                                        AND shift.id_shift = @id_shift 
		                                        INNER JOIN mata_kuliah 
		                                        ON jadwal_umum.kode_mk = mata_kuliah.kode_mk 
		                                        AND mata_kuliah.mata_kuliah = @mk 
		                                        INNER JOIN praktikan 
		                                        ON jadwal_praktikan.nrp = praktikan.nrp";
                _comm.Parameters.AddWithValue("jenis_pertemuan", data.Pertemuan.id_jenis_pertemuan);
                _comm.Parameters.AddWithValue("id_periode",
                    data.JadwalPraktikan.id_jadwal_umum.fk_jadwalUmum_periode.id_periode);
                _comm.Parameters.AddWithValue("id_shift",
                    data.JadwalPraktikan.id_jadwal_umum.fk_jadwalUmum_Shift.id_shift);
                _comm.Parameters.AddWithValue("mk",
                    data.JadwalPraktikan.id_jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                while (reader.Read())
                {
                    var nilai = 0;
                    if (DBNull.Value.Equals(reader[3]))
                    {
                        nilai = 0;
                    }
                    else
                    {
                        nilai = Convert.ToInt32(reader[3]);
                    }
                    var list = new praktikan()
                    {
                        Foto = File.ReadAllBytes(reader[0].ToString()),
                        NRP = reader[1].ToString().TrimEnd(),
                        Nama = reader[2].ToString().TrimEnd(),
                        absen = new AbsensiPraktikan() {Nilai = nilai}
                    };
                    praktikan.Add(list);
                }
                return praktikan;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int AddStaff(Staff data)
        {
            try
            {
                const string dir = @"C:\LIK\STAFF\";
                const string format = @".png";
                var foo = dir + data.id_staff + @"\";
                if (Exists(foo)) //kalau folder sudah ada
                    File.WriteAllBytes(foo + data.nama + format, data.foto); // simpan foto ke folder
                else
                {
                    CreateDirectory(foo);
                    File.WriteAllBytes(foo + data.nama + format, data.foto); // simpan foto ke folder
                }
                _comm.CommandText = @"INSERT INTO users 
                                      VALUES (@id_staff, @password, @status); 
                                      INSERT INTO staff
                                      VALUES (@id_staff, @nama, @foto, @alamat, @no_hp)";
                _comm.Parameters.AddWithValue("id_staff", data.id_staff);
                _comm.Parameters.AddWithValue("password", data.users.password);
                _comm.Parameters.AddWithValue("status", data.users.status);
                _comm.Parameters.AddWithValue("nama", data.nama);
                _comm.Parameters.AddWithValue("foto", @"C:\LIK\STAFF\" + data.id_staff + @"\" + data.nama + ".png");
                _comm.Parameters.AddWithValue("alamat", data.alamat);
                _comm.Parameters.AddWithValue("no_hp", data.no_hp);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int EditStaff(string id_staff, Staff data)
        {
            try
            {
                const string dir = @"C:\LIK\STAFF\";
                const string format = @".png";
                var foo = dir + data.id_staff + @"\";
                if (Exists(foo)) //kalau folder sudah ada
                    File.WriteAllBytes(foo + data.nama + format, data.foto); // simpan foto ke folder
                else
                {
                    CreateDirectory(foo);
                    File.WriteAllBytes(foo + data.nama + format, data.foto); // simpan foto ke folder
                }
                _comm.CommandText = @"UPDATE    users 
                                      SET       username = @id_staff, 
                                                password = @password, 
                                                status = @status
                                      WHERE     username = @idStaff; 
                                      UPDATE    staff
                                      SET       id_staff    = @id_staff,
                                                nama        = @nama,
                                                foto        = @foto,
                                                alamat      = @alamat, 
                                                no_hp       = @no_hp
                                      WHERE     id_staff    = @id_staff";
                _comm.Parameters.AddWithValue("idStaff", id_staff);
                _comm.Parameters.AddWithValue("id_staff", data.id_staff);
                _comm.Parameters.AddWithValue("password", data.users.password);
                _comm.Parameters.AddWithValue("status", data.users.status);
                _comm.Parameters.AddWithValue("nama", data.nama);
                _comm.Parameters.AddWithValue("foto", @"C:\LIK\STAFF\" + data.id_staff + @"\" + data.nama + ".png");
                _comm.Parameters.AddWithValue("alamat", data.alamat);
                _comm.Parameters.AddWithValue("no_hp", data.no_hp);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int KonfirmasiAbsensi(AbsensiPraktikan data)
        {
            try
            {
                _comm.CommandText = @"UPDATE    absensi_praktikan
                                      SET       nilai = @nilai, 
                                                id_staff = @id_staff
                                      WHERE	    id_absensi IN( SELECT	absensi_praktikan_1.id_absensi
						                                       FROM	    absensi_praktikan AS absensi_praktikan_1 
								                                        INNER JOIN pertemuan 
									                                        ON absensi_praktikan_1.id_pertemuan = pertemuan.id_pertemuan 
									                                        AND pertemuan.id_jenis_pertemuan = @jenis_pertemuan 
								                                        INNER JOIN jadwal_praktikan 
									                                        ON absensi_praktikan_1.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan 
									                                        AND jadwal_praktikan.nrp = @nrp 
								                                        INNER JOIN jadwal_umum 
									                                        ON jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum 
								                                        INNER JOIN shift 
									                                        ON jadwal_umum.id_shift = shift.id_shift 
									                                        AND shift.id_shift = @id_shift 
								                                        INNER JOIN periode 
									                                        ON jadwal_umum.id_periode = periode.id_periode 
									                                        AND periode.id_periode = @id_periode )";
                _comm.Parameters.AddWithValue("nilai", data.Nilai);
                _comm.Parameters.AddWithValue("id_staff", data.staff.id_staff);
                _comm.Parameters.AddWithValue("jenis_pertemuan", data.Pertemuan.id_jenis_pertemuan);
                _comm.Parameters.AddWithValue("nrp", data.JadwalPraktikan.nrp);
                _comm.Parameters.AddWithValue("id_shift", data.JadwalPraktikan.id_jadwal_umum.id_shift);
                _comm.Parameters.AddWithValue("id_periode", data.JadwalPraktikan.id_jadwal_umum.id_periode);

                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int HapusAbsensi (AbsensiPraktikan data)
        {
            try
            {
                _comm.CommandText = @"DELETE    
                                      FROM      absensi_praktikan
                                      WHERE	    id_absensi IN( SELECT	absensi_praktikan_1.id_absensi
						                                       FROM	    absensi_praktikan AS absensi_praktikan_1 
								                                        INNER JOIN pertemuan 
									                                        ON absensi_praktikan_1.id_pertemuan = pertemuan.id_pertemuan 
									                                        AND pertemuan.id_jenis_pertemuan = @jenis_pertemuan 
								                                        INNER JOIN jadwal_praktikan 
									                                        ON absensi_praktikan_1.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan 
									                                        AND jadwal_praktikan.nrp = @nrp 
								                                        INNER JOIN jadwal_umum 
									                                        ON jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum 
								                                        INNER JOIN shift 
									                                        ON jadwal_umum.id_shift = shift.id_shift 
									                                        AND shift.id_shift = @id_shift 
								                                        INNER JOIN periode 
									                                        ON jadwal_umum.id_periode = periode.id_periode 
									                                        AND periode.id_periode = @id_periode )";

                _comm.Parameters.AddWithValue( "jenis_pertemuan", data.Pertemuan.id_jenis_pertemuan );
                _comm.Parameters.AddWithValue( "nrp", data.JadwalPraktikan.nrp );
                _comm.Parameters.AddWithValue( "id_shift", data.JadwalPraktikan.id_jadwal_umum.id_shift );
                _comm.Parameters.AddWithValue( "id_periode", data.JadwalPraktikan.id_jadwal_umum.id_periode );

                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                return _comm.ExecuteNonQuery( );
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public List<jadwalPraktikan> GetJadwalPraktikan (jadwalPraktikan data)
        {
            try
            {
                var jadwal = new List<jadwalPraktikan>( );
                _comm.CommandText = @"SELECT	id_jadwal_praktikan, nrp, id_jadwal_umum
                                      FROM	    jadwal_praktikan
                                      WHERE	    (id_jadwal_umum IN
		                                            (SELECT    id_jadwal_umum
		                                             FROM      jadwal_umum
		                                             WHERE     (id_periode = @id_periode))) AND (nrp = @nrp)";
                _comm.Parameters.AddWithValue( "nrp", data.nrp );
                _comm.Parameters.AddWithValue( "id_periode", data.id_jadwal_umum.id_periode );
                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                SqlDataReader reader = _comm.ExecuteReader( );
                while ( reader.Read( ) )
                {
                    var list = new jadwalPraktikan( )
                    {
                        id_jadwal_praktikan = Convert.ToInt16( reader[0] ),
                        nrp = reader[1].ToString().TrimEnd(),
                        id_jadwal_umum = new jadwal_umum( ) { id_jadwal_umum = Convert.ToInt16(reader[2])}
                    };
                    jadwal.Add( list );
                }
                return jadwal;
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public List<jadwalStaff> GetJadwalAsisten (jadwalStaff data)
        {
            try
            {
                var jadwal = new List<jadwalStaff>( );
                _comm.CommandText = @"SELECT	id_jadwal_staff, id_staff , id_jadwal_umum
                                      FROM	    jadwal_staff
                                      WHERE	    (id_jadwal_umum IN
		                                            (SELECT    id_jadwal_umum
		                                             FROM      jadwal_umum
		                                             WHERE     (id_periode = @id_periode))) AND (id_staff = @id_staff)";
                _comm.Parameters.AddWithValue( "id_staff", data.staff.id_staff );
                _comm.Parameters.AddWithValue( "id_periode", data.jadwal_umum.id_periode );
                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                SqlDataReader reader = _comm.ExecuteReader( );
                while ( reader.Read( ) )
                {
                    var list = new jadwalStaff( )
                    {
                        id_jadwal_staff = Convert.ToInt16( reader[0] ),
                        staff = new Staff( ) { id_staff = reader[1].ToString( ).TrimEnd( ) },
                        jadwal_umum = new jadwal_umum( ) { id_jadwal_umum = Convert.ToInt16( reader[2] ) }
                    };
                    jadwal.Add( list );
                }
                return jadwal;
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public int AddJadwalPraktikan(List<jadwalPraktikan> data)
        {
            try
            {
                var v = new string[data.Count];
                for ( var i = 0; i < data.Count; i++ )
                {
                    v[i] = string.Format("('{0}', {1})", 
                        data[i].nrp, 
                        data[i].id_jadwal_umum.id_jadwal_umum );
                }
                var val = string.Join( ",", v );
                _comm.CommandText = "INSERT INTO jadwal_praktikan (nrp, id_jadwal_umum) " +
                                    "VALUES " + val;

                _comm.CommandType = CommandType.Text;
                _conn.Open();
                return _comm.ExecuteNonQuery();
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int AddJadwalStaffAsisten (List<jadwalStaff> data)
        {
            try
            {
                var v = new string[data.Count];
                for ( var i = 0; i < data.Count; i++ )
                {
                    v[i] = string.Format( "('{0}', {1})",
                        data[i].staff.id_staff,
                        data[i].jadwal_umum.id_jadwal_umum );
                }
                var val = string.Join( ",", v );
                _comm.CommandText = "INSERT INTO jadwal_staff (id_staff, id_jadwal_umum) " +
                                    "VALUES " + val;

                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                return _comm.ExecuteNonQuery( );
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public int DeleteJadwalPraktikan (jadwalPraktikan data)
        {
            try
            {
                _comm.CommandText = @"DELETE 
                                      FROM   jadwal_praktikan
                                      WHERE  nrp = @nrp AND id_jadwal_umum IN(SELECT    id_jadwal_umum
		                                                       FROM      jadwal_umum
		                                                       WHERE     id_periode = @id_periode)";
                _comm.Parameters.AddWithValue( "nrp", data.nrp );
                _comm.Parameters.AddWithValue( "id_periode", data.id_jadwal_umum.id_periode);

                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                return _comm.ExecuteNonQuery( );
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public int TambahAngaktan (angkatan data)
        {
            try
            {
                _comm.CommandText = @"INSERT INTO angkatan 
                                      VALUES (@kode_angkatan, @tahun_angkatan)";
                _comm.Parameters.AddWithValue( "kode_angkatan", data.KodeAngkatan );
                _comm.Parameters.AddWithValue( "tahun_angkatan", data.TahunAngkatan );

                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                return _comm.ExecuteNonQuery( );
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public modul GetModul(modul data)
        {
            try
            {
                _comm.CommandText = @"SELECT lokasi_modul
                                      FROM   modul
                                      WHERE  file_modul = @file AND kode_mk = @kode_mk";
                _comm.Parameters.AddWithValue( "file", data.file_modul );
                _comm.Parameters.AddWithValue( "kode_mk", data.matkul.kode_mk );
                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                SqlDataReader reader = _comm.ExecuteReader( );

                var modul = new modul();
                while ( reader.Read() )
                {
                    modul.lokasi_modul = File.ReadAllBytes( reader[0].ToString().TrimEnd() );
                }
                return modul;
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public List<modul> GetListModul (modul data)
        {
            try
            {
                _comm.CommandText = @"SELECT file_modul
                                      FROM   modul
                                      WHERE  kode_mk = @kode_mk";
                _comm.Parameters.AddWithValue( "kode_mk", data.matkul.kode_mk );
                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                SqlDataReader reader = _comm.ExecuteReader( );

                var modul = new List<modul>( );
                while ( reader.Read( ) )
                {
                    var list = new modul( )
                    {
                        file_modul = reader[0].ToString( ).TrimEnd( )
                    };
                    modul.Add( list );
                }
                return modul;
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public int UploadModul (modul data)
        {
            const string dir = @"C:\LIK\MODUL\";
            try
            {
                var loc = dir+data.matkul.kode_mk+"\\";
                var filedir = loc + data.file_modul + " - " + data.modul_file;
                _comm.CommandText = @"INSERT INTO modul 
                                      VALUES (@kode_mk, @file_modul, @lokasi_modul)";
                _comm.Parameters.AddWithValue( "kode_mk", data.matkul.kode_mk );
                _comm.Parameters.AddWithValue( "file_modul", data.file_modul );
                _comm.Parameters.AddWithValue( "lokasi_modul", filedir );

                _comm.CommandType = CommandType.Text;
                _conn.Open( );

                if ( Exists( loc ) ) //kalau folder sudah ada
                    File.WriteAllBytes( filedir, data.lokasi_modul );
                else
                {
                    CreateDirectory( loc );
                    File.WriteAllBytes( filedir, data.lokasi_modul );
                }

                return _comm.ExecuteNonQuery( );
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public int GetIDAbsensiPraktikan (AbsensiPraktikan data)
        {
            try
            {
                _comm.CommandText = @"SELECT	absensi_praktikan.id_absensi
                                      FROM      absensi_praktikan 
		                                        INNER JOIN 	jadwal_praktikan 
			                                        ON	absensi_praktikan.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan 
			                                        AND jadwal_praktikan.nrp = @nrp 
		                                        INNER JOIN 	jadwal_umum 
			                                        ON 	jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum 
		                                        INNER JOIN periode 
			                                        ON 	jadwal_umum.id_periode = periode.id_periode 
			                                        AND periode.id_periode = @id_periode 
		                                        INNER JOIN shift 
			                                        ON 	jadwal_umum.id_shift = shift.id_shift 
			                                        AND	shift.id_shift = @id_shift 
		                                        INNER JOIN mata_kuliah 
			                                        ON 	jadwal_umum.kode_mk = mata_kuliah.kode_mk 
			                                        AND mata_kuliah.kode_mk = @kode_mk 
		                                        INNER JOIN pertemuan 
			                                        ON 	absensi_praktikan.id_pertemuan = pertemuan.id_pertemuan 
			                                        AND pertemuan.id_jenis_pertemuan = @pertemuan";
                _comm.Parameters.AddWithValue("nrp",data.JadwalPraktikan.nrp);
                _comm.Parameters.AddWithValue("id_periode",data.JadwalPraktikan.id_jadwal_umum.id_periode);
                _comm.Parameters.AddWithValue("id_shift",data.JadwalPraktikan.id_jadwal_umum.id_shift);
                _comm.Parameters.AddWithValue("kode_mk",data.JadwalPraktikan.id_jadwal_umum.kode_mk);
                _comm.Parameters.AddWithValue("pertemuan",data.Pertemuan.id_jenis_pertemuan);
                _comm.CommandType = CommandType.Text;
                _conn.Open( );
                SqlDataReader reader = _comm.ExecuteReader( );

                var id = new int( );
                while ( reader.Read( ) )
                {
                    id = Convert.ToInt16( reader[0] );
                }
                return id;
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public int GetUpLoadFile (upload_file data)
        {
            const string dir = @"C:\LIK\USER\";
            string foo = dir + data.lokasi_file + @"\";
            string locationfile = foo + @"\" + data.nama_file;

            try
            {
                _comm.CommandText = @"INSERT INTO upload_file (id_absensi, lokasi_file)
                                      VALUES (@id_absensi, @lokasi_file)";

                _comm.Parameters.AddWithValue( "id_absensi", data.id_absensi );
                _comm.Parameters.AddWithValue( "lokasi_file", Format( "{0}{1}", foo, data.nama_file ) );

                _comm.CommandType = CommandType.Text;
                _conn.Open( );

                //cek folder user di server
                if ( Exists( foo ) ) //kalau folder sudah ada
                    File.WriteAllBytes( Format( "{0}{1}", foo, data.nama_file ), data.data_file );
                else
                {
                    CreateDirectory( foo );
                    File.WriteAllBytes( Format( "{0}{1}", foo, data.nama_file ), data.data_file );
                }

                return _comm.ExecuteNonQuery( );
            }
            catch ( Exception )
            {
                throw;
            }
            finally
            {
                _conn?.Close( );
            }
        }

        public List<AbsensiPraktikan> Nilai(jadwal_umum data)
        {
            try
            {
                var nilai = new List<AbsensiPraktikan>();
                _comm.CommandText = @"SELECT    jadwal_praktikan.nrp, absensi_praktikan.nilai, absensi_praktikan.id_pertemuan
                                      FROM      absensi_praktikan INNER JOIN
		                                        jadwal_praktikan ON absensi_praktikan.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan
                                      WHERE	    jadwal_praktikan.id_jadwal_umum = @id_jadwal_umum";
                _comm.Parameters.AddWithValue("id_jadwal_umum", data.id_jadwal_umum);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();

                while ( reader.Read() )
                {
                    var n = 0;
                    if ( DBNull.Value.Equals(reader[1]) )
                    {
                        n = 0;
                    }
                    else
                    {
                        n = Convert.ToInt32(reader[1]);
                    }

                    var list = new AbsensiPraktikan()
                    {
                        JadwalPraktikan = new jadwalPraktikan() { nrp = reader[0].ToString() },
                        Nilai = n,
                        Pertemuan = new pertemuan() { id_pertemuan = Convert.ToInt16(reader[2]) }
                    };
                    nilai.Add(list);
                }
                return nilai;
            }
            finally
            {
                _conn.Close();
            }
        }

        public List<praktikan> ListPraktikanPraktikum(jadwal_umum data)
        {
            try
            {
                var praktikan = new List<praktikan>();
                _comm.CommandText = @"SELECT	praktikan.nrp, praktikan.nama
                                      FROM	    jadwal_umum 
		                                        INNER JOIN 	jadwal_praktikan 
			                                        ON 		jadwal_umum.id_jadwal_umum = jadwal_praktikan.id_jadwal_umum 
			                                        AND 	jadwal_umum.id_periode = @id_periode 
		                                        INNER JOIN 	praktikan 
			                                        ON 		jadwal_praktikan.nrp = praktikan.nrp 
		                                        INNER JOIN 	mata_kuliah 
			                                        ON 		jadwal_umum.kode_mk = mata_kuliah.kode_mk 
			                                        AND 	mata_kuliah.mata_kuliah = @mata_kuliah";
                _comm.Parameters.AddWithValue("id_periode", data.id_periode);
                _comm.Parameters.AddWithValue("mata_kuliah", data.fk_jadwalUmum_matakuliah.mata_kuliah);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();

                while ( reader.Read() )
                {
                    var list = new praktikan()
                    {
                        NRP = reader[0].ToString().TrimEnd(),
                        Nama = reader[1].ToString().TrimEnd()
                    };
                    praktikan.Add(list);
                }
                return praktikan;
            }
            finally
            {
                _conn.Close();
            }
        }

        public List<upload_file> GetFileUjian(jadwalPraktikan data)
        {
            try
            {
                var file = new List<upload_file>();
                _comm.CommandText = @"SELECT	praktikan.nrp, praktikan.nama, upload_file.lokasi_file
                                      FROM  	upload_file 
		                                        INNER JOIN absensi_praktikan 
			                                        ON upload_file.id_absensi = absensi_praktikan.id_absensi 
		                                        INNER JOIN jadwal_praktikan 
			                                        ON absensi_praktikan.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan 
		                                        INNER JOIN jadwal_umum 
			                                        ON jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum 
			                                        AND jadwal_umum.id_periode = @id_periode 
		                                        INNER JOIN pertemuan 
			                                        ON absensi_praktikan.id_pertemuan = pertemuan.id_pertemuan 
		                                        INNER JOIN mata_kuliah 
			                                        ON jadwal_umum.kode_mk = mata_kuliah.kode_mk 
			                                        AND mata_kuliah.mata_kuliah = @mata_kuliah 
		                                        INNER JOIN praktikan 
			                                        ON jadwal_praktikan.nrp = praktikan.nrp
                                      WHERE  	pertemuan.id_jenis_pertemuan = @jenis_pertemuan";
                _comm.Parameters.AddWithValue("id_periode", data.id_jadwal_umum.id_periode);
                _comm.Parameters.AddWithValue("mata_kuliah", data.id_jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah);
                _comm.Parameters.AddWithValue("jenis_pertemuan", data.absen.Pertemuan.id_jenis_pertemuan);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();
                
                while ( reader.Read() )
                {
                    var info = new FileInfo(reader[2].ToString().TrimEnd());
                    var list = new upload_file()
                    {
                        jadwal = new AbsensiPraktikan()
                        {
                            JadwalPraktikan = new jadwalPraktikan()
                            {
                                praktikan = new praktikan()
                                {
                                    NRP = reader[0].ToString().TrimEnd(),
                                    Nama = reader[1].ToString().TrimEnd()
                                }
                            }
                        },
                        data_file = File.ReadAllBytes(reader[2].ToString().TrimEnd()),
                        nama_file = info.Name
                    };

                    file.Add(list);
                }
                return file;
            }
            finally
            {
                _conn.Close();
            }
        }

        public int InputMatkul(matkul data)
        {
            try
            {
                _comm.CommandText = @"INSERT INTO mata_kuliah (kode_mk, mata_kuliah)
                                      VALUES (@kode_mk, @mata_kuliah)";

                _comm.Parameters.AddWithValue("kode_mk", data.kode_mk);
                _comm.Parameters.AddWithValue("mata_kuliah", data.mata_kuliah);

                _comm.CommandType = CommandType.Text;
                _conn.Open();

                return _comm.ExecuteNonQuery();
            }
            catch ( Exception )
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int InputKelas(kelas data)
        {
            try
            {
                _comm.CommandText = @"INSERT INTO kelas (kelas)
                                      VALUES (@kelas)";

                _comm.Parameters.AddWithValue("kelas", data.Kelas);

                _comm.CommandType = CommandType.Text;
                _conn.Open();

                return _comm.ExecuteNonQuery();
            }
            catch ( Exception )
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int HapusPraktikan(Users data)
        {
            try
            {
                

                _comm.CommandText = @"DELETE FROM upload_file
                                      WHERE id_absensi IN(SELECT id_absensi
					                                    FROM absensi_praktikan
						                                    INNER JOIN jadwal_praktikan
						                                    ON jadwal_praktikan.id_jadwal_praktikan = absensi_praktikan.id_jadwal_praktikan
					                                    WHERE jadwal_praktikan.nrp = @username);
                                      DELETE FROM absensi_praktikan
                                      WHERE id_jadwal_praktikan IN( SELECT id_jadwal_praktikan
                                                                  FROM jadwal_praktikan
							                                      WHERE nrp =@username);
                                      DELETE FROM jadwal_praktikan WHERE nrp = @username;
                                      DELETE FROM praktikan WHERE nrp = @username;
                                      DELETE FROM users WHERE username = @username";
                _comm.Parameters.AddWithValue("username", data.username);

                _comm.CommandType = CommandType.Text;
                _conn.Open();

                var path = @"C:\LIK\USER\" + data.username;
                var dir = new DirectoryInfo(path);
                dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                dir.Delete(true);

                return _comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int HapusPJadwalAsisten(jadwalStaff data)
        {
            try
            {


                _comm.CommandText = @"DELETE FROM  jadwal_staff
                                      WHERE id_staff = @id_staff AND id_jadwal_umum = @id_jadwal_umum";
                _comm.Parameters.AddWithValue("id_staff", data.staff.id_staff);
                _comm.Parameters.AddWithValue("id_jadwal_umum", data.jadwal_umum.id_jadwal_umum);

                _comm.CommandType = CommandType.Text;
                _conn.Open();

                return _comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int HapusAngkatan(angkatan data)
        {
            try
            {


                _comm.CommandText = @"DELETE FROM angkatan
                                      WHERE tahun_angkatan = @tahun_angkatan";
                _comm.Parameters.AddWithValue("tahun_angkatan", data.TahunAngkatan);
                _comm.CommandType = CommandType.Text;
                _conn.Open();

                return _comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int HapusMataKuliah(matkul data)
        {
            try
            {


                _comm.CommandText = @"DELETE FROM mata_kuliah
                                      WHERE kode_mk = @kode_mk AND mata_kuliah = @mata_kuliah";
                _comm.Parameters.AddWithValue("kode_mk", data.kode_mk);
                _comm.Parameters.AddWithValue("mata_kuliah", data.mata_kuliah);
                _comm.CommandType = CommandType.Text;
                _conn.Open();

                return _comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public int HapusKelas(kelas data)
        {
            try
            {
                _comm.CommandText = @"DELETE FROM kelas
                                      WHERE kelas = @kelas";
                _comm.Parameters.AddWithValue("kelas", data.Kelas);
                _comm.CommandType = CommandType.Text;
                _conn.Open();

                return _comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn?.Close();
            }
        }

        

        public int jumPraktikum(modul data)
        {
            try
            {
                _comm.CommandText = @"SELECT    COUNT(kode_mk)
                                      FROM      modul
                                      WHERE     (kode_mk = @kode_mk)";
                _comm.Parameters.AddWithValue("kode_mk", data.matkul.kode_mk);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();

                var n = 0;
                while (reader.Read())
                {
                    n = Convert.ToInt16(reader[0]);
                }
                return n;
            }
            finally
            {
                _conn?.Close();
            }
        }

        public DateTime ServerTime()
        {
            var time_now = DateTime.Now;
            return time_now;
        }

        public List<AbsensiPraktikan> ambilNilaiPraktikan(AbsensiPraktikan data)
        {
            try
            {
                _comm.CommandText = @"SELECT        jadwal_praktikan.nrp, absensi_praktikan.nilai, pertemuan.id_pertemuan
                                    FROM            absensi_praktikan INNER JOIN
                                                             jadwal_praktikan ON absensi_praktikan.id_jadwal_praktikan = jadwal_praktikan.id_jadwal_praktikan INNER JOIN
                                                             jadwal_umum ON jadwal_praktikan.id_jadwal_umum = jadwal_umum.id_jadwal_umum AND jadwal_umum.id_periode = @id_periode INNER JOIN
                                                             pertemuan ON absensi_praktikan.id_pertemuan = pertemuan.id_pertemuan INNER JOIN
                                                             mata_kuliah ON jadwal_umum.kode_mk = mata_kuliah.kode_mk AND mata_kuliah.mata_kuliah = @mata_kuliah";
                _comm.Parameters.AddWithValue("id_periode", data.JadwalPraktikan.id_jadwal_umum.id_periode);
                _comm.Parameters.AddWithValue("mata_kuliah", data.JadwalPraktikan.id_jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah);
                _comm.CommandType = CommandType.Text;
                _conn.Open();
                SqlDataReader reader = _comm.ExecuteReader();

                var row = new List<AbsensiPraktikan>();
                while (reader.Read())
                {
                    var n = 0;
                    if (DBNull.Value.Equals(reader[1]))
                    {
                        n = 0;
                    }
                    else
                    {
                        n = Convert.ToInt32(reader[1]);
                    }
                    var read = new AbsensiPraktikan()
                    {
                        JadwalPraktikan = new jadwalPraktikan()
                        {
                            nrp = reader[0].ToString().TrimEnd()
                        },
                        Nilai = n,
                        Pertemuan = new pertemuan()
                        {
                            id_pertemuan = Convert.ToInt16(reader[2])
                        }
                    };
                    row.Add(read);
                }
                return row;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn.Close();
            }
        }

    }
}