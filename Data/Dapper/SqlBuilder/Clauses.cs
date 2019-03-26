using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repository.Dapper.Base {
	public class Clauses : List<Clause> {
		private string joiner;
		private string postfix;
		private string prefix;

		public Clauses(string joiner, string prefix = "", string postfix = "") {
			this.joiner = joiner;
			this.prefix = prefix;
			this.postfix = postfix;
		}

		public string ResolveClauses(DynamicParameters p) {
			foreach (var item in this) {
				p.AddDynamicParams(item.Parameters);
			}
			return this.Any(a => a.IsInclusive)
				? prefix +
				  string.Join(joiner,
					  this.Where(a => !a.IsInclusive)
						  .Select(c => c.Sql)
						  .Union(new[]
						  {
								  " ( " +
								  string.Join(" OR ", this.Where(a => a.IsInclusive).Select(c => c.Sql).ToArray()) +
								  " ) "
						  })) + postfix
				: prefix + string.Join(joiner, this.Select(c => c.Sql)) + postfix;
		}
	}
}