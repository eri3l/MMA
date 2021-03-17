using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "useruser")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT userrecid, userid, usernm, usernode_nodeid, useraccount_acctid, userprefs, userstate, usertransaction_tranid, useractive, userdaylimit FROM useruser WHERE useractive = TRUE;
	INSERT INTO useruser (userrecid, userid, usernm, usernode_nodeid, useraccount_acctid, userprefs, userstate, usertransaction_tranid, useractive, userdaylimit) VALUES (@userrecid, @userid, @usernm, @usernode_nodeid, @useraccount_acctid, @userprefs, @userstate, @usertransaction_tranid, @useractive, @userdaylimit);
	UPDATE useruser SET useractive = FALSE WHERE userrecid = @userrecid;
	DELETE FROM useruser WHERE userrecid = @userrecid")]

	[Criteria("id", @"SELECT userrecid, userid, usernm, usernode_nodeid, useraccount_acctid, userprefs, userstate, usertransaction_tranid, useractive, userdaylimit FROM useruser WHERE useractive = TRUE AND userid = @userid;
	INSERT INTO useruser (userrecid, userid, usernm, usernode_nodeid, useraccount_acctid, userprefs, userstate, usertransaction_tranid, useractive, userdaylimit) VALUES (@userrecid, @userid, @usernm, @usernode_nodeid, @useraccount_acctid, @userprefs, @userstate, @usertransaction_tranid, @useractive, @userdaylimit);
	UPDATE useruser SET useractive = FALSE WHERE userrecid = @userrecid;
	DELETE FROM useruser WHERE userrecid = @userrecid")]

	[Criteria("countbynm", @"SELECT count(*) AS recordscount FROM useruser WHERE useractive = TRUE AND usernm = @usernm;
	INSERT INTO useruser (userrecid, userid, usernm, usernode_nodeid, useraccount_acctid, userprefs, userstate, usertransaction_tranid, useractive, userdaylimit) VALUES (@userrecid, @userid, @usernm, @usernode_nodeid, @useraccount_acctid, @userprefs, @userstate, @usertransaction_tranid, @useractive, @userdaylimit);
	UPDATE useruser SET useractive = FALSE WHERE userrecid = @userrecid;
	DELETE FROM useruser WHERE userrecid = @userrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(userid), 0) + 1) AS nextid FROM useruser;
	INSERT INTO useruser (userrecid, userid, usernm, usernode_nodeid, useraccount_acctid, userprefs, userstate, usertransaction_tranid, useractive, userdaylimit) VALUES (@userrecid, @userid, @usernm, @usernode_nodeid, @useraccount_acctid, @userprefs, @userstate, @usertransaction_tranid, @useractive, @userdaylimit);
	UPDATE useruser SET useractive = FALSE WHERE userrecid = @userrecid;
	DELETE FROM useruser WHERE userrecid = @userrecid")]

	[Credentials("mmalive")]
	public class useruser {
		public long userrecid;
		public long userid;
		public string usernm;
		public long usernode_nodeid;
		public long useraccount_acctid;
		public string userprefs;
		public int userstate;
		public long usertransaction_tranid;
		public bool useractive;
		public short userdaylimit;

		public long nextid;
		public long recordscount;
	}
}