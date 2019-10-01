using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Data;
using Autodesk.Navisworks.Api.Takeoff;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GetQuantification
{
    class DatabaseUtility
    {
        #region SQL
        public static void ClaerSQL()
        {
            DocumentTakeoff docTakeoff = Autodesk.Navisworks.Api.Application.MainDocument.GetTakeoff();
            using (NavisworksTransaction trans = docTakeoff.Database.BeginTransaction(DatabaseChangedAction.Edited))
            {
                using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
                {
                    //use SELECT ... FROM ... WHERE ... sql for query.
                    //last_insert_rowid() is a stored function used to retrieve the rowid of the last insert row
                    cmd.CommandText = "DELETE FROM TK_Item";
                    cmd.ExecuteReader();
                }
                using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
                {
                    //use SELECT ... FROM ... WHERE ... sql for query.
                    //last_insert_rowid() is a stored function used to retrieve the rowid of the last insert row
                    cmd.CommandText = "DELETE FROM TK_ItemGroup";
                    cmd.ExecuteReader();
                }
                trans.Commit();
            }
        }
        public static void InsertItemGroup(long? parent, string name, string description, string wbs)
        {
            DocumentTakeoff docTakeoff = Autodesk.Navisworks.Api.Application.MainDocument.GetTakeoff();
            string sql = "INSERT INTO TK_ItemGroup(parent, name, description, wbs) VALUES(@parent, @name, @description, @wbs)";
            using (NavisworksTransaction trans = docTakeoff.Database.BeginTransaction(DatabaseChangedAction.Edited))
            {
                using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
                {
                    NavisworksParameter p = cmd.CreateParameter();
                    p.ParameterName = "@parent";
                    if (parent.HasValue)
                    {
                        p.Value = parent.Value;
                    }
                    else
                    {
                        p.Value = null;
                    }

                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@name";
                    p.Value = name;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@description";
                    p.Value = description;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@wbs";
                    p.Value = wbs;
                    cmd.Parameters.Add(p);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
        }
        public static void InsertItem(long? parent, string name, string description, string wbs)
        {
            //Debug.Assert(name != null);
            DocumentTakeoff docTakeoff = Autodesk.Navisworks.Api.Application.MainDocument.GetTakeoff();
            //ItemTable table = docTakeoff.Items;
            //Debug.Assert(table != null);

            //Directly operate on database
            //Database schema entry: TakeoffTable
            //INSERT INTO TABLE(COL1,COL2,COL3...) VALUES(V1,V2,V3...);
            string sql = "INSERT INTO TK_ITEM(parent, name, description, wbs, color, transparency, linethickness, countsymbol, countsize) VALUES(@parent, @name, @description,@wbs, @color, @transparency, @linethickness, @countsymbol, @countsize)";
            //Modification must be surrounded by NavisworksTransaction
            using (NavisworksTransaction trans = docTakeoff.Database.BeginTransaction(DatabaseChangedAction.Edited))
            {
                using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
                {
                    NavisworksParameter p = cmd.CreateParameter();
                    p.ParameterName = "@parent";
                    if (parent.HasValue)
                    {
                        p.Value = parent.Value;
                    }
                    else
                    {
                        p.Value = null;
                    }

                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@name";
                    p.Value = name;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@description";
                    p.Value = description;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@wbs";
                    p.Value = wbs;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@color";
                    p.Value = 24;// 24, 0.5, 0.1, 1, 2
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@transparency";
                    p.Value = 0.5;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@linethickness";
                    p.Value = 0.1;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@countsymbol";
                    p.Value = 1;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@countsize";
                    p.Value = 2;
                    cmd.Parameters.Add(p);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
        }
        public static void InsertItemFormula(long? parent, string name, string description, string wbs)
        {
            //Debug.Assert(name != null);
            DocumentTakeoff docTakeoff = Autodesk.Navisworks.Api.Application.MainDocument.GetTakeoff();
            //ItemTable table = docTakeoff.Items;
            //Debug.Assert(table != null);

            //Directly operate on database
            //Database schema entry: TakeoffTable
            //INSERT INTO TABLE(COL1,COL2,COL3...) VALUES(V1,V2,V3...);
            string sql = "INSERT INTO TK_ITEM(parent, name, description, wbs, color, transparency, linethickness, countsymbol, countsize, area_formula) VALUES(@parent, @name, @description,@wbs, @color, @transparency, @linethickness, @countsymbol, @countsize, @area_formula)";
            //Modification must be surrounded by NavisworksTransaction
            using (NavisworksTransaction trans = docTakeoff.Database.BeginTransaction(DatabaseChangedAction.Edited))
            {
                using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
                {
                    NavisworksParameter p = cmd.CreateParameter();
                    p.ParameterName = "@parent";
                    if (parent.HasValue)
                    {
                        p.Value = parent.Value;
                    }
                    else
                    {
                        p.Value = null;
                    }

                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@name";
                    p.Value = name;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@description";
                    p.Value = description;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@wbs";
                    p.Value = wbs;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@color";
                    p.Value = 24;// 24, 0.5, 0.1, 1, 2
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@transparency";
                    p.Value = 0.5;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@linethickness";
                    p.Value = 0.1;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@countsymbol";
                    p.Value = 1;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@countsize";
                    p.Value = 2;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = "@area_formula";
                    p.Value = "=height*width";
                    cmd.Parameters.Add(p);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
        }
        public static long GetLastInsertRowId()
        {
            DocumentTakeoff docTakeoff = Autodesk.Navisworks.Api.Application.MainDocument.GetTakeoff();
            using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
            {
                //use SELECT ... FROM ... WHERE ... sql for query.
                //last_insert_rowid() is a stored function used to retrieve the rowid of the last insert row
                cmd.CommandText = "select last_insert_rowid()";
                using (NavisWorksDataReader dataReader = cmd.ExecuteReader())
                {
                    long lastId = -1;
                    if (dataReader.Read())
                    {
                        long.TryParse(dataReader[0].ToString(), out lastId);
                    }
                    return lastId;
                }
            }
        }
        public static void DoTakeoff0(long itemId, Guid modelItemGuid, int wbs_number) //вызываем из Main 
        {
            DocumentTakeoff docTakeoff = Autodesk.Navisworks.Api.Application.MainDocument.GetTakeoff();
            List<ModelItem> items = Autodesk.Navisworks.Api.Application.MainDocument.Models.RootItemDescendantsAndSelf.WhereInstanceGuid(modelItemGuid).ToList();
            //Int64 lastId = -1;
            if (items.Count != 0)
            {
                using (NavisworksTransaction trans = docTakeoff.Database.BeginTransaction(DatabaseChangedAction.Edited))
                {
                    long lastId = GetLastInsertRowId();
                    docTakeoff.Objects.InsertModelItemTakeoff(itemId, items[0]);
                    //TakeoffVariableCollection variable_сollection = docTakeoff.Objects.CreateDefaultInputVariables();//docTakeoff.Objects.SelectInputVariables(itemId);
                    //docTakeoff.Objects.InsertModelItemTakeoff(itemId, modelItemGuid, variable_сollection, false); //использование метода с VariableCollection даёт пустые значения данных в квантификатион
                    //Quantification UI actually expect the takeoff to have a non-empty wbs, so better to set the wbs for it using the sql way

                    //ApiUtility.fail_log.Add("lastId"+lastId.ToString());
                    //ApiUtility.Write_to_log_file();
                    //Debug.Assert(lastId > 0); //проверить необходимость
                    using (NavisworksCommand cmd = docTakeoff.Database.Value.CreateCommand())
                    {
                        //UPDATE Object set WBS = value WHERE id = lastId;
                        cmd.CommandText = "UPDATE TK_OBJECT SET wbs = @wbs WHERE id = @id";
                        NavisworksParameter p = cmd.CreateParameter();
                        p.ParameterName = "@wbs";
                        p.Value = wbs_number.ToString();// последнее цифра в индексе wbs
                        cmd.Parameters.Add(p);

                        p = cmd.CreateParameter();
                        p.ParameterName = "@id";
                        p.Value = lastId;
                        cmd.Parameters.Add(p);

                        cmd.ExecuteNonQuery();
                    }
                    //docTakeoff.Objects.UpdateInputVariables(itemId, variable_сollection);
                    trans.Commit();
                    
                }
            }
        }
        #endregion
    }
}
