using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using app_c.mma.rdi;
using system.block.item;
using system.block.uda.dataSource;
using NLog;
using app.mma.utils;

namespace app.mma {
	
	//****sleeping sheep***


	public class Test {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		private static void Config() {
			Log.Log(LogLevel.Info, "Reading configs");
			NameValueCollection nvc = (NameValueCollection) ConfigurationSettings.GetConfig("appSettings");

			foreach (string s in nvc.Keys) {
				string keyValue = nvc[s];

				switch (s) {
					case "dbdriver":
						system.block.uda.dataSource.RawDataItemDB_c.DbDriver = keyValue;
						Log.Log(LogLevel.Info, "Config setting \"" + s + "\" set to \"" + keyValue + "\"");
						break;
					case "credentials":
						system.block.uda.dataSource.RawDataItemDB_c.DbCredentials = keyValue;
						system.block.uda.dataSource.RawDataItem_c.DbCredentials = keyValue;
						system.block.uda.dataSource.RawDataItem_c.FillCredentials();
						// Log.Log(LogLevel.Info, "Config setting \"" + s + "\" set to \"" + keyValue + "\"");
						break;
				}
			}

			Log.Log(LogLevel.Info, "Finished reading configs");
		}

		private static void Test1() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (enttentity_c) enttentity_c.getInstance(oApp.context);
			string critName = "id";

			long iCritID = obj.getCritID(critName);
			Log.Log(LogLevel.Info, iCritID);

	/*
			RdiDotNetSQLInstance_c obj1 = (RdiDotNetSQLInstance_c) obj[(int) iCritID] ;
			Log.Log(LogLevel.Info, "obj1 is " + (obj1 == null ? "" : "NOT ") + "NULL");
			obj1.search(new object[] {1});
			obj1.refresh();
*/
			obj.search(critName, new object[] {1});
			obj.refresh(critName);			
			long rowsCount = obj.getDataCount(iCritID);
			Log.Log(LogLevel.Info, "Rows count: " + rowsCount);

			long[] fieldsIds = obj.getSearchCriteriaFields(iCritID);
			int fieldsCount = fieldsIds.Length;

			foreach (long fieldId in fieldsIds)
				Log.Log(LogLevel.Info, "Field Id: " + fieldId);

			string[] fieldsNames = obj.getSearchCriteriaFieldNames(iCritID);

			foreach (string fieldName in fieldsNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);


			foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			}

