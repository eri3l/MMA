using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "srcesource")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"SELECT srcerecid, srceid, srcenm, srcenode_nodeid, srcetyp_sctpid, srceurl, srcetransaction_tranid, srceactive FROM srcesource;
	INSERT INTO srcesource (srcerecid, srceid, srcenm, srcenode_nodeid, srcetyp_sctpid, srceurl, srcetransaction_tranid, srceactive) VALUES (@srcerecid, @srceid, @srcenm, @srcenode_nodeid, @srcetyp_sctpid, @srceurl, @srcetransaction_tranid, @srceactive);
	UPDATE srcesource SET srceactive = FALSE WHERE srcerecid = @srcerecid;
	DELETE FROM srcesource WHERE srcerecid = @srcerecid")]

	[Criteria("id", @"SELECT srcerecid, srceid, srcenm, srcenode_nodeid, srcetyp_sctpid, srceurl, srcetransaction_tranid, srceactive FROM srcesource WHERE srceactive = TRUE AND srceid = @srceid;
	INSERT INTO srcesource (srcerecid, srceid, srcenm, srcenode_nodeid, srcetyp_sctpid, srceurl, srcetransaction_tranid, srceactive) VALUES (@srcerecid, @srceid, @srcenm, @srcenode_nodeid, @srcetyp_sctpid, @srceurl, @srcetransaction_tranid, @srceactive);
	UPDATE srcesource SET srceactive = FALSE WHERE srcerecid = @srcerecid;
	DELETE FROM srcesource WHERE srcerecid = @srcerecid")]

	[Criteria("sctpnmbysrceid", @"SELECT sctpnm FROM srcesource INNER JOIN sctpsourcetype ON srcetyp_sctpid = sctpid WHERE srceactive = TRUE AND sctpactive = TRUE AND srceid = @srceid;
	INSERT INTO srcesource (srcerecid, srceid, srcenm, srcenode_nodeid, srcetyp_sctpid, srceurl, srcetransaction_tranid, srceactive) VALUES (@srcerecid, @srceid, @srcenm, @srcenode_nodeid, @srcetyp_sctpid, @srceurl, @srcetransaction_tranid, @srceactive);
	UPDATE srcesource SET srceactive = FALSE WHERE srcerecid = @srcerecid;
	DELETE FROM srcesource WHERE srcerecid = @srcerecid")]

	[Credentials("mmalive")]
	public class srcesource {
		public long srcerecid;
		public long srceid;
		public string srcenm;
		public long srcenode_nodeid;
		public short srcetyp_sctpid;
		public string srceurl;
		public long srcetransaction_tranid;
		public bool srceactive;

		public string sctpnm;
	}
}