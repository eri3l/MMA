using System;
using System.Collections;
using System.Collections.Specialized;

namespace app.mma {
	/// <summary>
	/// This is static hashtable describing the relation between the item type (defined in the xml) and the relevant rdi class
	/// </summary>
	public class ItemTypeRdiMap {
		public static Hashtable Map = new Hashtable();

		static ItemTypeRdiMap() {
			Map["userrecords"] = "Ent";
		}
	}
}
