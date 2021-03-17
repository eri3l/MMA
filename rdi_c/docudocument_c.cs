using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class docudocument_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static docudocument_c thisItem;

		private docudocument_c(Type oUserType) : base(oUserType) {
		}

		private docudocument_c() : base(typeof(app.mma.rdi.docudocument)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new docudocument_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new docudocument_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new docudocument_c(oUserType);

			return thisItem;
		}
	}
}
