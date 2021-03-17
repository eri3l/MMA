using System;
using System.Collections;
using System.Data;
using system.block.item;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	public class RDISplImplRWM_c : RawDataItemDB_c {
		public static seqisequences_c seqRDI;
		public static string transTableName;
		private bool keepHistoricalDbRecords = true;

		public RDISplImplRWM_c(System.Type t) : base(t) {	
			this.registerMembers(typeof(app.mma.rdi.RDISplImplRWM));
			this.GetAttributes(typeof(app.mma.rdi.RDISplImplRWM));
			this.fireDefault(Context_c.getRootContext());

			if (keepHistoricalDbRecords) {
				if (seqRDI == null)
					seqRDI = (seqisequences_c) seqisequences_c.getInstance();

				transTableName = (string) ((Hashtable) this.oMeta)["tablename"];
			}
		}

		internal string TableName {
			get {
				return (string) ((Hashtable) this.oMeta)["tablename"];
			}
		}

		public override long addNew(long instance, object[] addNewParams) {
			return this.addNew(instance, addNewParams, null);
		}

		public long addNew(long instance, object[] addNewParams, IDbConnection dbConnection) {
			long id;

			if (keepHistoricalDbRecords)
				id = addNewHistorical(instance, addNewParams, dbConnection);
			else
				id = addNewNonHistorical(instance, addNewParams, dbConnection);

			return id;
		}

		public override long modify(long instance, object[] param) {
			return this.modify(instance, param, null);
		}

		public long modify(long instance, object[] param, IDbConnection dbConnection) {
			long id;

			if (keepHistoricalDbRecords)
				id = modifyHistorical(instance, param, dbConnection);
			else
				id = modifyNonHistorical(instance, param, dbConnection);

			return id;
		}

		public override long delete(long instance, object[] delParams) {
			return this.delete(instance, delParams, null);
		}

		public long delete(long instance, object[] delParams, IDbConnection dbConnection) {
			long id;

			if (keepHistoricalDbRecords)
				id = deleteHistorical(instance, delParams, dbConnection);
			else
				id = deleteNonHistorical(instance, delParams, dbConnection);

			return id;
		}

		#region Non-Historical Methods
		private long addNewNonHistorical(long instance, object[] addNewParams, IDbConnection dbConnection) {
			throw new NotImplementedException();
			/*
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			long[] paramsIDs = rdiInst.getAddNewParamIDs();
			int paramsCount = paramsIDs.Length;
			object[] paramArray = new object[paramsCount];

			for (int i = 0; i < paramsCount; i++)
				paramArray[i] = addNewParams[paramsIDs[i]];

			try {
				rdiInst.addNew(paramArray);
			} catch (Exception ex) {
				throw new Exception("Database Error: " + ex.Message, ex);
			}

			return (long) errorCode.NoError;
			*/
		}

		private long modifyNonHistorical(long instance, object[] param, IDbConnection dbConnection) {
			throw new NotImplementedException();
			/*
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			long[] fieldsIDs = rdiInst.getModifySetParamIDs();
			int fieldsCount = fieldsIDs.Length;
			long[] paramsIDs = rdiInst.getModifyCritParamIDs();
			int paramsCount = paramsIDs.Length;

			int paramsAndFieldsCount = fieldsCount + paramsCount;
			object[] paramArray = new object[paramsAndFieldsCount];
			int arrayIndex = 0;

			for (int i = 0; i < fieldsCount; i++, arrayIndex++)
				paramArray[arrayIndex] = param[fieldsIDs[i]];

			for (int i = 0; i < paramsCount; i++, arrayIndex++)
				paramArray[arrayIndex] = param[paramsIDs[i]];

			try {
				rdiInst.modify(paramArray);
			} catch (Exception ex) {
				throw new Exception("Database Error: " + ex.Message, ex);
			}

			return (long) errorCode.NoError;
			*/
		}

		private long deleteNonHistorical(long instance, object[] delParams, IDbConnection dbConnection) {
			throw new NotImplementedException();
			/*
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			long[] paramsIDs = rdiInst.getDeleteParamIDs();
			int paramsCount = paramsIDs.Length;
			object[] paramArray = new object[paramsCount];

			for (int i = 0; i < paramsCount; i++)
				paramArray[i] = delParams[paramsIDs[i]];

			try {
				long delRes = rdiInst.delete(paramArray);
			} catch (System.Exception ex) {
				throw new Exception("Database: " + ex.Message, ex);
			}

			return (long) errorCode.NoError;
			*/
		}
		#endregion

		#region Historical Methods
		private long addNewHistorical(long instance, object[] addNewParams, IDbConnection dbConnection) {
			/// 1) get the a new sequence id from the sequence table
			/// 2) get a new transactionID
			/// 3) add a new record with the transactionid and the recid for this record.
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			string tableName = this.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			string sRecIDName = tablePrefix +"recid";
			string sIDName = tablePrefix +"id";
			string sTransName = tablePrefix +"transaction_tranid";

			long newRecId = (int) seqRDI.getNextSeqNo(tableName);
			int recidElementId = (int) elemInfo[sRecIDName].elementID;
			addNewParams[recidElementId] = newRecId;

			int idElementId = (int) elemInfo[sIDName].elementID;
			object idValueAsObject = addNewParams[idElementId];
			long id;

			if (idValueAsObject == null || idValueAsObject == System.DBNull.Value) {
				string nextidCriterionName = "nextid";
				string nextidFieldName = nextidCriterionName;
				long nextidCritId = this.getCritID(nextidCriterionName);
				this.search(nextidCriterionName, new object[0]);
				this.refresh(nextidCriterionName);
				id = Convert.ToInt64(this.getO(0, nextidFieldName, nextidCritId));
				addNewParams[idElementId] = id;
			} else
				id = Convert.ToInt64(idValueAsObject);

			addNewParams[(int)elemInfo[sTransName].elementID] = this.context.lCurrTransID;

			/* if(! checkUnique(uniqueCrit, addNewParams))
				return (long) errorCode.DuplicateKey;/// return error code.
			*/

			long[] InsertParams = rdiInst.getAddNewParamIDs();
			object[] paramArray = new object[InsertParams.Length];

			for (int i = 0, n = InsertParams.Length; i < n; i++)
				paramArray[i] = addNewParams[InsertParams[i]];

			try {
				rdiInst.addNew(paramArray, dbConnection);
			} catch (Exception e) {
				// rollback transaction
				//rdiInst.context.invokeEvent(3, rdiInst.context);
				//!!! rdiInst.rollbackTransaction();
				throw new Exception("Database Error: "+e.Message, e);
				//return (long)errorCode.DatabaseError;
			}

			return id;
		}

		private long addNewHistorical(long instance, object[] addNewParams, IDbConnection dbConnection, out long newRecId) {
			/// 1) get the a new sequence id from the sequence table
			/// 2) get a new transactionID
			/// 3) add a new record with the transactionid and the recid for this record.
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			string tableName = this.TableName;
			string tablePrefix = this.TableName.Substring(0, 4);
			string sRecIDName = tablePrefix +"recid";
			string sIDName = tablePrefix +"id";
			string sTransName = tablePrefix +"transaction_tranid";
			// string uniqueCrit = tablePrefix +"Unique";

			/* if(! checkUnique(uniqueCrit, addNewParams))
				return (long) errorCode.DuplicateKey;/// return error code.
			*/

			newRecId = (int) seqRDI.getNextSeqNo(tableName);
			int recidElementId = (int) elemInfo[sRecIDName].elementID;
			addNewParams[recidElementId] = newRecId;

			int idElementId = (int) elemInfo[sIDName].elementID;
			object idValueAsObject = addNewParams[idElementId];
			long id;

			if (idValueAsObject == null || idValueAsObject == System.DBNull.Value) {
				string nextidCriterionName = "nextid";
				string nextidFieldName = nextidCriterionName;
				long nextidCritId = this.getCritID(nextidCriterionName);
				this.search(nextidCriterionName, new object[0]);
				this.refresh(nextidCriterionName);
				id = Convert.ToInt64(this.getO(0, nextidFieldName, nextidCritId));
				addNewParams[idElementId] = id;
			} else
				id = Convert.ToInt64(idValueAsObject);

			addNewParams[(int) elemInfo[sTransName].elementID] = this.context.lCurrTransID;

			long[] InsertParams =  rdiInst.getAddNewParamIDs();
			object[] paramArray = new object[InsertParams.Length];

			for (int i = 0, n = InsertParams.Length; i < n; i++)
				paramArray[i] = addNewParams[InsertParams[i]];

			try {
				rdiInst.addNew(paramArray, dbConnection);
			} catch (Exception e) {
				// rollback transaction
				//rdiInst.context.invokeEvent(3, rdiInst.context);
				//!!! rdiInst.rollbackTransaction(rdiInst.context);
				throw new Exception("Database Error: " + e.Message, e);
				//return (long)errorCode.DatabaseError;
			}

			return id;
		}

		private long modifyHistorical(long instance, object[] param, IDbConnection dbConnection) {
			/// 1) mark the record as inactive
			/// 2) add a new record with a transactionType as update.
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			long id;
			bool isInternallyGeneratedConnection = dbConnection == null;

			try {
				// Don't use transaction if the connection was not generated internally but passed as param, because it might be part of transaction.
				if (isInternallyGeneratedConnection)
					rdiInst.startTransaction(rdiInst.context);

				this.deleteHistorical(instance, param, dbConnection);
				id = this.addNewHistorical(instance, param, dbConnection);

				if (isInternallyGeneratedConnection)
					rdiInst.commitTransaction(rdiInst.context);
			} catch (System.Exception e) {
				// rollback transaction
				//rdiInst.context.invokeEvent(3, rdiInst.context);
				if (isInternallyGeneratedConnection)
					rdiInst.rollbackTransaction(rdiInst.context);

				throw new Exception("Database Error: "+e.Message, e);
				//return (long)errorCode.DatabaseError;
			}

			return id;
		}

		private long deleteHistorical(long instance, object[] delParams, IDbConnection dbConnection) {
			RdiDotNetSQLInstance_c rdiInst = (RdiDotNetSQLInstance_c) this[(int) instance];
			string tablePrefix = this.TableName.Substring(0, 4);
			string sRecIDName = tablePrefix +"recid";

			int recidElementId = (int) elemInfo[sRecIDName].elementID;
			object recidValueAsObject = delParams[recidElementId];
			object[] updateParams = new object[] {recidValueAsObject};

			//kjj added - the db caching framework detects a change in the db looking at the
			// seqi table - so we'd need to iincrease the seqi id even when deleting ,
			// so that the cache is invalidated
			// long seqid = seqRDI.getNextSeqNo(tableName);

			try {
				rdiInst.modify(updateParams, dbConnection);
			} catch (Exception e) {
				// rollback transaction
				//rdiInst.context.invokeEvent(3, rdiInst.context);
				//!!! rdiInst.rollbackTransaction();
				throw new Exception("Database: " + e.Message, e);
				//return (long)errorCode.DatabaseError;
			}

			return -1L;
		}

		/*
		private bool checkUnique(string uniqueCrit, object[] addNewParams)
		{
			RdiDotNetSQLInstance_c rdiUnique = (RdiDotNetSQLInstance_c)this[uniqueCrit];
			if(rdiUnique != null)
			{
				long rdiUniqID = rdiUnique.id;
				long[] srchUniqParIDs = rdiUnique.getSearchParamIDs();
				object[] srchUniqParVals = new object[srchUniqParIDs.Length];
				for(int i=0;i<srchUniqParIDs.Length;i++)
					srchUniqParVals[i] = addNewParams[srchUniqParIDs[i]];

				this.search(rdiUniqID, srchUniqParVals);
				this.refresh(rdiUniqID);
				if(rdiUnique.Count > 0)
				{
					return false;///// return error code.
				}
			}
			return true;
		}*/
		#endregion
	}
}
