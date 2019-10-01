using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetQuantification
{
    public class ApiUtility
    {
        #region Классы
        public class Folder //класс папки, может содержать другие папки, или поисковые наборы
        {
            public List<Folder> SubFolder = new List<Folder>();// { get; }
            public string FolderName { get; set; }
            public List<SearchAndItems> SAIList = new List<SearchAndItems>();// { get; }
            public int ParentIndex { get; set; }
            public int SelfIndex { get; set; }

            public void WriteSubFolder(Folder sub_folder) { SubFolder.Add(sub_folder); }
            public void WriteFolderName(string _folder) { FolderName = _folder; }
            public void WriteSearchItems(SearchAndItems _search_and_items) { SAIList.Add(_search_and_items); }
        }

        public class SearchAndItems //класс с информацией из поискового набора
        {
            public string SearhSet { get; set; }
            public List<Guid> GuidList { get; set; }
            public string CategoryName { get; set; }
            public void WriteData(string _searchsetname, List<Guid> guid_list, string _catname)
            {
                SearhSet = _searchsetname;
                GuidList = guid_list;
                if (_catname != null) { CategoryName = _catname; }
                else { CategoryName = null; }
            }
        }
        #endregion

        public static Folder base_folder = new Folder(); //ключевая "папка" объект, в котором воспроизводится структура папок и поисковых наборов

        public static Folder WriteSelectionSetContent(SavedItem item, Folder parent) //вызываем из Main
        {
            Folder current = new Folder();
            if (base_folder.FolderName == null)
            {
                base_folder.WriteFolderName(item.DisplayName); base_folder.SelfIndex = 1; base_folder.ParentIndex = 0; //для наследования подпапок надо заменить base_folder.SelfIndex на индекс следующий за индексом последней папки
                current = base_folder;
            }
            else
            {
                if (item.IsGroup) { current.WriteFolderName(item.DisplayName); current.SelfIndex = base_folder.SelfIndex + (Params.q_group_count_new += 1); current.ParentIndex = parent.SelfIndex; }
            }
            if (!item.IsGroup)
            {
                current.WriteSearchItems(SAIsearch(item));
            }
            if (item.IsGroup)
            {
                foreach (SavedItem childItem in ((GroupItem)item).Children)
                {
                    if (item.IsGroup) { current.WriteSubFolder(WriteSelectionSetContent(childItem, current)); }
                    else { current.WriteSearchItems(SAIsearch(childItem)); }
                }
            }
            return current;
        }

        public static SearchAndItems SAIsearch(SavedItem item)
        {
            ModelItemCollection mod_col = ((SelectionSet)item).GetSelectedItems();
            SearchAndItems SAI = new SearchAndItems();
            List<Guid> output_guids = new List<Guid>();
            HashSet<string> unic_cats = new HashSet<string>();
            string cat = null;
            if (mod_col != null)
            {
                foreach (ModelItem mditem in mod_col)
                {
                    Guid item_guid = FindGuid(mditem);
                    if (item_guid.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        PropertyCategoryCollection mitem_property = mditem.GetUserFilteredPropertyCategories(); //свойства объекта для получения его категории, и поиска в категории окон и дверей, следующий номер за количеством папок в Q
                        DataProperty mitem_dataprop = mitem_property.FindPropertyByName("LcRevitData_Element", "LcRevitPropertyElementCategory"); //
                        if (mitem_dataprop != null) { unic_cats.Add(mitem_dataprop.Value.ToDisplayString()); }
                    }
                    output_guids.Add(item_guid);
                }
                if (unic_cats.Count == 1 && (unic_cats.Contains("Окна") || unic_cats.Contains("Двери"))) { cat = "ОкнаДвери"; }
                //#endregion

                SAI.WriteData(item.DisplayName, output_guids, cat);
            }
            return SAI;
        }

        public static Guid FindGuid(ModelItem mditem)
        {
            Guid item_guid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (mditem.Parent != null)
            {
                if (mditem.InstanceGuid.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    item_guid = FindGuid(mditem.Parent);
                }
                else
                {
                    item_guid = mditem.InstanceGuid;
                }
            }
            return item_guid;
        }

        public static void OutputToQuantification(Folder output_list) //вызываем из Main
        {
            if (output_list.FolderName != null)
            {
                Params.q_group_count += 1;
                if (Params.q_group_count == 1) { DatabaseUtility.InsertItemGroup(null, output_list.FolderName, "", Params.q_group_count.ToString()); }
                else { DatabaseUtility.InsertItemGroup(output_list.ParentIndex, output_list.FolderName, "", Params.q_group_count.ToString()); } //создание каталогов
            }
            if (output_list.SAIList.Count > 0)
            {
                int local_wbs = 1;
                foreach (SearchAndItems SAI in output_list.SAIList)
                {
                    if (SAI.CategoryName == "ОкнаДвери") { DatabaseUtility.InsertItemFormula(Params.q_group_count, SAI.SearhSet, output_list.FolderName, local_wbs.ToString()); } //создание записей с формулой
                    else { DatabaseUtility.InsertItem(Params.q_group_count, SAI.SearhSet, output_list.FolderName, local_wbs.ToString()); } //создание записей
                    foreach (Guid guid in SAI.GuidList)
                    {
                        CreateTakeoffLog(Params.q_element_count, guid); //запись элементов в файл для дальнейшего извлечения в квантификацию
                    }
                    local_wbs += 1;
                    Params.q_element_count += 1;
                }
            }
            if (output_list.SubFolder.Count > 0)
            {
                foreach (Folder folder in output_list.SubFolder)
                {
                    OutputToQuantification(folder);
                }
            }
            Params.Write_to_log_file();
        }

        public static void CreateTakeoffLog(long itemId, Guid modelItemGuid)
        {
            File.AppendAllText(Params.temp_guid_path, String.Format("{0},{1}|", itemId.ToString(), modelItemGuid.ToString(), Encoding.Unicode));
            if (modelItemGuid.ToString() != "00000000-0000-0000-0000-000000000000") { Params.guid_count += 1; }// else { Params.guid_count_0000 += 1; }
        }


        public static void FindItemFolderByName(SavedItem item, string base_search_set_folder) //вызываем из Main функция для парсинга папок, для поиска целевой папки
        {
            if (item.IsGroup)
            {
                foreach (SavedItem childItem in ((GroupItem)item).Children)
                {
                    if (childItem.IsGroup && childItem.DisplayName == base_search_set_folder)
                    {
                        WriteSelectionSetContent(childItem, ApiUtility.base_folder);
                        break;
                    }
                    else
                    {
                        FindItemFolderByName(childItem, base_search_set_folder);
                    }
                }
            }
        }


    }
}
