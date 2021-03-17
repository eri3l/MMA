using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "docudocument")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT docurecid, docuid, docunm, docunode_nodeid, docudat, docuconttyp, docusmt, doculang_langid, docuchop, docutransaction_tranid, docuactive FROM docudocument;
	INSERT INTO docudocument (docurecid, docuid, docunm, docunode_nodeid, docudat, docuconttyp, docusmt, doculang_langid, docuchop, docutransaction_tranid, docuactive) VALUES (@docurecid, @docuid, @docunm, @docunode_nodeid, @docudat, @docuconttyp, @docusmt, @doculang_langid, @docuchop, @docutransaction_tranid, @docuactive);
	UPDATE docudocument SET docuactive = FALSE WHERE docurecid = @docurecid;
	DELETE FROM docudocument WHERE docurecid = @docurecid")]

	[Criteria("id", @"SELECT docurecid, docuid, docunm, docunode_nodeid, docudat, docuconttyp, docusmt, doculang_langid, docuchop, docutransaction_tranid, docuactive FROM docudocument WHERE docuactive = TRUE AND docuid = @docuid;
	INSERT INTO docudocument (docurecid, docuid, docunm, docunode_nodeid, docudat, docuconttyp, docusmt, doculang_langid, docuchop, docutransaction_tranid, docuactive) VALUES (@docurecid, @docuid, @docunm, @docunode_nodeid, @docudat, @docuconttyp, @docusmt, @doculang_langid, @docuchop, @docutransaction_tranid, @docuactive);
	UPDATE docudocument SET docuactive = FALSE WHERE docurecid = @docurecid;
	DELETE FROM docudocument WHERE docurecid = @docurecid")]

	[Criteria("countbynm", @"SELECT count(*) AS recordscount FROM docudocument WHERE docuactive = TRUE AND docunm = @docunm;
	INSERT INTO docudocument (docurecid, docuid, docunm, docunode_nodeid, docudat, docuconttyp, docusmt, doculang_langid, docuchop, docutransaction_tranid, docuactive) VALUES (@docurecid, @docuid, @docunm, @docunode_nodeid, @docudat, @docuconttyp, @docusmt, @doculang_langid, @docuchop, @docutransaction_tranid, @docuactive);
	UPDATE docudocument SET docuactive = FALSE WHERE docurecid = @docurecid;
	DELETE FROM docudocument WHERE docurecid = @docurecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(docuid), 0) + 1) AS nextid FROM docudocument;
	INSERT INTO docudocument (docurecid, docuid, docunm, docunode_nodeid, docudat, docuconttyp, docusmt, doculang_langid, docuchop, docutransaction_tranid, docuactive) VALUES (@docurecid, @docuid, @docunm, @docunode_nodeid, @docudat, @docuconttyp, @docusmt, @doculang_langid, @docuchop, @docutransaction_tranid, @docuactive);
	UPDATE docudocument SET docuactive = FALSE WHERE docurecid = @docurecid;
	DELETE FROM docudocument WHERE docurecid = @docurecid")]

	[Credentials("mmalive")]
	public class docudocument {
		public long docurecid;
		public long docuid;
		public string docunm;
		public long docunode_nodeid;
		public string docudat;
		public string docuconttyp;
		public float docusmt;
		public short doculang_langid;
		public bool docuchop;
		public long docutransaction_tranid;
		public bool docuactive;

		public long nextid;
		public long recordscount;
	}
}
