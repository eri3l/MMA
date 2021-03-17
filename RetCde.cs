using System;
using System.Collections.Generic;

namespace app.mma {
	public enum RetCde {Success = 100, UnknownError = 0, GeneralError = -1};

	public class RetCdeMap {
		private static Dictionary<RetCde, string>[] map = null;
		private enum Lng {English};

		static RetCdeMap() {
			map = new Dictionary<RetCde, string>[1];
			Dictionary<RetCde, string> englishMap = map[0] = new Dictionary<RetCde, string>();
			englishMap.Add(RetCde.Success, "Success");
			englishMap.Add(RetCde.UnknownError, "Unknown error");
			englishMap.Add(RetCde.GeneralError, "General error");
		}

		public static string Msg(RetCde retCde, int langId) {
			Dictionary<RetCde, string> languageMap = map[langId];

			if (languageMap == null)
				throw new Exception(string.Format("Language ID {0} not supported", langId));

			string message = languageMap[retCde];

			if (message == null)
				throw new Exception(string.Format("Return code {0} not supported for language ID {0}", retCde, langId));

			return message;
		}
	}
}
