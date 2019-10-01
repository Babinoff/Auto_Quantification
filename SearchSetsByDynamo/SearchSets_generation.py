# -*- coding: utf-8 -*-

#region-----------------------Импоорт библиотек----------------------
import clr, sys
sys.path.append('C:\Program Files (x86)\IronPython 2.7\Lib')
import string
from System import Guid
clr.AddReference("RevitServices")
from RevitServices.Persistence import DocumentManager

clr.AddReference('RevitAPI')
import Autodesk
from Autodesk.Revit.DB import BuiltInParameter
#endregion

#region-----------------------Функции----------------------		

#endregion

#region-----------------------Параметры----------------------	
doc = DocumentManager.Instance.CurrentDBDocument
elements_lists = UnwrapElement(IN[0])
folders_list = IN[1]
xml_base = IN[2]
xml_conditions = IN[3]
elems_parameters = IN[4]
cond_placment_to_modif = IN[5]

xml_part_ONE_folder1 = xml_base[0]
xml_part_TWO_folder2 = xml_base[1]
xml_part_THREE_SELECTION_SET__ = xml_base[2]

__FOLDER_1__ = folders_list[0]
folders_2 = folders_list[1]

condition_0 = doc.Title
condition_1 = elems_parameters[1]
condition_2 = elems_parameters[2]

__FOLDER_1_GUID__ = Guid.NewGuid().ToString()
#endregion

#region-----------------------Main----------------------	
selection_set = ""
folders_2_dict = {fold: set() for fold in folders_2}

test = []
xml_full = xml_part_ONE_folder1.replace("__FOLDER_1__",__FOLDER_1__)
xml_full = xml_full.replace("__FOLDER_1_GUID__",__FOLDER_1_GUID__)

xml_folder2_list = [xml_part_TWO_folder2.replace("__FOLDER_2__",fold).replace("__FOLDER_2_GUID__",Guid.NewGuid().ToString()) for fold in folders_2]

test_dict = {c2:set() for clist in condition_2 for c2 in clist }
[test_dict[c2].add(c1) for clist1,clist2 in zip(condition_1,condition_2) for c1,c2 in zip(clist1,clist2) ]

cat_list = []
selection_sets_list = []
for cond2 in test_dict.keys():
    xml_folder = xml_part_TWO_folder2.replace("__FOLDER_2__",cond2).replace("__FOLDER_2_GUID__",Guid.NewGuid().ToString())
    selection_sets_list = []
    for cond1 in test_dict[cond2]:
        cond_list = []
        for str_to_modf,cond in zip(cond_placment_to_modif,xml_conditions):
            if str_to_modf == "__FILE_TITLE__":
                cond_list.append(cond.replace(str_to_modf,condition_0))
            elif str_to_modf == "__TYPE_NAME__":
                cond_list.append(cond.replace(str_to_modf,cond1))
            elif str_to_modf == "__CATEGORY__":
                cond_list.append(cond.replace(str_to_modf,cond2))
        selection_sets_list.append(xml_part_THREE_SELECTION_SET__.replace("__SELECTIONSET_NAME__",cond1).replace("__SELECTIONSET_GUID__",Guid.NewGuid().ToString()).replace(" __CONDITIONS__",string.join(cond_list,"")))
    cat_list.append(xml_folder.replace("__SELECTION_SETS__",string.join(selection_sets_list,"")))
xml_full = xml_full.replace("__FOLDER_2_SET__",string.join(cat_list,""))
#endregion

OUT = xml_full