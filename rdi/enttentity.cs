using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "enttentity")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid FROM enttentity WHERE enttactive = TRUE;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("id", @"SELECT enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid FROM enttentity WHERE enttactive = TRUE AND enttid = @enttid;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("getnmbyid", @"SELECT enttnm FROM enttentity WHERE enttactive = TRUE AND enttid = @enttid;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("getnmsnmbyid", @"SELECT enttnm, enttsnm FROM enttentity WHERE enttactive = TRUE AND enttid = @enttid;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("getfrfollbygeoagegender", @"SELECT enttsubs.enttid, enttsubs.enttsnm as enttsnm, geolcntry FROM chnlchannel INNER JOIN edgeedge AS edgechnl ON edgechnl.edgesub_nodeid = chnlnode_nodeid INNER JOIN acctaccount ON acctnode_nodeid = edgeprov_nodeid INNER JOIN enttentity AS enttacct ON enttacct.enttid = acctentity_enttid INNER JOIN edgeedge AS edgesubs ON edgesubs.edgeprov_nodeid = enttacct.enttnode_nodeid INNER JOIN nodenode AS nodesubs ON nodesubs.nodeid = edgesubs.edgesub_nodeid INNER JOIN enttentity AS enttsubs ON enttsubs.enttnode_nodeid = nodesubs.nodeid INNER JOIN geolgeolocation ON geolid = nodesubs.nodegeo_geolid WHERE chnlactive = TRUE AND edgechnl.edgeactive = TRUE AND acctactive = TRUE AND enttacct.enttactive = TRUE AND edgesubs.edgeactive = TRUE AND nodesubs.nodeactive = TRUE AND enttsubs.enttactive = TRUE AND geolactive = TRUE AND chnlid = @chnlid AND chnluser_userid = @chnluser_userid AND edgechnl.edgetyp_edtpid = 9 AND (edgesubs.edgepriv & @edgepriv != 0) AND edgesubs.edgetyp_edtpid IN (1, 2) AND geole = TRUE AND enttsubs.enttgender = @enttgender AND (geolid = @geolid OR (geolx >= @geolxmin AND geolx <= @geolxmax AND geoly >= @geolymin AND geoly <= @geolymax)) AND nodebirthdnum >= @nodebirthdnummin AND nodebirthdnum <= @nodebirthdnummax ORDER BY enttsubs.enttsnm;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("getfrfollbygeogender", @"SELECT enttsubs.enttid, enttsubs.enttsnm as enttsnm, geolcntry FROM chnlchannel INNER JOIN edgeedge AS edgechnl ON edgechnl.edgesub_nodeid = chnlnode_nodeid INNER JOIN acctaccount ON acctnode_nodeid = edgeprov_nodeid INNER JOIN enttentity AS enttacct ON enttacct.enttid = acctentity_enttid INNER JOIN edgeedge AS edgesubs ON edgesubs.edgeprov_nodeid = enttacct.enttnode_nodeid INNER JOIN nodenode AS nodesubs ON nodesubs.nodeid = edgesubs.edgesub_nodeid INNER JOIN enttentity AS enttsubs ON enttsubs.enttnode_nodeid = nodesubs.nodeid INNER JOIN geolgeolocation ON geolid = nodesubs.nodegeo_geolid WHERE chnlactive = TRUE AND edgechnl.edgeactive = TRUE AND acctactive = TRUE AND enttacct.enttactive = TRUE AND edgesubs.edgeactive = TRUE AND nodesubs.nodeactive = TRUE AND enttsubs.enttactive = TRUE AND geolactive = TRUE AND chnlid = @chnlid AND chnluser_userid = @chnluser_userid AND edgechnl.edgetyp_edtpid = 9 AND (edgesubs.edgepriv & @edgepriv != 0) AND edgesubs.edgetyp_edtpid IN (1, 2) AND geole = TRUE AND enttsubs.enttgender = @enttgender AND (geolid = @geolid OR (geolx >= @geolxmin AND geolx <= @geolxmax AND geoly >= @geolymin AND geoly <= @geolymax)) ORDER BY enttsubs.enttsnm;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("getfrfollbygeoage", @"SELECT enttsubs.enttid, enttsubs.enttsnm as enttsnm, geolcntry FROM chnlchannel INNER JOIN edgeedge AS edgechnl ON edgechnl.edgesub_nodeid = chnlnode_nodeid INNER JOIN acctaccount ON acctnode_nodeid = edgeprov_nodeid INNER JOIN enttentity AS enttacct ON enttacct.enttid = acctentity_enttid INNER JOIN edgeedge AS edgesubs ON edgesubs.edgeprov_nodeid = enttacct.enttnode_nodeid INNER JOIN nodenode AS nodesubs ON nodesubs.nodeid = edgesubs.edgesub_nodeid INNER JOIN enttentity AS enttsubs ON enttsubs.enttnode_nodeid = nodesubs.nodeid INNER JOIN geolgeolocation ON geolid = nodesubs.nodegeo_geolid WHERE chnlactive = TRUE AND edgechnl.edgeactive = TRUE AND acctactive = TRUE AND enttacct.enttactive = TRUE AND edgesubs.edgeactive = TRUE AND nodesubs.nodeactive = TRUE AND enttsubs.enttactive = TRUE AND geolactive = TRUE AND chnlid = @chnlid AND chnluser_userid = @chnluser_userid AND edgechnl.edgetyp_edtpid = 9 AND (edgesubs.edgepriv & @edgepriv != 0) AND edgesubs.edgetyp_edtpid IN (1, 2) AND geole = TRUE AND (geolid = @geolid OR (geolx >= @geolxmin AND geolx <= @geolxmax AND geoly >= @geolymin AND geoly <= @geolymax)) AND nodebirthdnum >= @nodebirthdnummin AND nodebirthdnum <= @nodebirthdnummax ORDER BY enttsubs.enttsnm;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("getfrfollbygeo", @"SELECT enttsubs.enttid, enttsubs.enttsnm as enttsnm, geolcntry FROM chnlchannel INNER JOIN edgeedge AS edgechnl ON edgechnl.edgesub_nodeid = chnlnode_nodeid INNER JOIN acctaccount ON acctnode_nodeid = edgeprov_nodeid INNER JOIN enttentity AS enttacct ON enttacct.enttid = acctentity_enttid INNER JOIN edgeedge AS edgesubs ON edgesubs.edgeprov_nodeid = enttacct.enttnode_nodeid INNER JOIN nodenode AS nodesubs ON nodesubs.nodeid = edgesubs.edgesub_nodeid INNER JOIN enttentity AS enttsubs ON enttsubs.enttnode_nodeid = nodesubs.nodeid INNER JOIN geolgeolocation ON geolid = nodesubs.nodegeo_geolid WHERE chnlactive = TRUE AND edgechnl.edgeactive = TRUE AND acctactive = TRUE AND enttacct.enttactive = TRUE AND edgesubs.edgeactive = TRUE AND nodesubs.nodeactive = TRUE AND enttsubs.enttactive = TRUE AND geolactive = TRUE AND chnlid = @chnlid AND chnluser_userid = @chnluser_userid AND edgechnl.edgetyp_edtpid = 9 AND (edgesubs.edgepriv & @edgepriv != 0) AND edgesubs.edgetyp_edtpid IN (1, 2) AND geole = TRUE AND (geolid = @geolid OR (geolx >= @geolxmin AND geolx <= @geolxmax AND geoly >= @geolymin AND geoly <= @geolymax)) ORDER BY enttsubs.enttsnm;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("searchmsghstnodaterange", @"SELECT msgqid, entttarg.enttsnm AS enttsnm, msgqtext, msgqdatestart FROM useruser INNER JOIN acctaccount ON useraccount_acctid = acctid INNER JOIN enttentity AS enttsrc ON acctentity_enttid = enttsrc.enttid INNER JOIN msgqmessagequeue ON msgqprov_enttid = enttsrc.enttid INNER JOIN enttentity AS entttarg ON msgqtarg_enttid = entttarg.enttid WHERE useractive = TRUE AND acctactive = TRUE AND enttsrc.enttactive = TRUE AND msgqactive = TRUE AND entttarg.enttactive = TRUE AND userid = @userid AND msgqprov_enttid = @msgqprov_enttid AND msgqtarg_enttid = @msgqtarg_enttid ORDER BY msgqdatestart DESC;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("countbynm", @"SELECT count(*) AS recordscount FROM enttentity WHERE enttactive = TRUE AND enttnm = @enttnm;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("enttsctpbyid", @"SELECT enttnm, enttsnm, sctpnm FROM enttentity INNER JOIN acctaccount ON acctentity_enttid = enttid INNER JOIN srcesource ON acctsource_srceid = srceid INNER JOIN sctpsourcetype ON srcetyp_sctpid = sctpid WHERE enttactive = TRUE AND acctactive = TRUE AND srceactive = TRUE AND sctpactive = TRUE AND enttid = @enttid AND srceid = @srceid;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(enttid), 0) + 1) AS nextid FROM enttentity;
	INSERT INTO enttentity (enttrecid, enttid, enttnm, enttnode_nodeid, enttsnm, enttsmt, enttgender, enttlang_langid, enttkeywords, enttpic_docuid, entttransaction_tranid, enttactive) VALUES (@enttrecid, @enttid, @enttnm, @enttnode_nodeid, @enttsnm, @enttsmt, @enttgender, @enttlang_langid, @enttkeywords, @enttpic_docuid, @entttransaction_tranid, @enttactive);
	UPDATE enttentity SET enttactive = FALSE WHERE enttrecid = @enttrecid;
	DELETE FROM enttentity WHERE enttrecid = @enttrecid")]

	[Credentials("mmalive")]
	public class enttentity {
		public long enttrecid;
		public long enttid;
		public string enttnm;
		public long enttnode_nodeid;
		public string enttsnm;
		public float enttsmt;
		public char enttgender;
		public long enttlang_langid;
		public string enttkeywords;
		public long enttpic_docuid;
		public long entttransaction_tranid;
		public bool enttactive;

		public long nextid;
		public long recordscount;

		public long nodeid;
		public float noderpn;
		public long nodebirthdnum;
		public long nodebirthdnummin;
		public long nodebirthdnummax;

		public int geolid;
		public string geolcntry;
		public float geolx;
		public float geoly;
		public float geolxmin;
		public float geolxmax;
		public float geolymin;
		public float geolymax;

		public long msgqid;
		public long msgqprov_enttid;
		public long msgqtarg_enttid;
		public string msgqtext;
		public DateTime msgqdatestart;

		public long chnlid;
		public long chnluser_userid;

		public long userid;

		public short edgepriv;

		public long srceid;

		public string sctpnm;
	}
}
