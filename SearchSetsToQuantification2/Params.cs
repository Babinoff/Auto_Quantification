
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetQuantification
{
    class Params
    {
        #region Параметры логов
        public static string temp_folder = Path.GetTempPath() + @"\NW_QUNTIFICATION_LOG\"; //"%temp%";
        public static string fail_log_path = temp_folder + "FAIL_LOG_Navisworks_SearchsToQuantification.log";
        public static string temp_guid_path = temp_folder + "DATA_NWGUID_SearchsToQuantification.log";
        //public static List<string> log_guid_list = new List<string>();
        public static List<string> fail_log = new List<string>();
        #endregion

        #region Параметры подсчёта элементов
        //public static int count = 0;
        public static int guid_count = 0;
        //public static int guid_count_0000 = 0;
        public static int q_group_count = 0;
        public static int q_group_count_new = 0;
        public static int q_element_count = 1;
        #endregion

        #region Параметры базового класса хранилища для структуры папок и поисковых элементов
        //public static Folder base_folder = new Folder();
        #endregion

        #region Методы параметров
        public static void ClearParams()
        {
            File.Delete(temp_guid_path);
            File.Create(temp_guid_path).Close();
            File.Delete(fail_log_path);
            File.Create(fail_log_path).Close();
            fail_log = new List<string>();
            //fail_log.Add("");
            ApiUtility.base_folder = new ApiUtility.Folder();
            guid_count = 0;
            //guid_count_0000 = 0;
            q_group_count = 0;
            q_group_count_new = 0;
            q_element_count = 1; //для наследования правильнйо структуры загружаемых элементов номер должен быть следующим за количеством созданых элементов в Q
        }
        public static void Write_to_log_file()
        {
            File.AppendAllLines(fail_log_path, fail_log, Encoding.Unicode);
        }
        #endregion
    }
}
