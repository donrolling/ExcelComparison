using System.Collections.Generic;

namespace Data.Repository.Dapper.Base {
	public class SqlBuilder {
		public Dictionary<string, Clauses> Data { get; set; } = new Dictionary<string, Clauses>();
		public int seq { get; set; }

		public SqlBuilder AddParameters(dynamic parameters) {
			AddClause("--parameters", "", parameters, "");
			return this;
		}

		public Template AddTemplate(string sql, dynamic parameters = null) {
			return new Template(this, sql, parameters);
		}

		public SqlBuilder GroupBy(string sql, dynamic parameters = null) {
			AddClause("groupby", sql, parameters, joiner: " , ", prefix: "\nGROUP BY ", postfix: "\n");
			return this;
		}

		public SqlBuilder Having(string sql, dynamic parameters = null) {
			AddClause("having", sql, parameters, joiner: "\nAND ", prefix: "HAVING ", postfix: "\n");
			return this;
		}

		public SqlBuilder InnerJoin(string sql, dynamic parameters = null) {
			AddClause("innerjoin", sql, parameters, joiner: "\nINNER JOIN ", prefix: "\nINNER JOIN ", postfix: "\n");
			return this;
		}

		public SqlBuilder Intersect(string sql, dynamic parameters = null) {
			AddClause("intersect", sql, parameters, joiner: "\nINTERSECT\n ", prefix: "\n ", postfix: "\n");
			return this;
		}

		public SqlBuilder Join(string sql, dynamic parameters = null) {
			AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN ", postfix: "\n");
			return this;
		}

		public SqlBuilder LeftJoin(string sql, dynamic parameters = null) {
			AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
			return this;
		}

		public SqlBuilder OrderBy(string sql, dynamic parameters = null) {
			AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
			return this;
		}

		public SqlBuilder OrWhere(string sql, dynamic parameters = null) {
			AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n", IsInclusive: true);
			return this;
		}

		public SqlBuilder RightJoin(string sql, dynamic parameters = null) {
			AddClause("rightjoin", sql, parameters, joiner: "\nRIGHT JOIN ", prefix: "\nRIGHT JOIN ", postfix: "\n");
			return this;
		}

		public SqlBuilder Select(string sql, dynamic parameters = null) {
			AddClause("select", sql, parameters, " , ", prefix: "", postfix: "\n");
			return this;
		}

		public SqlBuilder Where(string sql, dynamic parameters = null) {
			AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
			return this;
		}

		private void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool IsInclusive = false) {
			Clauses clauses;
			if (!Data.TryGetValue(name, out clauses)) {
				clauses = new Clauses(joiner, prefix, postfix);
				Data[name] = clauses;
			}
			clauses.Add(new Clause { Sql = sql, Parameters = parameters });
			seq++;
		}
	}
}