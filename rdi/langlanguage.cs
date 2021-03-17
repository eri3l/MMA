using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "langlanguage")]
	[NativeDialect("version", "pgsql")]

	[Criteria("default", @"
	SELECT langrecid, langid, langnm, langtransaction_tranid, langactive FROM langlanguage;
	INSERT INTO langlanguage (langrecid, langid, langnm, langtransaction_tranid, langactive) VALUES (@langrecid, @langid, @langnm, @langtransaction_tranid, @langactive);
	UPDATE langlanguage SET langactive = FALSE WHERE langrecid = @langrecid;
	DELETE FROM langlanguage WHERE langrecid = @langrecid")]

	// TODO: check this - we need it to return the matching id for given lang 'nm' value (i.e. en, or en-uk etc )
	[Criteria("idbynm",  @"SELECT langid FROM langlanguage WHERE langactive = TRUE AND langnm = @langnm;
	INSERT INTO langlanguage (langrecid, langid, langnm, langtransaction_tranid, langactive) VALUES (@langrecid, @langid, @langnm, @langtransaction_tranid, @langactive);
	UPDATE langlanguage SET langactive = FALSE WHERE langrecid = @langrecid;
	DELETE FROM langlanguage WHERE langrecid = @langrecid")]

	[Criteria("id", @"SELECT langrecid, langid, langnm, langtransaction_tranid, langactive FROM langlanguage WHERE langactive = TRUE AND langid = @langid;
	INSERT INTO langlanguage (langrecid, langid, langnm, langtransaction_tranid, langactive) VALUES (@langrecid, @langid, @langnm, @langtransaction_tranid, @langactive);
	UPDATE langlanguage SET langactive = FALSE WHERE langrecid = @langrecid;
	DELETE FROM langlanguage WHERE langrecid = @langrecid")]

	[Criteria("nextid", @"SELECT (COALESCE(max(langid), 0) + 1) AS nextid FROM langlanguage;
	INSERT INTO langlanguage (langrecid, langid, langnm, langtransaction_tranid, langactive) VALUES (@langrecid, @langid, @langnm, @langtransaction_tranid, @langactive);
	UPDATE langlanguage SET langactive = FALSE WHERE langrecid = @langrecid;
	DELETE FROM langlanguage WHERE langrecid = @langrecid")]

	[Credentials("mmalive")]
	public class langlanguage {
		public long langrecid;
		public long langid;
		public string langnm;
		public long langtransaction_tranid;
		public bool langactive;

		public long nextid;
	}
}
