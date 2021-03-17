using System;
using app_c.mma.rdi;
using system.block.item;

namespace app.mma {
	public class MsgQ : RDISplImplRWM {
		public MsgQ() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (msgqmessagequeue_c) msgqmessagequeue_c.getInstance(oApp.context);
		}
	}
}
