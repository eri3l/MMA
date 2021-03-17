using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "chnlchannel")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT chnlrecid, chnlid, chnlnm, chnlsnm, chnlnode_nodeid, chnluser_userid, chnltransaction_tranid, chnlactive FROM chnlchannel;
	INSERT INTO chnlchannel (chnlrecid, chnlid, chnlnm, chnlsnm, chnlnode_nodeid, chnluser_userid, chnltransaction_tranid, chnlactive) VALUES (@chnlrecid, @chnlid, @chnlnm, @chnlsnm, @chnlnode_nodeid, @chnluser_userid, @chnltransaction_tranid, @chnlactive);
	UPDATE chnlchannel SET chnlactive = FALSE WHERE chnlrecid = @chnlrecid;
	DELETE FROM chnlchannel WHERE chnlrecid = @chnlrecid")]

	[Criteria("id", @"SELECT chnlrecid, chnlid, chnlnm, chnlsnm, chnlnode_nodeid, chnluser_userid, chnltransaction_tranid, chnlactive FROM chnlchannel WHERE chnlactive = TRUE AND chnlid = @chnlid;
	INSERT INTO chnlchannel (chnlrecid, chnlid, chnlnm, chnlsnm, chnlnode_nodeid, chnluser_userid, chnltransaction_tranid, chnlactive) VALUES (@chnlrecid, @chnlid, @chnlnm, @chnlsnm, @chnlnode_nodeid, @chnluser_userid, @chnltransaction_tranid, @chnlactive);
	UPDATE chnlchannel SET chnlactive = FALSE WHERE chnlrecid = @chnlrecid;
	DELETE FROM chnlchannel WHERE chnlrecid = @chnlrecid")]

	[Criteria("userprofiles", @"SELECT chnlid, chnlsnm FROM chnlchannel WHERE chnlactive = TRUE AND chnluser_userid = @chnluser_userid ORDER BY chnlsnm ASC;
	INSERT INTO chnlchannel (chnlrecid, chnlid, chnlnm, chnlsnm, chnlnode_nodeid, chnluser_userid, chnltransaction_tranid, chnlactive) VALUES (@chnlrecid, @chnlid, @chnlnm, @chnlsnm, @chnlnode_nodeid, @chnluser_userid, @chnltransaction_tranid, @chnlactive);
	UPDATE chnlchannel SET chnlactive = FALSE WHERE chnlrecid = @chnlrecid;
	DELETE FROM chnlchannel WHERE chnlrecid = @chnlrecid")]

	[Criteria("channelnode1", @"SELECT chnlnode_nodeid FROM chnlchannel INNER JOIN nodenode ON chnlnode_nodeid = nodeid WHERE chnlactive = TRUE AND nodeactive = TRUE AND chnlid = @chnlid AND chnluser_userid = @chnluser_userid;
	INSERT INTO chnlchannel (chnlrecid, chnlid, chnlnm, chnlsnm, chnlnode_nodeid, chnluser_userid, chnltransaction_tranid, chnlactive) VALUES (@chnlrecid, @chnlid, @chnlnm, @chnlsnm, @chnlnode_nodeid, @chnluser_userid, @chnltransaction_tranid, @chnlactive);
	UPDATE chnlchannel SET chnlactive = FALSE WHERE chnlrecid = @chnlrecid;
	DELETE FROM chnlchannel WHERE chnlrecid = @chnlrecid")]

	[Credentials("mmalive")]
	public class chnlchannel {
		public long chnlrecid;
		public long chnlid;
		public string chnlnm;
		public string chnlsnm;
		public long chnlnode_nodeid;
		public long chnluser_userid;
		public long chnltransaction_tranid;
		public bool chnlactive;
	}
}
