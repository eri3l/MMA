using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "msgqmessagequeue")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT msgqrecid, msgqid, msgqprov_enttid, msgqtarg_enttid, msgqsrc, msgqtyp, msgqtext, msgqdoc_docuid, msgqdatestart, msgqdateend, msgqstatus, msgqtransaction_tranid, msgqactive FROM msgqmessagequeue;
	INSERT INTO msgqmessagequeue (msgqrecid, msgqid, msgqprov_enttid, msgqtarg_enttid, msgqsrc, msgqtyp, msgqtext, msgqdoc_docuid, msgqdatestart, msgqdateend, msgqstatus, msgqtransaction_tranid, msgqactive) VALUES (@msgqrecid, @msgqid, @msgqprov_enttid, @msgqtarg_enttid, @msgqsrc, @msgqtyp, @msgqtext, @msgqdoc_docuid, @msgqdatestart, @msgqdateend, @msgqstatus, @msgqtransaction_tranid, @msgqactive);
	UPDATE msgqmessagequeue SET msgqactive = FALSE WHERE msgqrecid = @msgqrecid;
	DELETE FROM msgqmessagequeue WHERE msgqrecid = @msgqrecid")]

	[Criteria("id", @"SELECT msgqrecid, msgqid, msgqprov_enttid, msgqtarg_enttid, msgqsrc, msgqtyp, msgqtext, msgqdoc_docuid, msgqdatestart, msgqdateend, msgqstatus, msgqtransaction_tranid, msgqactive FROM msgqmessagequeue WHERE msgqactive = TRUE AND msgqid = @msgqid;
	INSERT INTO msgqmessagequeue (msgqrecid, msgqid, msgqprov_enttid, msgqtarg_enttid, msgqsrc, msgqtyp, msgqtext, msgqdoc_docuid, msgqdatestart, msgqdateend, msgqstatus, msgqtransaction_tranid, msgqactive) VALUES (@msgqrecid, @msgqid, @msgqprov_enttid, @msgqtarg_enttid, @msgqsrc, @msgqtyp, @msgqtext, @msgqdoc_docuid, @msgqdatestart, @msgqdateend, @msgqstatus, @msgqtransaction_tranid, @msgqactive);
	UPDATE msgqmessagequeue SET msgqactive = FALSE WHERE msgqrecid = @msgqrecid;
	DELETE FROM msgqmessagequeue WHERE msgqrecid = @msgqrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(msgqid), 0) + 1) AS nextid FROM msgqmessagequeue;
	INSERT INTO msgqmessagequeue (msgqrecid, msgqid, msgqprov_enttid, msgqtarg_enttid, msgqsrc, msgqtyp, msgqtext, msgqdoc_docuid, msgqdatestart, msgqdateend, msgqstatus, msgqtransaction_tranid, msgqactive) VALUES (@msgqrecid, @msgqid, @msgqprov_enttid, @msgqtarg_enttid, @msgqsrc, @msgqtyp, @msgqtext, @msgqdoc_docuid, @msgqdatestart, @msgqdateend, @msgqstatus, @msgqtransaction_tranid, @msgqactive);
	UPDATE msgqmessagequeue SET msgqactive = FALSE WHERE msgqrecid = @msgqrecid;
	DELETE FROM msgqmessagequeue WHERE msgqrecid = @msgqrecid")]

	[Credentials("mmalive")]
	public class msgqmessagequeue {
		public long msgqrecid;
		public long msgqid;
		public long msgqprov_enttid;
		public long msgqtarg_enttid;
		public string msgqsrc;
		public short msgqtyp;
		public string msgqtext;
		public long msgqdoc_docuid;
		public DateTime msgqdatestart;
		public DateTime msgqdateend;
		public short msgqstatus;
		public long msgqtransaction_tranid;
		public bool msgqactive;
		public long nextid;
	}
}