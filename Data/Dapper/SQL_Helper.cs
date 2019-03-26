using Dapper;
using Data.Dapper.Enums;
using Data.Dapper.Models;
using Data.Repository.Dapper.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Data.Repository.Dapper {
	public static class SQL_Helper {

		public static string AddSearchFilters(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo) {
			add_SearchFilter_Parameters(ref sql, dynamicParameters, pageInfo.Filters, pageInfo.Where);
			// Hacky fix but it was maddening and we needed to get this working
			sql = sql.Replace("  ", " ").Replace("and and", "and");
			return sql;
		}

		public static DynamicParameters GetDynamicParameters<T>(T obj) where T : class {
			return GetDynamicParameters<T>(obj, new List<string>(), new Dictionary<string, DbType>());
		}

		public static DynamicParameters GetDynamicParameters<T>(T obj, List<string> ignoreProperties) where T : class {
			return GetDynamicParameters<T>(obj, ignoreProperties, new Dictionary<string, DbType>());
		}

		public static DynamicParameters GetDynamicParameters<T>(T obj, List<string> ignoreProperties, Dictionary<string, DbType> outputParameters) where T : class {
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => w.CanRead).ToArray();
			if (properties == null || !properties.Any()) {
				return new DynamicParameters();
			}
			var columnNames = ignoreProperties.Any()
									? properties.Select(s => s.Name).Except(ignoreProperties)
									: properties.Select(s => s.Name);
			var _params = new DynamicParameters();
			foreach (string name in columnNames) {
				var val = columnNames.Select(s => properties.Single(a => a.Name.Equals(s)).GetValue(obj));
				_params.Add(name, val);
			}
			if (outputParameters == null || !outputParameters.Any()) {
				return _params;
			}
			foreach (var item in outputParameters) {
				var val = properties.Single(a => a.Name.Equals(item.Key)).GetValue(obj);
				_params.Add(item.Key, dbType: item.Value, direction: ParameterDirection.Output);
			}
			return _params;
		}

		/// <summary>
		/// Creates the string neccessary to call a scalar sql function.
		/// </summary>
		/// <param name="signature">The parameters that make up the signature of the sql function</param>
		/// <param name="databaseSchema">The sql database schema of the function that is being called.</param>
		/// <param name="userDefinedFunctionName">The name of the sql function that is being called</param>
		/// <returns></returns>
		public static string GetScalarFunctionCallSQL(string signature, string databaseSchema, string userDefinedFunctionName) {
			if (string.IsNullOrEmpty(databaseSchema)) { databaseSchema = "dbo"; }
			return $"SELECT [{ databaseSchema }].[{ userDefinedFunctionName }]({ signature })";
		}

		/// <summary>
		/// Creates the string neccessary to call a table-valued sql function.
		/// </summary>
		/// <param name="selectedFields">The fields that should be selected from the result set</param>
		/// <param name="signature">The parameters that make up the signature of the sql function</param>
		/// <param name="databaseSchema">The sql database schema of the function that is being called.</param>
		/// <param name="userDefinedFunctionName">The name of the sql function that is being called</param>
		/// <returns></returns>
		public static string GetTableValuedFunctionCallSQL(string selectedFields, string signature, string databaseSchema, string userDefinedFunctionName) {
			if (string.IsNullOrEmpty(databaseSchema)) { databaseSchema = "dbo"; }
			if (string.IsNullOrEmpty(selectedFields)) { selectedFields = "*"; }
			return $"SELECT { selectedFields } FROM [{ databaseSchema }].[{ userDefinedFunctionName }]({ signature })";
		}

		public static PresentationListPrep PrepForPresentationListResult(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo) {
			getDynamicData_ForPaging(dynamicParameters, pageInfo.PageStart, pageInfo.PageSize);
			add_SearchFilter_Parameters(ref sql, dynamicParameters, pageInfo.Filters, pageInfo.Where);

			if (
				!string.IsNullOrEmpty(pageInfo.Where)
				&& pageInfo.Filters.Count() == 0
			) {
				addWhereIfNeeded(ref sql, pageInfo.Where);
			}
			// Hacky fix but it was maddening and we needed to get this working
			sql = Regex.Replace(sql, @"\sand\sand\s", " and ");
			//trim any stray "and " stuff. Geez, this is getting out of hand.
			sql = Regex.Replace(sql, @"^\s*|and\s$", "");
			//add group by
			if (!string.IsNullOrEmpty(pageInfo.GroupBy)) {
				sql += $" { pageInfo.GroupBy }";
			}

			var query = wrapQueryInRowNumberQuery(sql, pageInfo.OrderBy);
			var countQuery = getCountQuery(sql);
			return new PresentationListPrep {
				TotalQuery = countQuery,
				Query = query,
				BaseQuery = sql
			};
		}

		private static void add_SearchFilter_Parameters(ref string baseQuery, DynamicParameters dynamicParameters, IEnumerable<SearchFilter> searchFilters, string where) {
			if (searchFilters == null || !searchFilters.Any()) {
				return;
			}
			var filterTotal = searchFilters.Count();
			if (filterTotal <= 0) {
				return;
			}
			var filterIndex = 0;
			var acceptableTypes = new List<Type> { typeof(DateTime), typeof(string), typeof(Int16), typeof(Int32), typeof(Int64), typeof(float), typeof(double), typeof(decimal), typeof(bool) };
			var groupedColumnNames = from s in searchFilters
									 where acceptableTypes.Contains(s.Type) && s.AllowGrouping
									 group s by s.Name into grp
									 where grp.Count() > 1
									 select grp.Key;
			if (groupedColumnNames == null) {
				groupedColumnNames = new List<string>();
			}

			var standardFilters = from u in searchFilters
								  where !groupedColumnNames.Contains(u.Name)
								  select u;

			addStandardSearchFilters(ref baseQuery, dynamicParameters, standardFilters, ref filterIndex, filterTotal, where);

			if (groupedColumnNames.Any()) {
				addGroupedSearchFilters(ref baseQuery, dynamicParameters, groupedColumnNames, searchFilters, ref filterIndex, filterTotal, where);
			}
		}

		private static void addFilter(ref string baseQuery, int index, int filterTotal, SearchFilter filter, DynamicParameters dynamicParameters, bool fromGroup) {
			var columnName = filter.Name;
			var filterName = filter.Name + index.ToString();
			var conditionSeperator = getConditionSeperator(index, filterTotal, filter);

			if (fromGroup && (filter.EqualityType == EqualityType.LIKE || filter.EqualityType == EqualityType.NOT_LIKE)) {
				//this prevents a combined like statement from looking like this:
				//WHERE Roles LIKE (Roles LIKE '%Social Media Associate%' OR Roles LIKE '%Monitoring AND Moderation Analyst%' OR Roles LIKE '%Social Media Manager%')
				//it should look like this:
				//WHERE (Roles LIKE '%Social Media Associate%' OR Roles LIKE '%Monitoring AND Moderation Analyst%' OR Roles LIKE '%Social Media Manager%')
				baseQuery += string.Concat(" ", filter.Value);
				return;
			}

			var parameter = string.Empty;//prepareParameter(columnName, filterName, filter.EqualityType, fromGroup);
			var comparator = getComparator(filter.EqualityType, fromGroup);
			var filterName2 = string.Empty;
			if (filter.EqualityType == EqualityType.BETWEEN && fromGroup) {
				index++;
				filterName2 = filter.Name + index.ToString();
				parameter = $" { columnName } { EqualityType.BETWEEN.ToString() } @{ filterName } AND @{ filterName2 }";
			} else {
				parameter = $" { columnName } { comparator } @{ filterName }";
			}

			baseQuery += string.Concat(conditionSeperator, parameter);

			if (filter.Type == typeof(string)) {
				var filterString = (filter.EqualityType == EqualityType.LIKE || filter.EqualityType == EqualityType.NOT_LIKE)
									? prepareForLikeQuery(filter.Value.ToString(), filter.SearchLeftSide, filter.SearchRightSide)
									: filter.Value.ToString();
				dynamicParameters.Add(filterName, filterString);
			} else if (filter.Type == typeof(DateTime) && filter.EqualityType == EqualityType.BETWEEN) {
				var valuesArray = (string[])filter.Value;
				// mz: making the data type explicit to correct filtering problems where dates are being compared as strings
				dynamicParameters.Add(filterName, valuesArray[0], dbType: DbType.DateTime);
				dynamicParameters.Add(filterName2, valuesArray[1], dbType: DbType.DateTime);
			} else {
				dynamicParameters.Add(filterName, filter.Value);
			}
		}

		private static void addGroupedSearchFilters(ref string baseQuery, DynamicParameters dynamicParameters, IEnumerable<string> groupedColumnNames, IEnumerable<SearchFilter> searchFilters, ref int filterIndex, int filterTotal, string where) {
			foreach (var columnName in groupedColumnNames) {
				var filters = searchFilters.Where(a => a.Name == columnName);
				baseQuery = processGroupFilter(baseQuery, dynamicParameters, filterIndex, filterTotal, filters, where);
				filterIndex++;
			}
		}

		private static void addStandardSearchFilters(ref string baseQuery, DynamicParameters dynamicParameters, IEnumerable<SearchFilter> searchFilters, ref int filterIndex, int filterTotal, string where) {
			addWhereIfNeeded(ref baseQuery, where);
			foreach (var filter in searchFilters) {
				if (isFilterValid(filter)) {
					addFilter(ref baseQuery, filterIndex, filterTotal, filter, dynamicParameters, false);
				}
				filterIndex++;
			}
		}

		private static string addWhereIfNeeded(ref string baseQuery, string where) {
			if (!baseQuery.Contains("WHERE")) {
				if (string.IsNullOrEmpty(where)) {
					baseQuery += " WHERE";
				} else {
					baseQuery += $" WHERE { where } and ";
				}
			}
			return baseQuery;
		}

		private static string getComparator(EqualityType equalityType, bool fromGroup) {
			if (!fromGroup && equalityType == EqualityType.IN) {
				//when we only have 1 filter then we use EQUALS instead of IN
				equalityType = EqualityType.EQUALS;
			}
			switch (equalityType) {
				case EqualityType.LIKE:
					return "LIKE";

				case EqualityType.NOT_LIKE:
					return "NOT LIKE";

				case EqualityType.EQUALS:
					return "=";

				case EqualityType.NOT_EQUALS:
					return "!=";

				case EqualityType.GREATER_THAN:
					return ">";

				case EqualityType.LESS_THAN:
					return "<";

				case EqualityType.GREATER_THAN_OR_EQUAL_TO:
					return ">=";

				case EqualityType.LESS_THAN_OR_EQUAL_TO:
					return "<=";

				case EqualityType.BETWEEN:
					return "BETWEEN";

				case EqualityType.IS_NULL:
					return "IS NULL";

				case EqualityType.IS_NOT_NULL:
					return "IS NOT NULL";

				case EqualityType.IN:
					return "IN";

				default:
					throw new Exception("Enum Type Not Matched");
			}
		}

		private static string getConditionSeperator(int index, int filterTotal, SearchFilter filter) {
			return (filterTotal > 1 && index > 0 && index < filterTotal) ? " and" : string.Empty; // Intentionally not using or here because of multiple groupings needing an AND between them
																								  //return (filterTotal > 1 && index > 0 && index < filterTotal) ? filter.ConditionType == ConditionType.AND ? " and" : " or" : string.Empty;
		}

		private static string getCountQuery(string query) {
			if (query.IndexOf("group by", StringComparison.OrdinalIgnoreCase) >= 0) {
				return $"select count(*) as Total from ({ query }) t";
			}
			var back = getQueryFrontAndBack(query).Item2;
			if (back.Contains("order by")) {
				var pos = back.IndexOf("order by");
				back = back.Substring(0, pos);
			}
			return $"select count(*) as Total { back }";
		}

		private static void getDynamicData_ForPaging(DynamicParameters dynamicParameters, int pageStart, int pageSize) {
			if (dynamicParameters == null) {
				dynamicParameters = new DynamicParameters();
			}
			dynamicParameters.Add("rowStart", pageStart + 1);
			var rowEnd = pageSize > 0 ? pageStart + pageSize : 100;//if 0 is passed in, then default to 100. IDK if this is a good idea, but it should work for now.
			dynamicParameters.Add("rowEnd", rowEnd);
		}

		private static Tuple<string, string> getQueryFrontAndBack(string baseQuery) {
			var lastFrom = baseQuery.IndexOf("from", StringComparison.CurrentCultureIgnoreCase);
			var frontPart = baseQuery.Substring(0, lastFrom);
			var backPart = baseQuery.Substring(lastFrom, baseQuery.Length - lastFrom);
			return Tuple.Create<string, string>(frontPart.Trim(), backPart.Trim());
		}

		private static bool isFilterValid(SearchFilter filter) {
			if (filter.Type == typeof(string) && string.IsNullOrEmpty(filter.Value.ToString())) {
				return false;
			}
			if (string.IsNullOrEmpty(filter.Name)) {
				return false;
			}
			//todo: add other things?
			return true;
		}

		private static string prepareForLikeQuery(string searchText, bool searchLeftSide, bool searchRightSide) {
			if (string.IsNullOrEmpty(searchText) || searchText == "%") { return searchText; }
			var value = searchText.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
			if (searchLeftSide) {
				value = "%" + value;
			}
			if (searchRightSide) {
				value += "%";
			}

			return value;
		}

		private static string processGroupFilter(string baseQuery, DynamicParameters dynamicParameters, int filterIndex, int filterTotal, IEnumerable<SearchFilter> filters, string where) {
			var combinedFilter = new SearchFilter(filters);
			if (!isFilterValid(combinedFilter)) {
				return baseQuery;
			}
			addWhereIfNeeded(ref baseQuery, where);

			if (filterIndex > 0) {
				baseQuery += " and";
			}
			addFilter(ref baseQuery, filterIndex, filterTotal, combinedFilter, dynamicParameters, true);
			return baseQuery;
		}

		private static string wrapQueryInRowNumberQuery(string baseQuery, string orderBy) {
			//in order to use the OVER function to retrieve row numbers, you have to specify an order.
			//this is difficult if you don't know the field names involved in the query. ha ha.
			//Pinal Dave suggested what I'm doing below. http://blog.sqlauthority.com/2015/05/05/sql-server-generating-row-number-without-ordering-any-columns/
			//Let's hand it to Pinal Dave for being there unfailingly for all these years.
			if (string.IsNullOrEmpty(orderBy)) {
				orderBy = "(select 1)";
			}
			var frontAndBack = getQueryFrontAndBack(baseQuery);
			//this assumes that the last FROM in your query is the one with the joins and stuff. it helps if you have subselects, but could get hosed in certain circumstances
			var realQuery = $"WITH result AS({ frontAndBack.Item1 }, ROW_NUMBER() OVER (ORDER BY { orderBy }) AS RowNumber { frontAndBack.Item2 }) SELECT * FROM result WHERE RowNumber BETWEEN @rowStart and @rowEnd OPTION(RECOMPILE)";
			return realQuery;
		}
	}
}