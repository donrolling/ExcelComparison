using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Dapper.Models {
	public class ClientPageInfo {
		public IEnumerable<ClientSearchFilter> Filters { get; set; }

		public string OrderBy { get; set; }

		public int PageSize { get; set; }

		public int PageStart { get; set; }

		public bool ReadActive { get; set; } = true;
		
		public bool ReadInactive { get; set; } = false;

		public ClientPageInfo() { }

		public static PageInfo ConvertToPageInfo(ClientPageInfo clientPageInfo) {
			var filters = (clientPageInfo.Filters == null || !clientPageInfo.Filters.Any()) ? new List<SearchFilter>() : clientPageInfo.Filters.Select(a => ClientSearchFilter.ConvertToSearchFilter(a)).ToList();
			return PageInfo.Build(clientPageInfo.PageStart, clientPageInfo.PageSize, clientPageInfo.OrderBy, filters, clientPageInfo.ReadActive, clientPageInfo.ReadInactive);
		}
	}

	public class ClientSearchFilter {
		public string ConditionType { get; set; } = "AND";
		public string EqualityType { get; set; } = "EQUALS";
		public string Name { get; set; }
		public bool SearchLeftSide { get; set; } = false;
		public bool SearchRightSide { get; set; } = true;
		public string Type { get; set; } = "string";
		public object Value { get; set; }

		public ClientSearchFilter() { }

		public static SearchFilter ConvertToSearchFilter(ClientSearchFilter clientSearchFilter) {
			var conditionType = Data.Dapper.Enums.ConditionType.AND;
			Enum.TryParse<Data.Dapper.Enums.ConditionType>(clientSearchFilter.ConditionType, out conditionType);
			var equalityType = Data.Dapper.Enums.EqualityType.EQUALS;
			Enum.TryParse<Data.Dapper.Enums.EqualityType>(clientSearchFilter.EqualityType, out equalityType);
			var searchFilter = new SearchFilter(clientSearchFilter.Name, clientSearchFilter.Value, conditionType, equalityType, false, false);
			searchFilter.SearchLeftSide = clientSearchFilter.SearchLeftSide;
			searchFilter.SearchRightSide = clientSearchFilter.SearchRightSide;
			return searchFilter;
		}
	}
}