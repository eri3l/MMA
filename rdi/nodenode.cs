using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "nodenode")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"SELECT noderecid, nodeid, nodenm, nodetyp_ndtpid, nodedelta0, nodedelta, nodebirthd, nodevld, noderpn, nodedat, nodegeo_geolid, nodetransaction_tranid, nodeactive, nodebirthdnum, nodesubs, nodesubtyp, nodeprvs, nodeprvtyp FROM nodenode WHERE nodeactive = TRUE;
	INSERT INTO nodenode (noderecid, nodeid, nodenm, nodetyp_ndtpid, nodedelta0, nodedelta, nodebirthd, nodevld, noderpn, nodedat, nodegeo_geolid, nodetransaction_tranid, nodeactive, nodebirthdnum, nodesubs, nodesubtyp, nodeprvs, nodeprvtyp) VALUES (@noderecid, @nodeid, @nodenm, @nodetyp_ndtpid, @nodedelta0, @nodedelta, @nodebirthd, @nodevld, @noderpn, @nodedat, @nodegeo_geolid, @nodetransaction_tranid, @nodeactive, @nodebirthdnum, @nodesubs, @nodesubtyp, @nodeprvs, @nodeprvtyp);
	UPDATE nodenode SET nodeactive = FALSE WHERE noderecid = @noderecid;
	DELETE FROM nodenode WHERE noderecid = @noderecid")]

	[Criteria("id", @"SELECT noderecid, nodeid, nodenm, nodetyp_ndtpid, nodedelta0, nodedelta, nodebirthd, nodevld, noderpn, nodedat, nodegeo_geolid, nodetransaction_tranid, nodeactive, nodebirthdnum, nodesubs, nodesubtyp, nodeprvs, nodeprvtyp FROM nodenode WHERE nodeactive = TRUE AND nodeid = @nodeid;
	INSERT INTO nodenode (noderecid, nodeid, nodenm, nodetyp_ndtpid, nodedelta0, nodedelta, nodebirthd, nodevld, noderpn, nodedat, nodegeo_geolid, nodetransaction_tranid, nodeactive, nodebirthdnum, nodesubs, nodesubtyp, nodeprvs, nodeprvtyp) VALUES (@noderecid, @nodeid, @nodenm, @nodetyp_ndtpid, @nodedelta0, @nodedelta, @nodebirthd, @nodevld, @noderpn, @nodedat, @nodegeo_geolid, @nodetransaction_tranid, @nodeactive, @nodebirthdnum, @nodesubs, @nodesubtyp, @nodeprvs, @nodeprvtyp);
	UPDATE nodenode SET nodeactive = FALSE WHERE noderecid = @noderecid;
	DELETE FROM nodenode WHERE noderecid = @noderecid")]

	[Criteria("countbynm", @"SELECT count(*) AS recordscount FROM nodenode WHERE nodeactive = TRUE AND nodenm = @nodenm;
	INSERT INTO nodenode (noderecid, nodeid, nodenm, nodetyp_ndtpid, nodedelta0, nodedelta, nodebirthd, nodevld, noderpn, nodedat, nodegeo_geolid, nodetransaction_tranid, nodeactive, nodebirthdnum, nodesubs, nodesubtyp, nodeprvs, nodeprvtyp) VALUES (@noderecid, @nodeid, @nodenm, @nodetyp_ndtpid, @nodedelta0, @nodedelta, @nodebirthd, @nodevld, @noderpn, @nodedat, @nodegeo_geolid, @nodetransaction_tranid, @nodeactive, @nodebirthdnum, @nodesubs, @nodesubtyp, @nodeprvs, @nodeprvtyp);
	UPDATE nodenode SET nodeactive = FALSE WHERE noderecid = @noderecid;
	DELETE FROM nodenode WHERE noderecid = @noderecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(nodeid), 0) + 1) AS nextid FROM nodenode;
	INSERT INTO nodenode (noderecid, nodeid, nodenm, nodetyp_ndtpid, nodedelta0, nodedelta, nodebirthd, nodevld, noderpn, nodedat, nodegeo_geolid, nodetransaction_tranid, nodeactive, nodebirthdnum, nodesubs, nodesubtyp, nodeprvs, nodeprvtyp) VALUES (@noderecid, @nodeid, @nodenm, @nodetyp_ndtpid, @nodedelta0, @nodedelta, @nodebirthd, @nodevld, @noderpn, @nodedat, @nodegeo_geolid, @nodetransaction_tranid, @nodeactive, @nodebirthdnum, @nodesubs, @nodesubtyp, @nodeprvs, @nodeprvtyp);
	UPDATE nodenode SET nodeactive = FALSE WHERE noderecid = @noderecid;
	DELETE FROM nodenode WHERE noderecid = @noderecid")]

	[Credentials("mmalive")]
	public class nodenode {
		public long noderecid;
		public long nodeid;
		public string nodenm;
		public short nodetyp_ndtpid;
		public DateTime nodedelta0;
		public DateTime nodedelta;
		public DateTime nodebirthd;
		public float nodevld;
		public float noderpn;
		public string nodedat;
		public long nodegeo_geolid;
		public long nodetransaction_tranid;
		public bool nodeactive;
		public long nodebirthdnum;
		public string nodesubs;
		public string nodesubtyp;
		public string nodeprvs;
		public string nodeprvtyp;

		public long nextid;
		public long recordscount;
	}
}