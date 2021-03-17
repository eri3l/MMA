using System;
using System.Collections;
using bs.item.attr.Attributes;

namespace app.mma.rdi
{

	public class RDISplImplRWM 
	{
		public static string transTableName;

		public long addNew(long instance, object[] addNewParams)
		{
			return -1;	
		}//addNew

		public long modify(long instance, object[] param)
		{
			return -1;
		}//Update


		public long delete(long instance, object[] delParams)
		{
			return -1;
		}


		#region transaction hack
		[Bind.Alias.Subscriber("startTransaction")]
		public long startTransaction(object oContext)
		{
			return -1;
		}//startTransaction


		[Bind.Alias.Subscriber("commitTransaction")]
		public long commitTransaction(object oContext)
		{			
			return -1;
		}//commitTransaction

		[Bind.Alias.Subscriber("rollbackTransaction")]
		public long rollbackTransaction(object oContext)
		{
			return -1;
		}
		#endregion

	}
}
