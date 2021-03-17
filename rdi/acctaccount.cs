using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "acctaccount")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive FROM acctaccount;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Criteria("id", @"SELECT acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctauth, acctstatus, accttransaction_tranid, acctactive FROM acctaccount WHERE acctactive = TRUE AND acctid = @acctid;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Criteria("idfromnodeid", @"SELECT acctid FROM acctaccount INNER JOIN enttentity ON acctentity_enttid = enttid INNER JOIN nodenode on enttnode_nodeid = nodeid WHERE acctactive = TRUE AND enttactive = TRUE AND nodeactive = TRUE AND acctnode_nodeid = @acctnode_nodeid;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Criteria("userlogin", @"SELECT userid, enttid, acctsecret FROM useruser INNER JOIN acctaccount ON useraccount_acctid = acctid INNER JOIN enttentity ON acctentity_enttid = enttid WHERE useractive = TRUE AND acctactive = TRUE AND enttactive = TRUE AND acctnm = @acctnm;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Criteria("userlogin2", @"SELECT userid, enttid FROM useruser INNER JOIN acctaccount ON useraccount_acctid = acctid INNER JOIN enttentity ON acctentity_enttid = enttid WHERE useractive = TRUE AND acctactive = TRUE AND enttactive = TRUE AND acctnm = @acctnm AND acctsecret = @acctsecret;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Criteria("countbynm", @"SELECT count(*) AS recordscount FROM acctaccount WHERE acctactive = TRUE AND acctnm = @acctnm;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(acctid), 0) + 1) AS nextid FROM acctaccount;
	INSERT INTO acctaccount (acctrecid, acctid, acctnm, acctsnm, acctnode_nodeid, acctsource_srceid, acctentity_enttid, acctforeignid, acctsecret, acctauth, acctdaylimit, acctstatus, accttransaction_tranid, acctactive) VALUES (@acctrecid, @acctid, @acctnm, @acctsnm, @acctnode_nodeid, @acctsource_srceid, @acctentity_enttid, @acctforeignid, @acctsecret, @acctauth, @acctdaylimit, @acctstatus, @accttransaction_tranid, @acctactive);
	UPDATE acctaccount SET acctactive = FALSE WHERE acctrecid = @acctrecid;
	DELETE FROM acctaccount WHERE acctrecid = @acctrecid")]

	[Credentials("mmalive")]
	public class acctaccount {
		public long acctrecid;
		public long acctid;
		public string acctnm;
		public string acctsnm;
		public long acctnode_nodeid;
		public long acctsource_srceid;
		public long acctentity_enttid;
		public string acctforeignid;
		public string acctsecret;
		public string acctauth;
		public short acctdaylimit;
		public short acctstatus;
		public long accttransaction_tranid;
		public bool acctactive;

		public long nextid;
		public long recordscount;

		public long userid;
		public long enttid;
	}
}
