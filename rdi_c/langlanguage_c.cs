using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class langlanguage_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static langlanguage_c thisItem;

		private langlanguage_c(Type oUserType) : base(oUserType) {
		}

		private langlanguage_c() : base(typeof(app.mma.rdi.langlanguage)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new langlanguage_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new langlanguage_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new langlanguage_c(oUserType);

			return thisItem;
		}
	}
}
