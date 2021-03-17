using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class srcesource_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static srcesource_c thisItem;

		private srcesource_c(Type oUserType) : base(oUserType) {
		}

		private srcesource_c() : base(typeof(app.mma.rdi.srcesource)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new srcesource_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new srcesource_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new srcesource_c(oUserType);

			return thisItem;
		}
	}
}
