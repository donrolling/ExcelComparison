using Dapper;
using System.Text.RegularExpressions;

namespace Data.Repository.Dapper.Base {
	public class Template {
		public object Parameters { get { ResolveSql(); return parameters; } }
		public string RawSql { get { ResolveSql(); return rawSql; } }
		private static Regex regex = new Regex(@"\/\*\*.+\*\*\/", RegexOptions.Compiled | RegexOptions.Multiline);
		private readonly SqlBuilder builder;
		private readonly object initParams;
		private readonly string sql;
		private int dataSeq = -1; // Unresolved

		private object parameters;

		private string rawSql;

		public Template(SqlBuilder builder, string sql, dynamic parameters) {
			this.initParams = parameters;
			this.sql = sql;
			this.builder = builder;
		}

		private void ResolveSql() {
			if (dataSeq != builder.seq) {
				DynamicParameters p = new DynamicParameters(initParams);

				rawSql = sql;

				foreach (var pair in builder.Data) {
					rawSql = rawSql.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
				}
				parameters = p;

				// replace all that is left with empty
				rawSql = regex.Replace(rawSql, "");

				dataSeq = builder.seq;
			}
		}
	}
}