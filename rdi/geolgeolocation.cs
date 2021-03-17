using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "geolgeolocation")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT geolrecid, geolid, geolnm, geolnode_nodeid, geoltyp_gltpid, geole, geolx, geoly, geolz, geolr, geolb, geolcity, geolcntry, geolzip, geoltz, geoltransaction_tranid FROM geolgeolocation;
	INSERT INTO geolgeolocation (geolrecid, geolid, geolnm, geolnode_nodeid, geoltyp_gltpid, geole, geolx, geoly, geolz, geolr, geolb, geolcity, geolcntry, geolzip, geoltz, geoltransaction_tranid, geolactive) VALUES (@geolrecid, @geolid, @geolnm, @geolnode_nodeid, @geoltyp_gltpid, @geole, @geolx, @geoly, @geolz, @geolr, @geolb, @geolcity, @geolcntry, @geolzip, @geoltz, @geoltransaction_tranid, @geolactive);
	UPDATE geolgeolocation SET geolactive = 0 WHERE geolrecid = @geolrecid;
	DELETE FROM geolgeolocation WHERE geolrecid = @geolrecid")]

	[Criteria("id", @"SELECT geolrecid, geolid, geolnm, geolnode_nodeid, geoltyp_gltpid, geole, geolx, geoly, geolz, geolr, geolb, geolcity, geolcntry, geolzip, geoltz, geoltransaction_tranid FROM geolgeolocation WHERE geolactive = TRUE AND geolid = @geolid;
	INSERT INTO geolgeolocation (geolrecid, geolid, geolnm, geolnode_nodeid, geoltyp_gltpid, geole, geolx, geoly, geolz, geolr, geolb, geolcity, geolcntry, geolzip, geoltz, geoltransaction_tranid, geolactive) VALUES (@geolrecid, @geolid, @geolnm, @geolnode_nodeid, @geoltyp_gltpid, @geole, @geolx, @geoly, @geolz, @geolr, @geolb, @geolcity, @geolcntry, @geolzip, @geoltz, @geoltransaction_tranid, @geolactive);
	UPDATE geolgeolocation SET geolactive = 0 WHERE geolrecid = @geolrecid;
	DELETE FROM geolgeolocation WHERE geolrecid = @geolrecid")]

	[Criteria("allcountries", @"SELECT geolid, geolnm FROM geolgeolocation WHERE geolactive = TRUE AND geoltyp_gltpid = 1 ORDER BY geolnm;
	INSERT INTO geolgeolocation (geolrecid, geolid, geolnm, geolnode_nodeid, geoltyp_gltpid, geole, geolx, geoly, geolz, geolr, geolb, geolcity, geolcntry, geolzip, geoltz, geoltransaction_tranid, geolactive) VALUES (@geolrecid, @geolid, @geolnm, @geolnode_nodeid, @geoltyp_gltpid, @geole, @geolx, @geoly, @geolz, @geolr, @geolb, @geolcity, @geolcntry, @geolzip, @geoltz, @geoltransaction_tranid, @geolactive);
	UPDATE geolgeolocation SET geolactive = 0 WHERE geolrecid = @geolrecid;
	DELETE FROM geolgeolocation WHERE geolrecid = @geolrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(geolid), 0) + 1) AS nextid FROM geolgeolocation;
	INSERT INTO geolgeolocation (geolrecid, geolid, geolnm, geolnode_nodeid, geoltyp_gltpid, geole, geolx, geoly, geolz, geolr, geolb, geolcity, geolcntry, geolzip, geoltz, geoltransaction_tranid, geolactive) VALUES (@geolrecid, @geolid, @geolnm, @geolnode_nodeid, @geoltyp_gltpid, @geole, @geolx, @geoly, @geolz, @geolr, @geolb, @geolcity, @geolcntry, @geolzip, @geoltz, @geoltransaction_tranid, @geolactive);
	UPDATE geolgeolocation SET geolactive = 0 WHERE geolrecid = @geolrecid;
	DELETE FROM geolgeolocation WHERE geolrecid = @geolrecid")]

	[Credentials("mmalive")]
	public class geolgeolocation {
		public int geolrecid;
		public int geolid;
		public string geolnm;
		public int geolnode_nodeid;
		public short geoltyp_gltpid;
		public bool geole;
		public float geolx;
		public float geoly;
		public int geolz;
		public int geolr;
		public string geolb;
		public string geolcity;
		public string geolcntry;
		public string geolzip;
		public string geoltz;
		public int geoltransaction_tranid;
		public bool geolactive;

		public long nextid;
	}
}
