using System;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using app_c.mma.rdi;
using system.block.item;
using NLog;

namespace app.mma {
	public class Src : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Src() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (srcesource_c) srcesource_c.getInstance(oApp.context);
		}
	}
}
