using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class useruser_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static useruser_c thisItem;

		private useruser_c(Type oUserType) : base(oUserType) {
		}

		private useruser_c() : base(typeof(app.mma.rdi.useruser)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new useruser_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new useruser_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new useruser_c(oUserType);

			return thisItem;
		}
	}
}
