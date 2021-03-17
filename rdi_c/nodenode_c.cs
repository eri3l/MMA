using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class nodenode_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static nodenode_c thisItem;

		private nodenode_c(Type oUserType) : base(oUserType) {
		}

		private nodenode_c() : base(typeof(app.mma.rdi.nodenode)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new nodenode_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new nodenode_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new nodenode_c(oUserType);

			return thisItem;
		}
	}
}
