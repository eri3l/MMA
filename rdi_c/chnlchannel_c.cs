using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class chnlchannel_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static chnlchannel_c thisItem;

		private chnlchannel_c(Type oUserType) : base(oUserType) {
		}

		private chnlchannel_c() : base(typeof(app.mma.rdi.chnlchannel)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new chnlchannel_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new chnlchannel_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new chnlchannel_c(oUserType);

			return thisItem;
		}
	}
}
