using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Text;
using Autodesk.Navisworks.Api.Takeoff;

namespace GetQuantification
{
    [Plugin("Auto_Q", "ADSK", DisplayName = "Генерация Quantification")]
    [RibbonLayout("SearchSetsToQuantification2.xaml")]
    [RibbonTab("Volumes", DisplayName = "Авто Q")]
    [Command("ID_Button_5", DisplayName = "Генерация Quantification", Icon = "q16.png", LargeIcon = "q32.png", ToolTip = "Создание таблицы квантификации на основе поисковых наборов. V 2.1", CanToggle = true)]

    public class Main : CommandHandlerPlugin
    {

        public override int ExecuteCommand(string name, params string[] parameters)
        {
            try
            {
                if (!Directory.Exists(Params.temp_folder)) { Directory.CreateDirectory(Params.temp_folder); }
                string base_search_set_folder = Microsoft.VisualBasic.Interaction.InputBox("Ввести имя папки с поисковыми запросами:", "Выбор папки", "Папка"); //папка задание

                if (base_search_set_folder == "")
                    return 0;

                var time_count = System.Diagnostics.Stopwatch.StartNew();
                Params.ClearParams();
                DatabaseUtility.ClaerSQL(); //выключить для сохранения квантификации

                foreach (SavedItem item in Autodesk.Navisworks.Api.Application.MainDocument.SelectionSets.RootItem.Children)
                    if (item.IsGroup && item.DisplayName == base_search_set_folder) //поиск папки по заданию в корневой деректории
                    {
                        ApiUtility.WriteSelectionSetContent(item, ApiUtility.base_folder);
                        break;
                    }
                    else
                    {
                        ApiUtility.FindItemFolderByName(item, base_search_set_folder); //парсинг внутренних деректорий для поиска папки
                    }
                //DatabaseUtility.ClaerSQL_folder(base_search_set_folder);

                ApiUtility.OutputToQuantification(ApiUtility.base_folder); //тут создаём структуру папок и элементов квантификации, записываем гуиды в темпфайл

                string[] guids_strings = File.ReadAllText(Params.temp_guid_path).Split('|'); //разделяем текст на ячейки индекс,гуид

                string wbs_last_integer_in_index = "0";
                int wbs_counter = 0;
                foreach (string gs in guids_strings)
                {
                    string[] gs_2_list = gs.Split(','); //в [0] попадает индекс, в [1] гуид
                    if (gs_2_list[0] != "")
                    {
                        if (gs_2_list[0] != wbs_last_integer_in_index)
                        {
                            wbs_last_integer_in_index = gs_2_list[0];
                            wbs_counter = 1;
                            DatabaseUtility.DoTakeoff0(long.Parse(gs_2_list[0], NumberStyles.Integer), Guid.Parse(gs_2_list[1]), wbs_counter); //здесь в структуру квантификации добавляем элементы
                        }
                        else
                        {
                            wbs_counter += 1;
                            DatabaseUtility.DoTakeoff0(long.Parse(gs_2_list[0], NumberStyles.Integer), Guid.Parse(gs_2_list[1]), wbs_counter); //здесь в структуру квантификации добавляем элементы

                        }
                    }
                }
                time_count.Stop();
                MessageBox.Show(("Elements (guid) counter: " + Params.guid_count.ToString()) + Environment.NewLine + "Time counter: Minutes " + time_count.Elapsed.Minutes.ToString() + ", Seconds " + time_count.Elapsed.Seconds);
                Params.fail_log.Add(String.Format("q_group_count: {0}, q_element_count: {1}, q_group_count_new: {2}|", Params.q_group_count, Params.q_element_count, Params.q_group_count_new, Encoding.Unicode));
                Params.Write_to_log_file();
                return 1;
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                StackTrace st = new StackTrace(ex, true);
                // Get the top stack frame
                StackFrame frame = st.GetFrame(0);
                // Get the line number from the stack frame
                int line = frame.GetFileLineNumber();
                Params.fail_log.Add(line.ToString() + "__" + ex.ToString());
                Params.Write_to_log_file();
                return 0;
            }
        }
    }
}
