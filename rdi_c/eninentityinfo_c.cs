using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class eninentityinfo_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static eninentityinfo_c thisItem;

		private eninentityinfo_c(Type oUserType) : base(oUserType) {
		}

		private eninentityinfo_c() : base(typeof(app.mma.rdi.eninentityinfo)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new eninentityinfo_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new eninentityinfo_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new eninentityinfo_c(oUserType);

			return thisItem;
		}
	}
}