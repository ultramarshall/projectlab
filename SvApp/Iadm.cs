﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;

namespace SvApp
{
    [ServiceContract]
    public interface Iadm
    {
        [OperationContract]
        string GetLogin(akun data);

        [OperationContract]
        List<jadwalPraktikan> getTimeLogin(jadwalPraktikan data);

        [OperationContract]
        List<jadwalPraktikan> getPraktikanPraktikum(string nrp, string shift, int periode);

        [OperationContract]
        praktikan getProfilePraktikan(praktikan data);

        [OperationContract]
        int AddPraktikan(praktikan data);

        [OperationContract]
        int EditPraktikan(string nrp, praktikan data);

        [OperationContract]
        List<jurusan> GetJurusan();

        [OperationContract]
        List<angkatan> GetAngkatan();

        [OperationContract]
        List<praktikan> GetPraktikan(praktikan data);

        [OperationContract]
        int InsertMultiplePraktikan(List<praktikan> data);

        [OperationContract]
        List<jadwal_umum> ViewJadwalUmum(jadwal_umum data);

        [OperationContract]
        List<matkul> GetMatKul();

        [OperationContract]
        List<kelas> GetKelas();

        [OperationContract]
        List<Shift> GetShift();

        [OperationContract]
        int InsertJadwal(List<jadwal_umum> data);

        [OperationContract]
        int DeleteJadwal(jadwal_umum data);

        [OperationContract]
        List<Staff> getStaffID();

        [OperationContract]
        Staff getProfileStaff(Staff data);

        [OperationContract]
        List<jadwalStaff> GetStaffJadwal(jadwalStaff data);

        [OperationContract]
        List<jadwalStaff> jadwalUmumStaff(periode data);

        [OperationContract]
        int updateJadwalStaff(string id, jadwalStaff data);

        [OperationContract]
        int addPeriode(periode data);

        [OperationContract]
        List<periode> viewPeriode();

        [OperationContract]
        List<pertemuan> GetPertemuan(jadwalPraktikan data);

        [OperationContract]
        List<pertemuan> ListPertemuan();

        [OperationContract]
        int PostAbsenPraktikan(AbsensiPraktikan data);

        [OperationContract]
        List<praktikan> GetAbsensiPraktikans(AbsensiPraktikan data);

        [OperationContract]
        int AddStaff(Staff data);

        [OperationContract]
        int EditStaff(string id_staff, Staff data);

        [OperationContract]
        int KonfirmasiAbsensi(AbsensiPraktikan data);

        [OperationContract]
        int HapusAbsensi (AbsensiPraktikan data);

        [OperationContract]
        List<jadwalPraktikan> GetJadwalPraktikan (jadwalPraktikan data);

        [OperationContract]
        List<jadwalStaff> GetJadwalAsisten (jadwalStaff data);

        [OperationContract]
        int AddJadwalPraktikan (List<jadwalPraktikan> data);

        [OperationContract]
        int AddJadwalStaffAsisten (List<jadwalStaff> data);

        [OperationContract]
        int DeleteJadwalPraktikan (jadwalPraktikan data);

        [OperationContract]
        int TambahAngaktan (angkatan data);

        [OperationContract]
        modul GetModul (modul data);

        [OperationContract]
        List<modul> GetListModul (modul data);

        [OperationContract]
        int UploadModul (modul data);

        [OperationContract]
        int GetIDAbsensiPraktikan (AbsensiPraktikan data);

        [OperationContract]
        int GetUpLoadFile (upload_file data);

        [OperationContract]
        List<AbsensiPraktikan> Nilai(jadwal_umum data);

        [OperationContract]
        List<praktikan> ListPraktikanPraktikum(jadwal_umum data);

        [OperationContract]
        List<upload_file> GetFileUjian(jadwalPraktikan data);

        [OperationContract]
        int InputMatkul(matkul data);

        [OperationContract]
        int InputKelas(kelas data);

        [OperationContract]
        int HapusPraktikan(Users data);

        [OperationContract]
        int HapusPJadwalAsisten(jadwalStaff data);

        [OperationContract]
        int HapusAngkatan(angkatan data);

        [OperationContract]
        int HapusMataKuliah(matkul data);

        [OperationContract]
        int HapusKelas(kelas data);

        [OperationContract]
        DateTime ServerTime();

        [OperationContract]
        int jumPraktikum(modul data);

        [OperationContract]
        List<AbsensiPraktikan> ambilNilaiPraktikan(AbsensiPraktikan data);

        [OperationContract]
        int EditPassword(Users data);
    }
}