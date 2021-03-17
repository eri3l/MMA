using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class enttentity_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static enttentity_c thisItem;

		private enttentity_c(Type oUserType) : base(oUserType) {
		}

		private enttentity_c() : base(typeof(app.mma.rdi.enttentity)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new enttentity_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new enttentity_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new enttentity_c(oUserType);

			return thisItem;
		}
	}
}
