using System;
using system.block.item;
using system.block.uda.dataSource;
using app.mma.rdi;
using app_c.mma.rdi;

namespace app_c.mma.rdi {
	[Serializable]
	public class msgqmessagequeue_c : RDISplImplRWM_c {
		private static msgqmessagequeue_c thisItem;

		private msgqmessagequeue_c(Type oUserType) : base(oUserType) {
		}

		private msgqmessagequeue_c() : base(typeof(msgqmessagequeue)) {
		}

		public static RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new msgqmessagequeue_c();

			return thisItem;
		}

		public static RawDataItemDB_c getInstance(Context_c oContext) {
			if (thisItem == null) {
				thisItem = new msgqmessagequeue_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new msgqmessagequeue_c(oUserType);

			return thisItem;
		}
	}
}
