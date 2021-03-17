using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "eninentityinfo")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT eninrecid, eninid, eninentity_enttid, enininfotype_intpid, eninval, enintransaction_tranid, eninactive FROM eninentityinfo;
	INSERT INTO eninentityinfo (eninrecid, eninid, eninentity_enttid, enininfotype_intpid, eninval, enintransaction_tranid, eninactive) VALUES (@eninrecid, @eninid, @eninentity_enttid, @enininfotype_intpid, @eninval, @enintransaction_tranid, @eninactive);
	UPDATE eninentityinfo SET eninactive = FALSE WHERE eninrecid = @eninrecid;
	DELETE FROM eninentityinfo WHERE eninrecid = @eninrecid")]

	[Criteria("id", @"SELECT eninrecid, eninid, eninentity_enttid, enininfotype_intpid, eninval, enintransaction_tranid, eninactive FROM eninentityinfo WHERE eninactive = TRUE AND eninid = @eninid;
	INSERT INTO eninentityinfo (eninrecid, eninid, eninentity_enttid, enininfotype_intpid, eninval, enintransaction_tranid, eninactive) VALUES (@eninrecid, @eninid, @eninentity_enttid, @enininfotype_intpid, @eninval, @enintransaction_tranid, @eninactive);
	UPDATE eninentityinfo SET eninactive = FALSE WHERE eninrecid = @eninrecid;
	DELETE FROM eninentityinfo WHERE eninrecid = @eninrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(eninid), 0) + 1) AS nextid FROM eninentityinfo;
	INSERT INTO eninentityinfo (eninrecid, eninid, eninentity_enttid, enininfotype_intpid, eninval, enintransaction_tranid, eninactive) VALUES (@eninrecid, @eninid, @eninentity_enttid, @enininfotype_intpid, @eninval, @enintransaction_tranid, @eninactive);
	UPDATE eninentityinfo SET eninactive = FALSE WHERE eninrecid = @eninrecid;
	DELETE FROM eninentityinfo WHERE eninrecid = @eninrecid")]

	[Credentials("mmalive")]
	public class eninentityinfo {
		public long eninrecid;
		public long eninid;
		public long eninentity_enttid;
		public short enininfotype_intpid;
		public string eninval;
		public long enintransaction_tranid;
		public bool eninactive;

		public long nextid;
	}
}
