using System;
using system.block.item;
using system.block.uda.dataSource;
using app.mma.rdi;

namespace app_c.mma.rdi {
	public class seqisequences_c : RawDataItemDB_c {
		private seqisequences_c() : base(typeof(seqisequences))	{
		}

		private seqisequences_c(Type oUserType) : base(oUserType) {
		}

		private static seqisequences_c thisItem;
		public static RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new seqisequences_c();

			return thisItem;
		}

		public static RawDataItemDB_c getInstance(Context_c oContext) {
			if (thisItem == null) {
				thisItem = new seqisequences_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new seqisequences_c(oUserType);

			return thisItem;
		}

		public long getNextSeqNo(string tableName) {
			long critID = this.getCritID("seqForTable");
			this.modify(critID, new object[] {tableName});
			this.search(critID, new object[] {tableName});
			this.refresh(critID);
			long res = Convert.ToInt64(this.getO(0, "seqinumber", (int) critID));
			return res;
		}

		public long getCurrentSeqNo(string tableName) {
			long critID = this.getCritID("seqForTable");
			this.search(critID, new object[] {tableName});
			this.refresh(critID);
			long res = Convert.ToInt64(this.getO(0, "seqinumber", (int) critID));
			return res;
		}
	}
}