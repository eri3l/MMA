using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class acctaccount_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static acctaccount_c thisItem;

		private acctaccount_c(Type oUserType) : base(oUserType) {
		}

		private acctaccount_c() : base(typeof(app.mma.rdi.acctaccount)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new acctaccount_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new acctaccount_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new acctaccount_c(oUserType);

			return thisItem;
		}
	}
}
