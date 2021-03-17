using System;
using bs.item.attr.Attributes;

namespace app.mma.rdi {
	[NativeDialect("tablename", "seqisequences")]
	[NativeDialect("version", "pgsql")]	

	[Criteria("default", @"select seqiseq_id, seqiseqname, seqitablename, seqiincfactor, seqinumber from seqisequences;
	insert into seqisequences (seqiseq_id, seqiseqname, seqitablename, seqiincfactor, seqinumber) values (@seqiseq_id, @seqiseqname, @seqitablename, @seqiincfactor, @seqinumber);
	update seqisequences set seqinumber=seqinumber+seqiincfactor where seqitablename=@seqitablename;
	delete from seqisequences where seqiseq_id=@seqiseq_id")]

	[Criteria("seqForTable", @"select seqiseq_id, seqiseqname, seqitablename, seqiincfactor, seqinumber from seqisequences where seqitablename=@seqitablename;
	insert into seqisequences (seqiseq_id, seqiseqname, seqitablename, seqiincfactor, seqinumber) values (@seqiseq_id, @seqiseqname, @seqitablename, @seqiincfactor, @seqinumber);
	update seqisequences set seqinumber=seqinumber+seqiincfactor where seqitablename=@seqitablename;
	delete from seqisequences where seqiseq_id=@seqiseq_id")]

	[Bind.Alias.Provider("seqisequences")]
	[Credentials("mmalive")]
	public class seqisequences {
		[Field.Constraint(Field.ConstraintType.PKey)]
		[Field.Constraint(Field.ConstraintType.NotNull)]
		public int seqiseq_id;
		public string seqiseqname;
		public string seqitablename;
		public int seqiincfactor;
		public long seqinumber;
	}
}