using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "edgeedge")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv FROM edgeedge;
	INSERT INTO edgeedge (edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv, edgetransaction_tranid, edgeactive) VALUES (@edgerecid, @edgeid, @edgenm, @edgetyp_edtpid, @edgeprov_nodeid, @edgesub_nodeid, @edgesmt, @edgepriv, @edgetransaction_tranid, @edgeactive);
	UPDATE edgeedge SET edgeactive = FALSE WHERE edgerecid = @edgerecid;
	DELETE FROM edgeedge WHERE edgerecid = @edgerecid")]

	[Criteria("id", @"SELECT edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv FROM edgeedge WHERE edgeactive = TRUE AND edgeid = @edgeid;
	INSERT INTO edgeedge (edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv, edgetransaction_tranid, edgeactive) VALUES (@edgerecid, @edgeid, @edgenm, @edgetyp_edtpid, @edgeprov_nodeid, @edgesub_nodeid, @edgesmt, @edgepriv, @edgetransaction_tranid, @edgeactive);
	UPDATE edgeedge SET edgeactive = FALSE WHERE edgerecid = @edgerecid;
	DELETE FROM edgeedge WHERE edgerecid = @edgerecid")]

	[Criteria("provnodeid1", @"SELECT edgeprov_nodeid FROM edgeedge WHERE edgeactive = TRUE AND edgepriv = @edgepriv AND edgetyp_edtpid = @edgetyp_edtpid AND edgesub_nodeid = @edgesub_nodeid;
	INSERT INTO edgeedge (edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv, edgetransaction_tranid, edgeactive) VALUES (@edgerecid, @edgeid, @edgenm, @edgetyp_edtpid, @edgeprov_nodeid, @edgesub_nodeid, @edgesmt, @edgepriv, @edgetransaction_tranid, @edgeactive);
	UPDATE edgeedge SET edgeactive = FALSE WHERE edgerecid = @edgerecid;
	DELETE FROM edgeedge WHERE edgerecid = @edgerecid")]

	[Criteria("countbynm", @"SELECT count(*) AS recordscount FROM edgeedge WHERE edgeactive = TRUE AND edgenm = @edgenm;
	INSERT INTO edgeedge (edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv, edgetransaction_tranid, edgeactive) VALUES (@edgerecid, @edgeid, @edgenm, @edgetyp_edtpid, @edgeprov_nodeid, @edgesub_nodeid, @edgesmt, @edgepriv, @edgetransaction_tranid, @edgeactive);
	UPDATE edgeedge SET edgeactive = FALSE WHERE edgerecid = @edgerecid;
	DELETE FROM edgeedge WHERE edgerecid = @edgerecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(edgeid), 0) + 1) AS nextid FROM edgeedge;
	INSERT INTO edgeedge (edgerecid, edgeid, edgenm, edgetyp_edtpid, edgeprov_nodeid, edgesub_nodeid, edgesmt, edgepriv, edgetransaction_tranid, edgeactive) VALUES (@edgerecid, @edgeid, @edgenm, @edgetyp_edtpid, @edgeprov_nodeid, @edgesub_nodeid, @edgesmt, @edgepriv, @edgetransaction_tranid, @edgeactive);
	UPDATE edgeedge SET edgeactive = FALSE WHERE edgerecid = @edgerecid;
	DELETE FROM edgeedge WHERE edgerecid = @edgerecid")]

	[Credentials("mmalive")]
	public class edgeedge {
		public long edgerecid;
		public long edgeid;
		public string edgenm;
		public short edgetyp_edtpid;
		public long edgeprov_nodeid;
		public long edgesub_nodeid;
		public float edgesmt;
		public short edgepriv;
		public long edgetransaction_tranid;
		public bool edgeactive;

		public long nextid;
		public long recordscount;
	}
}
