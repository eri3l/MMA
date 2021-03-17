using System;
using System.IO;
using system.block.uda.dataSource;

namespace app_c.mma.rdi {
	[Serializable]
	public class geolgeolocation_c : app_c.mma.rdi.RDISplImplRWM_c {
		private static geolgeolocation_c thisItem;

		private geolgeolocation_c(Type oUserType) : base(oUserType) {
		}

		private geolgeolocation_c() : base(typeof(app.mma.rdi.geolgeolocation)) {
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance() {
			if (thisItem == null)
				thisItem = new geolgeolocation_c();

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(system.block.item.Context_c oContext) {
			if (thisItem == null) {
				thisItem = new geolgeolocation_c();
				thisItem.context = oContext;
			}

			return thisItem;
		}

		public static system.block.uda.dataSource.RawDataItemDB_c getInstance(Type oUserType) {
			if (thisItem == null)
				thisItem = new geolgeolocation_c(oUserType);

			return thisItem;
		}
	}
}