			for (long i = 0; i < rowsCount; i++) {
				for (int j = 0; j < fieldsCount; j++) {
					object o = obj.getO(i, fieldsIds[j], iCritID);
					Log.Log(LogLevel.Info, fieldsNames[j] + ": " + o);
				}
			}
/*
			string Name = obj.GetType().Name;
			string fieldRecID = Name.Substring(0, 4)+"recid";
			obj.search("id", new object[]{ parameters[0] });
			obj.refresh("id");
*/
		}

		private static void Test2() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (enttentity_c) enttentity_c.getInstance(oApp.context);
			string critName = "id";
			long iCritID = obj.getCritID(critName);
			Log.Log(LogLevel.Info, iCritID);

			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) obj[(int) iCritID];
			long[] addNewParamsIds = obj.getAddCriteriaParams(iCritID);
			int paramsCount = addNewParamsIds.Length;
			Log.Log(LogLevel.Info, "paramsCount: " + paramsCount);

			foreach (long paramId in addNewParamsIds)
				Log.Log(LogLevel.Info, "Param Id: " + paramId);

			string[] fieldNames = obj.getAddCriteriaParamNames(iCritID);

			foreach (string fieldName in fieldNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

			Hashtable paramsData = new Hashtable();
			paramsData["enttnm"] = "Entt Name 3";
			paramsData["enttnode_nodeid"] = 2;
			paramsData["enttsnm"] = "Entt Snm 3";
			// paramsData["enttsmt"] = 0.2D;
			paramsData["enttgender"] = "m";
			paramsData["enttinfo_eninid"] = 2;
			paramsData["enttlang_langid"] = 1;
			paramsData["enttkeywords"] = "Entt Name 3 Keywords";
			paramsData["enttpic_docuid"] = 2;

			int elemsCount = obj.elemInfo.Count;
			object[] elemValues = new object[elemsCount];

/*
			foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			}
*/

			for (int i = 0; i < paramsCount; i++) {
				string fieldName = fieldNames[i];
				elemValues[addNewParamsIds[i]] = paramsData[fieldName];
			}

			obj.addNew(iCritID, elemValues);
		}

		private static void Test3() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (enttentity_c) enttentity_c.getInstance(oApp.context);
			string critName = "id";
			long iCritID = obj.getCritID(critName);
			Log.Log(LogLevel.Info, iCritID);

			// RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) obj[(int) iCritID];

			long[] deleteParamsIds = obj.getDeleteCriteriaParams(iCritID);
			int paramsCount = deleteParamsIds.Length;
			Log.Log(LogLevel.Info, "paramsCount: " + paramsCount);

			foreach (long paramId in deleteParamsIds)
				Log.Log(LogLevel.Info, "Param Id: " + paramId);

			string[] fieldNames = obj.getDeleteCriteriaParamNames(iCritID);

			foreach (string fieldName in fieldNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

			Hashtable paramsData = new Hashtable();
			paramsData["enttid"] = 6;
			paramsData["enttnm"] = "Entt Name 3";
			paramsData["enttnode_nodeid"] = 2;
			paramsData["enttsnm"] = "Entt Snm 3";
			paramsData["enttsmt"] = 0.2D;
			paramsData["enttgender"] = "m";
			paramsData["enttinfo_eninid"] = 2;
			paramsData["enttlang_langid"] = 1;
			paramsData["enttkeywords"] = "Entt Name 3 Keywords";
			paramsData["enttpic_docuid"] = 2;

			int elemsCount = obj.elemInfo.Count;
			object[] elemValues = new object[elemsCount];

/*
			foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			}
*/

			for (int i = 0; i < paramsCount; i++) {
				string fieldName = fieldNames[i];
				elemValues[deleteParamsIds[i]] = paramsData[fieldName];
			}

			obj.delete(iCritID, elemValues);
		}

		private static void Test4() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (enttentity_c) enttentity_c.getInstance(oApp.context);
			string critName = "id";
			long iCritID = obj.getCritID(critName);
			Log.Log(LogLevel.Info, iCritID);
			Log.Log(LogLevel.Info, "search");

			#region Search
			obj.search(critName, new object[] {1});
			obj.refresh();

			long[] searchFieldsIds = obj.getSearchCriteriaFields(iCritID);
			int searchFieldsCount = searchFieldsIds.Length;

			foreach (long fieldId in searchFieldsIds)
				Log.Log(LogLevel.Info, "Field Id: " + fieldId);

			string[] searchFieldsNames = obj.getSearchCriteriaFieldNames(iCritID);

			foreach (string fieldName in searchFieldsNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

/*
			for (int i = 0; i < searchFieldsCount; i++) {
				string fieldName = searchFieldsNames[i];
				o = obj.getO(0, fieldName, iCritID);
			}
*/
			#endregion

			#region Modify

			// RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) obj[(int) iCritID];

			Log.Log(LogLevel.Info, "modify");

			long[] modifyFieldsIds = obj.getUpdateCriteriaFields(iCritID); // obj.getUpdateCriteriaAllParams(iCritID); // getUpdateCriteriaFields(iCritID);
			int fieldsCount = modifyFieldsIds.Length;
			Log.Log(LogLevel.Info, "fieldsCount: " + fieldsCount);

			foreach (long fieldId in modifyFieldsIds)
				Log.Log(LogLevel.Info, "Field Id: " + fieldId);

			long[] modifyParamsIds = obj.getUpdateCriteriaParams(iCritID); // obj.getUpdateCriteriaAllParams(iCritID); // getUpdateCriteriaFields(iCritID);
			int paramsCount = modifyParamsIds.Length;
			Log.Log(LogLevel.Info, "paramsCount: " + paramsCount);

			foreach (long paramId in modifyParamsIds)
				Log.Log(LogLevel.Info, "Param Id: " + paramId);

			string[] fieldNames = obj.getUpdateCriteriaFieldNames(iCritID);

			foreach (string fieldName in fieldNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

			string[] paramNames = obj.getUpdateCriteriaParamNames(iCritID);

			foreach (string paramName in paramNames)
				Log.Log(LogLevel.Info, "Param Name: " + paramName);

			Hashtable paramsData = new Hashtable();
			paramsData["enttid"] = 2;
			paramsData["enttnm"] = "Entt Name 2.1";
			paramsData["enttnode_nodeid"] = 2;
			paramsData["enttsnm"] = "Entt Snm 2.1";
			paramsData["enttsmt"] = 0.2D;
			// paramsData["enttgender"] = "m";
			paramsData["enttinfo_eninid"] = 2;
			paramsData["enttlang_langid"] = 1;
			paramsData["enttkeywords"] = "Entt Name 2.1 Keywords";
			paramsData["enttpic_docuid"] = 2;

			int elemsCount = obj.elemInfo.Count;
			Log.Log(LogLevel.Info, "elemsCount: " + elemsCount);
			object[] elemValues = new object[elemsCount];

/*
			foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			}
*/

			for (int i = 0; i < fieldsCount; i++) {
				string fieldName = fieldNames[i];

				if (paramsData.ContainsKey(fieldName))
					elemValues[modifyFieldsIds[i]] = paramsData[fieldName];
				else {
					Log.Log(LogLevel.Info, "Taking " + fieldName + " value from db");
					elemValues[modifyFieldsIds[i]] = obj.getO(0, fieldName, iCritID);
				}
			}

			for (int i = 0; i < paramsCount; i++) {
				string paramName = paramNames[i];
				elemValues[modifyParamsIds[i]] = paramsData[paramName];

			}

			obj.modify(iCritID, elemValues);
			#endregion
		}

		private static void Test5() {
			// Log.Log(LogLevel.Info, RetCdeMap.Msg(RetCde.Success, 0));
			// RDISplImplRWM rdi = new RDISplImplRWM();
			object[] io = new object[2];
			Ent ent = new Ent();
			ent.s(io);
			object[,] res = (object[,]) io[1];

			Log.Log(LogLevel.Info, res[1,1]);
		}

		private static void Test6() {
			Log.Log(LogLevel.Info, "Test 6");
			// Log.Log(LogLevel.Info, RetCdeMap.Msg(RetCde.Success, 0));
			// RDISplImplRWM rdi = new RDISplImplRWM();
			object[] io = new object[2];
			Geo geo = new Geo();
			geo.s(io);
			object[,] res = (object[,]) io[1];

			Log.Log(LogLevel.Info, res[1,1]);
		}

		private static void Test7() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (geolgeolocation_c) geolgeolocation_c.getInstance(oApp.context);
			string critName = "allcountries";

			long iCritID = obj.getCritID(critName);
			Log.Log(LogLevel.Info, iCritID);

			obj.search(critName, new object[] {});
			obj.refresh(critName);
			long rowsCount = obj.getDataCount(iCritID);
			Log.Log(LogLevel.Info, "Rows count: " + rowsCount);

			long[] fieldsIds = obj.getSearchCriteriaFields(iCritID);
			int fieldsCount = fieldsIds.Length;

			foreach (long fieldId in fieldsIds)
				Log.Log(LogLevel.Info, "Field Id: " + fieldId);

			string[] fieldsNames = obj.getSearchCriteriaFieldNames(iCritID);

			foreach (string fieldName in fieldsNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);


			foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			}

			// for (long i = 0; i < rowsCount; i++) {
			for (long i = 0; i < 5; i++) {
				Log.Log(LogLevel.Info, "--- Row: " + (i + 1).ToString() + " ---");

				for (int j = 0; j < fieldsCount; j++) {
					object o = obj.getO(i, fieldsIds[j], iCritID);
					Log.Log(LogLevel.Info, fieldsNames[j] + ": " + o);
				}
			}
		}

		private static void Test8() {
			Log.Log(LogLevel.Info, "Test 8");
			Geo geo = new Geo();
			string alias = "listterritory";
			string criterion = "allcountries";
			ArrayList parameters = new ArrayList();
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;

			object[] io = new object[] {alias, criterion, parameters, offset, limit, fieldNames, null};
			geo.s(io);
			Log.Log(LogLevel.Info, "Rows: " + ((object[,]) io[io.Length - 1]).GetLength(0).ToString());
			Log.Log(LogLevel.Info, "Columns: " + ((object[,]) io[io.Length - 1]).GetLength(1).ToString());
			Log.Log(LogLevel.Info, ((object[,]) io[io.Length - 1])[0, 1].ToString());
		}

		private static void Test9() {
			Log.Log(LogLevel.Info, "Test 9");
			Chnl rdi = new Chnl();
			string alias = "listprofile";
			string criterion = "userprofiles";
			UserInfo userInfo = new UserInfo() {
				UserId = 2L,
				EntityId = 806L
			};
			ArrayList parameters = new ArrayList();
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;

			object[] io = new object[] {alias, criterion, parameters, offset, limit, fieldNames, userInfo, null};
			rdi.user_profiles(io);
			Log.Log(LogLevel.Info, "Rows: " + ((object[,]) io[io.Length - 1]).GetLength(0).ToString());
			Log.Log(LogLevel.Info, "Columns: " + ((object[,]) io[io.Length - 1]).GetLength(1).ToString());
			Log.Log(LogLevel.Info, ((object[,]) io[io.Length - 1])[0, 1].ToString());
			Log.Log(LogLevel.Info, ((object[,]) io[io.Length - 1])[1, 1].ToString());
		}

		private static void Test10() {
			Log.Log(LogLevel.Info, "Test 10");
			MmaEnt rdi = new MmaEnt();
			string alias = "userrecords";
			string criterion = "getfrfollbygeo";
			string userid = "2";
			string chnlid = "1";
			string edgepriv = "1";
			string geolid = "230";
			string geolr = "100";
			string ageMin = "0";
			string ageMax = "120";
			string gender = "0";
			ArrayList parameters = new ArrayList {userid, chnlid, edgepriv, geolid, geolr, ageMin, ageMax, gender};
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;

			object[] io = new object[] {alias, criterion, parameters, offset, limit, fieldNames, null};
			RetCde retCode = rdi.s(io);

			if (retCode != RetCde.Success) {
				Log.Log(LogLevel.Error, "Error: rdi.s() function returned error code: " + retCode.ToString());
				return;
			}

			object[,] data = (object[,]) io[io.Length - 1];
			int rowsCount = data.GetLength(0);
			Log.Log(LogLevel.Info, "Rows: " + (rowsCount - 1));
			/*
			for (long i = 0; i < rowsCount; i++) {
				for (int j = 0; j < colsCount; j++) {
					Console.Write(data [i, j]);
					Console.Write("\t");
				}

				Console.WriteLine();
			}*/
		}

		private static void Test12() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (msgqmessagequeue_c) msgqmessagequeue_c.getInstance(oApp.context);

			string critName = "id";
			long iCritID = obj.getCritID(critName);
			Log.Log(LogLevel.Info, iCritID);

			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) obj[(int) iCritID];
			long[] addNewParamsIds = obj.getAddCriteriaParams(iCritID);
			int paramsCount = addNewParamsIds.Length;
			Log.Log(LogLevel.Info, "paramsCount: " + paramsCount);

			foreach (long paramId in addNewParamsIds)
				Log.Log(LogLevel.Info, "Param Id: " + paramId);

			string[] fieldNames = obj.getAddCriteriaParamNames(iCritID);

			foreach (string fieldName in fieldNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

			DateTime msgqDate = DateTime.UtcNow;

			Hashtable paramsData = new Hashtable();
			//paramsData["msgqrecid"] = 2L;
			//paramsData["msgqid"] = 3L;
			paramsData["msgqprov_enttid"] = 2L;
			paramsData["msgqtarg_enttid"] = 2;
			paramsData["msgqsrc"] = "{1,2,3}";
			paramsData["msgqtyp"] = 1;
			paramsData["msgqtext"] = "Test message";
			paramsData["msgqdoc_docuid"] = 4L;
			paramsData["msgqdatestart"] = msgqDate;
			paramsData["msgqdateend"] = msgqDate.AddDays(5);
			paramsData["msgqstatus"] = 5;
			paramsData["msgqtransaction_tranid"] = 6L;
			paramsData["msgqactive"] = true;

			int elemsCount = obj.elemInfo.Count;
			object[] elemValues = new object[elemsCount];

			foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			}

			for (int i = 0; i < paramsCount; i++) {
				string fieldName = fieldNames[i];
				elemValues[addNewParamsIds[i]] = paramsData[fieldName];
			}

			obj.addNew(iCritID, elemValues);
		}

		private static void Test13() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[]{});
			RDISplImplRWM_c obj = (msgqmessagequeue_c) msgqmessagequeue_c.getInstance(oApp.context);

			string critName = "id";
			long iCritID = obj.getCritID(critName);

			Log.Log(LogLevel.Info, iCritID);
			Log.Log(LogLevel.Info, "search");

			#region Search
			obj.search(critName, new object[] {1});
			obj.refresh();

			long[] searchFieldsIds = obj.getSearchCriteriaFields(iCritID);
			int searchFieldsCount = searchFieldsIds.Length;
			/*
			foreach (long fieldId in searchFieldsIds)
				Log.Log(LogLevel.Info, "Field Id: " + fieldId);

	
			string[] searchFieldsNames = obj.getSearchCriteriaFieldNames(iCritID);

			foreach (string fieldName in searchFieldsNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

			for (int i = 0; i < searchFieldsCount; i++) {
				string fieldName = searchFieldsNames[i];
				object o = obj.getO(0, fieldName, iCritID);
				Console.WriteLine(fieldName + ": " + o);
			}
			*/
			#endregion

			#region Modify

			// RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) obj[(int) iCritID];
			long[] addNewParamsIds = obj.getAddCriteriaParams(iCritID);
			int paramsCount = addNewParamsIds.Length;
			Log.Log(LogLevel.Info, "paramsCount: " + paramsCount);

			foreach (long paramId in addNewParamsIds)
				Log.Log(LogLevel.Info, "Param Id: " + paramId);

			string[] fieldNames = obj.getAddCriteriaParamNames(iCritID);

			foreach (string fieldName in fieldNames)
				Log.Log(LogLevel.Info, "Field Name: " + fieldName);

			DateTime msgqDate = DateTime.UtcNow;

			Hashtable paramsData = new Hashtable();
			/* paramsData["msgqrecid"] = 1L;
			paramsData["msgqid"] = 1L;
			paramsData["msgqprov_enttid"] = 2L;
			*/
			paramsData["msgqtarg_enttid"] = 42;
			/*
			paramsData["msgqsrc"] = "{1,2,3}";
			paramsData["msgqtyp"] = 3;
			paramsData["msgqtext"] = "Test message";
			paramsData["msgqdoc_docuid"] = 4L;
			paramsData["msgqdatestart"] = msgqDate;
			paramsData["msgqdateend"] = msgqDate.AddDays(5);
			paramsData["msgqstatus"] = 5;
			paramsData["msgqtransaction_tranid"] = 6L;
			paramsData["msgqactive"] = true;
			*/

			/* int elemsCount = obj.elemInfo.Count;
			Console.WriteLine("obj.elemInfo.Count: " + elemsCount);*/
			object[] paramValues = new object[paramsCount];

			/* foreach (ElemInfo elem in obj.elemInfo) {
				Log.Log(LogLevel.Info, "Elem Name: " + elem.name);
			} */

			for (int i = 0; i < paramsCount; i++) {
				string fieldName = fieldNames[i];

				if (fieldName.EndsWith("recid") || fieldName.EndsWith("transaction_tranid"))
					continue;

				object o;

				if (paramsData.ContainsKey(fieldName)) {
					o = paramsData[fieldName];
				} else {
					o = obj.getO(0, fieldName, iCritID);
				}

				paramValues[i] = o;
				Console.WriteLine(fieldName + ": " + o);
			}

			obj.modify(iCritID, paramValues);
			#endregion
		}

		private static void Test14() {
			Console.WriteLine("Test 14");
			Log.Log(LogLevel.Info, "Test 14");
			MmaEnt mmaEnt = new MmaEnt();
			string targEnt = "{1,2,3}";
			string targSrc = "{3,2,1}";
			string prov = "1";
			string msgTyp = "2";
			string text = "Test Message";
			string doc = null;
			string dateTimeStringFormat = "yyyy-MM-dd HH:mm:ss";
			DateTime now = DateTime.UtcNow;
			string dateStart = now.ToString(dateTimeStringFormat);
			string dateEnd = now.AddDays(5).ToString(dateTimeStringFormat);
			string[] parameters = new string[] {targEnt, targSrc, prov, msgTyp, text, doc, dateStart, dateEnd};

			object[] io = new object[] {parameters, null};
			mmaEnt.send_msg(io);
			// Log.Log(LogLevel.Info, ((object[,]) io[io.Length - 1])[0, 1].ToString());
		}

		private static void Test15() {
			MsgQ msgQ = new MsgQ();
			string alias = null;
			string criterion = "id";
			Dictionary<string, object> newData = new Dictionary<string, object>(15);

			DateTime msgqDate = DateTime.UtcNow;
			newData["msgqrecid"] = 2L;
			newData["msgqid"] = 3L;
			newData["msgqprov_enttid"] = 2L;
			newData["msgqtarg_enttid"] = 42;
			newData["msgqsrc"] = "{1,2,3}";
			newData["msgqtyp"] = 3;
			newData["msgqtext"] = "Test message";
			newData["msgqdoc_docuid"] = 4L;
			newData["msgqdatestart"] = msgqDate;
			newData["msgqdateend"] = msgqDate.AddDays(5);
			newData["msgqstatus"] = 5;
			newData["msgqtransaction_tranid"] = 6L;
			newData["msgqactive"] = true;

			msgQ.AddNew(alias, criterion, newData);
		}

		private static void Test16() {
			MsgQ msgQ = new MsgQ();
			string alias = null;
			string criterion = "id";
			Dictionary<string, object> newData = new Dictionary<string, object>(15);

			DateTime msgqDate = DateTime.UtcNow;
			// newData["msgqrecid"] = 27L;
			newData["msgqid"] = 6L;
			/*newData["msgqprov_enttid"] = 2L;
			*/
			newData["msgqtarg_enttid"] = 42;
			/*
			newData["msgqsrc"] = "{1,2,3}";
			newData["msgqtyp"] = 3;
			newData["msgqtext"] = "Test message";
			newData["msgqdoc_docuid"] = 4L;
			newData["msgqdatestart"] = msgqDate;
			newData["msgqdateend"] = msgqDate.AddDays(5);
			newData["msgqstatus"] = 5;
			newData["msgqtransaction_tranid"] = 6L;
			newData["msgqactive"] = true;
			*/
			msgQ.Modify(alias, criterion, newData);
		}

		private static void Test17() {
			MsgQ msgQ = new MsgQ();
			string alias = null;
			string criterion = "id";
			Dictionary<string, object> newData = new Dictionary<string, object>(15);

			DateTime msgqDate = DateTime.UtcNow;
			newData["msgqrecid"] = 66L;
			// newData["msgqid"] = 6L;

			msgQ.Delete(alias, criterion, newData);
		}

		private static void Test18() {
			Log.Log(LogLevel.Info, "Test 18");
			MmaEnt rdi = new MmaEnt();
			string alias = "messagehistoryrecords";
			string criterion = "searchmsghst";
			string targEntId = "668";
			string dateStart = null;
			string dateEnd = null;
			string orderBy = "DESC";
			string recordLimit = "5";
			UserInfo userInfo = new UserInfo() {
				UserId = 2L,
				EntityId = 806L
			};
			ArrayList parameters = new ArrayList {targEntId, dateStart, dateEnd, orderBy, recordLimit};
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;

			object[] io = new object[] {alias, criterion, parameters, offset, limit, fieldNames, userInfo, null};
			RetCde retCode = rdi.search_msg_hst(io);

			if (retCode != RetCde.Success) {
				Log.Log(LogLevel.Error, "Error: rdi.search_msg_hst() function returned error code: " + retCode.ToString());
				return;
			}

			object[,] data = (object[,]) io[io.Length - 1];
			int rowsCount = data.GetLength(0);
			Log.Log(LogLevel.Info, "Rows: " + (rowsCount - 1));
			/*
			for (long i = 0; i < rowsCount; i++) {
				for (int j = 0; j < colsCount; j++) {
					Console.Write(data [i, j]);
					Console.Write("\t");
				}

				Console.WriteLine();
			}*/
		}

		/*
		private static void Test19() {
			Log.Log(LogLevel.Info, "Test 19");
			MmaEnt rdi = new MmaEnt();
			string targSrc = null;
			long prov = -1;
			long targEntId = -1;
			string text = "Hi Hi";
			long? doc = null;
			ushort msgTyp = 1;
			DateTime dateStart = DateTime.UtcNow;
			DateTime? dateEnd = null;
			
			rdi.SendMessageToSource(targSrc, prov, targEntId, text, doc, msgTyp, dateStart, dateEnd);
		}*/

		/*
		private static void Test20() {
			Log.Log(LogLevel.Info, "Test 20");
			Acct rdi = new Acct();
			string userName = "";
			string userPassword = "";
			string alias = "acctaccount";
			string criterion = "userlogin";
			ArrayList parameters = new ArrayList {userName, userPassword};
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;
			object[,] data;

			try {
				data = rdi.UserLogin(alias, criterion, parameters, offset, limit, fieldNames);
			} catch (Exception ex) {
				string errorMessage = "Acct.UserLogin: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return;
			}

			int rowsCount = data.GetLength(0);
			int colsCount = data.GetLength(1);
			Log.Log(LogLevel.Info, "Rows: " + (rowsCount - 1));
			Log.Log(LogLevel.Info, "Cols: " + colsCount);

			for (long i = 0; i < rowsCount; i++) {
				for (int j = 0; j < colsCount; j++) {
					Console.Write(data [i, j]);
					Console.Write("\t");
				}

				Console.WriteLine();
			}
		}
		*/

		private static void Test21() {
			Log.Log(LogLevel.Info, "Test 21");
			Acct rdi = new Acct();
			string userName = "";
			string userPassword = "";
			long userId, userEntityId;

			bool success = rdi.TryUserLogin(userName, userPassword, out userId, out userEntityId);

			if (success)
				Log.Log(LogLevel.Info, string.Format("Successful User login: User ID = {0}, User Entity ID = {1}", userId, userEntityId));
			else
				Log.Log(LogLevel.Error, "User login failed");
		}

		private static void Test22() {
			Nd node = new Nd();
			string alias = null;
			string criterion = "countbynm";
			string nodeNm = "testnode";

			/*
			int duplicatesFound = 0;
			Random random = null;

			do {
				ArrayList critParameters = new ArrayList {nodeNm};
				object[,] resultData = node.Search(alias, criterion, critParameters, 0L, -1L, null);
				duplicatesFound = Convert.ToInt32(resultData[1, 0]);
				Log.Log(LogLevel.Error, "Number of records with the same 'nm': " + duplicatesFound);

				if (duplicatesFound > 0) {
					if (random == null)
						random = new Random();

					nodeNm += random.Next(2, 999999);

					if (nodeNm.Length > 100)
						nodeNm = nodeNm.Substring(0, 20) + random.Next(2, 999999);

					Log.Log(LogLevel.Error, "New 'nm' will be used: " + nodeNm);
				}
			} while (duplicatesFound > 0);
			*/

			criterion = "default";
			DateTime utcNow = DateTime.UtcNow;
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = nodeNm;
			newData["nodetyp_ndtpid"] = 1;

			/*
			newData["nodedelta0"] = utcNow;
			newData["nodedelta"] = utcNow;
			newData["nodebirthd"] = utcNow;
			newData["nodevld"] = 0.1;
			newData["noderpn"] = 0.1;
			newData["nodedat"] = "dat";
			newData["nodegeo_geolid"] = 230L;
			newData["nodeactive"] = true;
			newData["nodebirthdnum"] = -1;
			newData["nodesubs"] = null;
			newData["nodesubtyp"] = null;
			newData["nodeprvs"] = null;
			newData["nodeprvtyp"] = null;
			*/

			node.AddNew(alias, criterion, newData);
		}

		private static void Test23() {
			Ent entt = new Ent();
			string alias = null;
			string criterion = "countbynm";
			string enttNm = "testentt";
			int duplicatesFound = 0;
			Random random = null;

			do {
				ArrayList critParameters = new ArrayList {enttNm};
				object[,] resultData = entt.Search(alias, criterion, critParameters, 0L, -1L, null);
				duplicatesFound = Convert.ToInt32(resultData[1, 0]);
				Log.Log(LogLevel.Error, "Number of records with the same 'nm': " + duplicatesFound);

				if (duplicatesFound > 0) {
					if (random == null)
						random = new Random();

					enttNm += random.Next(2, 999999);

					if (enttNm.Length > 100)
						enttNm = enttNm.Substring(0, 20) + random.Next(2, 999999);

					Log.Log(LogLevel.Error, "New 'nm' will be used: " + enttNm);
				}
			} while (duplicatesFound > 0);

			criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["enttnm"] = enttNm;
			newData["enttsnm"] = "snm";
			newData["enttnode_nodeid"] = -1L;
			newData["enttsmt"] = -1.1;
			newData["enttgender"] = 'm';
			newData["enttlang_langid"] = 1L;
			newData["enttkeywords"] = "keywords";
			newData["enttpic_docuid"] = -1L;
			newData["acctstatus"] = 1;
			newData["enttactive"] = true;

			entt.AddNew(alias, criterion, newData);
		}

		private static void Test24() {
			Acct acct = new Acct();
			string alias = null;
			string criterion = "countbynm";
			string acctNm = "testacct";
			int duplicatesFound = 0;
			Random random = null;

			do {
				ArrayList critParameters = new ArrayList {acctNm};
				object[,] resultData = acct.Search(alias, criterion, critParameters, 0L, -1L, null);
				duplicatesFound = Convert.ToInt32(resultData[1, 0]);
				Log.Log(LogLevel.Error, "Number of records with the same 'nm': " + duplicatesFound);

				if (duplicatesFound > 0) {
					if (random == null)
						random = new Random();

					acctNm += random.Next(2, 999999);

					if (acctNm.Length > 100)
						acctNm = acctNm.Substring(0, 20) + random.Next(2, 999999);

					Log.Log(LogLevel.Error, "New 'nm' will be used: " + acctNm);
				}
			} while (duplicatesFound > 0);

			criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["acctnm"] = acctNm;
			newData["acctsnm"] = "snm";
			newData["acctnode_nodeid"] = -1L;
			newData["acctentity_enttid"] = -1L;
			newData["acctsource_srceid"] = -1L;
			newData["acctforeignid"] = -1L;
			newData["acctsecret"] = -1;
			newData["acctauth"] = "auth";
			newData["acctauth2"] = "auth2";
			newData["acctstatus"] = 1;
			newData["acctactive"] = true;
			newData["acctdaylimit"] = 40;

			acct.AddNew(alias, criterion, newData);
		}

		private static void Test25() {
			Usr user = new Usr();
			string alias = null;
			string criterion = "countbynm";
			string userNm = "testuser";
			int duplicatesFound = 0;
			Random random = null;

			do {
				ArrayList critParameters = new ArrayList {userNm};
				object[,] resultData = user.Search(alias, criterion, critParameters, 0L, -1L, null);
				duplicatesFound = Convert.ToInt32(resultData[1, 0]);
				Log.Log(LogLevel.Error, "Number of records with the same 'nm': " + duplicatesFound);

				if (duplicatesFound > 0) {
					if (random == null)
						random = new Random();

					userNm += random.Next(2, 999999);

					if (userNm.Length > 100)
						userNm = userNm.Substring(0, 20) + random.Next(2, 999999);

					Log.Log(LogLevel.Error, "New 'nm' will be used: " + userNm);
				}
			} while (duplicatesFound > 0);

			criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["usernm"] = userNm;
			newData["usernode_nodeid"] = -1L;
			newData["useraccount_acctid"] = -1L;
			newData["useractive"] = true;
			newData["userdaylimit"] = 40;

			user.AddNew(alias, criterion, newData);
		}

		private static void Test26() {
			Acct acct = new Acct();
			string alias = null;
			string criterion = "default";
			DateTime utcNow = DateTime.UtcNow;
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = "testacct";

			newData["acctnm"] = "testacct";
			newData["acctsnm"] = "snm";
			// newData["acctnode_nodeid"] = -1L;
			newData["acctentity_enttid"] = 1L;
			newData["acctsource_srceid"] = 1L;
			newData["acctforeignid"] = 1L;
			newData["acctsecret"] = "secret";
			// newData["acctauth"] = "auth";
			// newData["acctauth2"] = "auth2";
			/* newData["acctstatus"] = 1;
			newData["acctactive"] = true;
			*/
			newData["acctdaylimit"] = (short) 40;

			acct.AddNew(alias, criterion, newData);
		}

		private static void Test27() {
			Ent ent = new Ent();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = "testent";

			newData["enttnm"] = "testent";
			newData["enttsnm"] = "snm";
			// newData["enttsmt"] = 0.1;
			newData["enttgender"] = 'o';
			/* newData["enttlang_langid"] = 1L;
			newData["enttkeywords"] = null;
			newData["enttpic_docuid"] = 1L;*/

			ent.AddNew(alias, criterion, newData);
		}

		private static void Test28() {
			Usr usr = new Usr();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = "testusr";
			newData["useraccount_acctid"] = 1L;
			newData["usernm"] = "testusr";
			/*newData["userprefs"] = "prefs";
			newData["userstate"] = 1;
			newData["userdaylimit"] = (short) 40;*/

			usr.AddNew(alias, criterion, newData);
		}

		private static void Test29() {
			MmaEnt mmaEnt = new MmaEnt();
			string accountName = "normantonstreet";
			string screenName = "NormantonStreet";
			string accountSecret = "secret";
			char? gender = 'm';
			short? accountDayLimit = 40;
			UserInfo userInfo;
			mmaEnt.RegisterUser(accountName, screenName, accountSecret, gender, accountDayLimit, out userInfo);

			Console.WriteLine("User ID: " + userInfo.UserId);
			Console.WriteLine("Entity ID: " + userInfo.EntityId);
		}

		private static void Test30() {
			MmaEnt mmaEnt = new MmaEnt();
			string accountName = "testnm";
			string screenName = "testsnm";
			string foreignId = "foreignId";
			string accountAuthKey = "auth";
			string accountAuthKeySecret = "secret";
			DateTime? accountBirthDate = null; // DateTime.UtcNow.AddDays(-1D);
			short? accountDayLimit = 40;
			long entityId = 810L;
			long sourceId = 3L; // Twitter

			mmaEnt.RegisterAccount(accountName, screenName, foreignId, accountAuthKey, accountAuthKeySecret, accountBirthDate, accountDayLimit, entityId, sourceId);
		}

		private static void Test31() {
			Usr usr = new Usr();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = "testusr";
			newData["useraccount_acctid"] = 1L;
			newData["usernm"] = "testusr";

			Aeye.Framework.AeyeConnection aeyeDbConn = new Aeye.Framework.AeyeConnection();

			using (IDbConnection dbConn = aeyeDbConn.CreateDbConnection()) {
				dbConn.Open();
				IDbTransaction tr = null;

				using (tr = dbConn.BeginTransaction()) {
					try {
						usr.AddNew(alias, criterion, newData, dbConn);
						// tr.Rollback();
						tr.Commit();
					} catch (Exception ex) {
						Console.WriteLine("Error: " + ex.Message);

						if (tr != null) {
							try {
								tr.Rollback();
							} catch (Exception ex2) {
								Console.WriteLine("Error2: " + ex2.Message);
							}
						}
					}
				}
			}
		}

		private static void Test32() {
			Lang lang = new Lang();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["langnm"] = "testlang";

			lang.AddNew(alias, criterion, newData);
		}

		private static void Test33() {
			Edg edge = new Edg();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["edgenm"] = "edge";
			newData["edgetyp_edtpid"] = (short) 11;
			newData["edgeprov_nodeid"] = 2L;
			newData["edgesub_nodeid"] = 3L;
			/*
			newData["edgesmt"] = 0.5F;
			newData["edgepriv"] = (short) 1;
			*/

			edge.AddNew(alias, criterion, newData);
		}

		private static void Test34() {
			EntInfo entInfo = new EntInfo();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["eninentity_enttid"] = 1L;
			newData["enininfotype_intpid"] = (short) 2;
			newData["eninval"] = "Test Value";

			entInfo.AddNew(alias, criterion, newData);
		}

		private static void Test35() {
			Geo geo = new Geo();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = "testnodenm";
			newData["geolnm"] = "testgeonm";
			newData["geolx"] = 1.1F;
			newData["geoly"] = 2.2F;
			newData["geolr"] = 3.3F;
			newData["geolb"] = "b";
			newData["geolcity"] = "city";
			newData["geolcntry"] = "cntry";
			newData["geolzip"] = "zip_OTHER";
			newData["geoltz"] = "tz";
			newData["geoltyp_gltpid"] = (short) 2;

			geo.AddNew(alias, criterion, newData);
		}

		private static void Test36() {
			Doc doc = new Doc();
			string alias = null;
			string criterion = "default";
			Dictionary<string, object> newData = new Dictionary<string, object>();
			newData["nodenm"] = "docu";
			newData["docunm"] = "docu";
			// newData["docunode_nodeid"] = 1L; // Add this param, only if the node exists, otherwise leave it empty and will be auto-generated.
			newData["docudat"] = "https://some.url";
			newData["docuconttyp"] = "picture";
			newData["docusmt"] = 0.0F;
			newData["doculang_langid"] = (short) 1; // en.
			newData["docuchop"] = false;

			Aeye.Framework.AeyeConnection aeyeDbConn = new Aeye.Framework.AeyeConnection();

			using (IDbConnection dbConn = aeyeDbConn.CreateDbConnection()) {
				dbConn.Open();
				IDbTransaction tr = null;

				using (tr = dbConn.BeginTransaction()) {
					try {
						doc.AddNew(alias, criterion, newData, dbConn);
						tr.Commit();
					} catch (Exception ex) {
						Console.WriteLine("Error: " + ex.Message);

						if (tr != null) {
							try {
								tr.Rollback();
							} catch (Exception ex2) {
								Console.WriteLine("Error2: " + ex2.Message);
							}
						}
					}
				}
			}
		}

		private static void Test37(string[] args){
			long entid = 812; //whichever one to import : TODO: read from args.. (if none, will assume current ent instance, if initialised and fail otherwise )
			UserInfo userInfo; 
			MmaEnt mmaent = new MmaEnt();
			Dictionary<string, object> dargs = new Dictionary<string, object>();

			dargs.Add("accountName", "morontonstreet");
			dargs.Add("screenName", "MorontonStreet");
			dargs.Add("accountSecret", "secret");
			dargs.Add("gender", 'o');
			dargs.Add("accountDayLimit", 1000);
			dargs.Add("filename", "/home/aeye/normantonstreet.tsv");
			dargs.Add("separator", "\t");

			for( int i = 0; i < args.Length; i+=2) {
				switch(args [i]){
				case "--accname":
					dargs["accountName"] = args[i+1];
					break;
				case "--snname":
					dargs["screenName"] = args[i+1];
					break;
				case "--accsecret":
					dargs["accountSecret"] = args[i+1];
					break;
				case "--gender":
					dargs["gender"] = args[i+1];
					break;
				case "--accdaylimit":
					dargs["accountDayLimit"] = args[i+1];
					break;
				case "--filename":
					dargs["filename"] = args[i+1];
					break;
				case "--separator":
					dargs["separator"] = args[i+1];
					break;
				
				default:
					Console.WriteLine ("Error with parameter {0}", args [i]);
					break;
				}
			} //while

			// mmaEnt.RegisterUser(accountName, screenName, accountSecret, gender, accountDayLimit, out userInfo);
			short daylimit = 1000; //dargs ["accountDayLimit"];
			// mmaEnt.RegisterUser((string)dargs["accountName"], (string)dargs ["screenName"], (string)dargs["accountSecret"], (char)dargs ["gender"], daylimit, out userInfo);

			Console.WriteLine("User ID: " + userInfo.UserId);
			Console.WriteLine("Entity ID: " + userInfo.EntityId);

			//MmaEnt mmaent = new MmaEnt ();
			mmaent.ImportFrFollFile (dargs, entid);
		}

		private static void Test38() {
			MmaEnt mmaEnt = new MmaEnt();
			string accountName = "morontonstreet";
			string screenName = "Normanton Street";
			string accountSecret = "secret";
			char? gender = 'o';
			short? accountDayLimit = 1000;
			UserInfo userInfo;
			mmaEnt.RegisterUser(accountName, screenName, accountSecret, gender, accountDayLimit, out userInfo);

			Console.WriteLine("User ID: " + userInfo.UserId);
			Console.WriteLine("Entity ID: " + userInfo.EntityId);
		}

		public static void Main(string[] args) {
			Config();
			//Test35 ();
			Test37(args);
			//Test38();
		}
	}
}

