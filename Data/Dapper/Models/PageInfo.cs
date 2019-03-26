using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Data.Dapper.Models {
	public class PageInfo {
		public List<SearchFilter> Filters { get; set; } = new List<SearchFilter>();

		public string GroupBy { get; set; } = string.Empty;

		public string OrderBy { get; set; } = string.Empty;

		public int PageSize { get; set; } = 10;
		public int PageStart { get; set; } = 0;
		public bool ReadActive { get; set; } = true;
		public bool ReadInactive { get; set; } = false;
		public List<string> SelectedColumns { get; set; } = new List<string>();
		public string Where { get; set; } = string.Empty;

		public PageInfo() {
		}

		public PageInfo(int pageSize) {
			this.PageSize = pageSize;
		}

		public static PageInfo Build() {
			var pageInfo = new PageInfo();
			return pageInfo;
		}

		public static PageInfo Build(bool readActive, bool readInactive) {
			var pageInfo = new PageInfo();
			pageInfo.ReadActive = readActive;
			pageInfo.ReadInactive = readInactive;
			return pageInfo;
		}

		public static PageInfo Build(string orderBy) {
			var pageInfo = new PageInfo();
			pageInfo.setOrderBy(orderBy);
			return pageInfo;
		}

		public static PageInfo Build(int pageStart, int pageSize, string orderBy) {
			var pageInfo = new PageInfo();
			pageInfo.setOrderBy(orderBy);
			pageInfo.setPageStartAndSize(pageStart, pageSize);
			return pageInfo;
		}

		public static PageInfo Build(SearchFilter searchFilter) {
			var pageInfo = new PageInfo();
			pageInfo.addFilter(searchFilter);
			return pageInfo;
		}

		public static PageInfo Build(List<SearchFilter> searchFilters) {
			var pageInfo = new PageInfo();
			pageInfo.setFilters(searchFilters);
			return pageInfo;
		}

		public static PageInfo Build(SearchFilter searchFilter, string orderBy, string filter) {
			var pageInfo = new PageInfo();
			pageInfo.setAutoFilterWhere(filter);
			pageInfo.addFilter(searchFilter);
			pageInfo.setOrderBy(orderBy);
			return pageInfo;
		}

		public static PageInfo Build(List<SearchFilter> searchFilters, string orderBy) {
			var pageInfo = new PageInfo();
			pageInfo.AddFilters(searchFilters);
			pageInfo.setOrderBy(orderBy);
			return pageInfo;
		}

		public static PageInfo Build(int pageStart, int pageSize, string orderBy, SearchFilter searchFilter) {
			var pageInfo = new PageInfo();
			pageInfo.addFilter(searchFilter);
			pageInfo.setOrderBy(orderBy);
			pageInfo.setPageStartAndSize(pageStart, pageSize);
			return pageInfo;
		}

		public static PageInfo Build(int pageStart, int pageSize, string orderBy, List<SearchFilter> searchFilters) {
			var pageInfo = new PageInfo();
			pageInfo.setFilters(searchFilters);
			pageInfo.setOrderBy(orderBy);
			pageInfo.setPageStartAndSize(pageStart, pageSize);
			return pageInfo;
		}

		public static PageInfo Build(int pageStart, int pageSize, string orderBy, List<SearchFilter> searchFilters, bool readActive, bool readInactive)
		{
			var pageInfo = new PageInfo();
			pageInfo.setFilters(searchFilters);
			pageInfo.setOrderBy(orderBy);
			pageInfo.setPageStartAndSize(pageStart, pageSize);
			pageInfo.setReadActiveAndReadInactive(readActive, readInactive);
			return pageInfo;
		}

		public void AddFilter(SearchFilter searchFilter) {
			this.addFilter(searchFilter);
		}

		public void AddFilters(List<SearchFilter> searchFilters) {
			foreach (var filter in searchFilters) {
				this.addFilter(filter);
			}
		}

		public void SetOrderBy(string orderBy) {
			this.OrderBy = orderBy;
		}

		private void addFilter(SearchFilter searchFilter) {
			this.Filters.Add(searchFilter);
		}

		private void setAutoFilterWhere(string filter) {
			if (string.IsNullOrEmpty(filter)) {
				return;
			}
			var filterDTOs = JsonConvert.DeserializeObject<List<SearchFilterDTO>>(filter);
			if (filterDTOs == null || !filterDTOs.Any()) {
				return;
			}
			var filters = from f in filterDTOs select new SearchFilter(f);
			this.setFilters(filters.ToList());
		}

		private void setFilters(List<SearchFilter> searchFilters) {
			this.Filters.AddRange(searchFilters);
		}

		private void setOrderBy(string orderBy) {
			if (!string.IsNullOrEmpty(orderBy)) {
				this.OrderBy = orderBy;
			}
		}

		private void setOrderBy(string sort, string orderBy) {
			if (!string.IsNullOrEmpty(sort)) {
				if (!string.IsNullOrEmpty(orderBy)) {
					this.OrderBy = string.Concat(sort, ", ", orderBy);
				} else {
					this.OrderBy = sort;
				}
			} else {
				if (!string.IsNullOrEmpty(orderBy)) {
					this.OrderBy = orderBy;
				}
			}
		}

		private void setPageSize(int pageSize) {
			this.PageSize = pageSize == 0 ? int.MaxValue : pageSize;
		}

		private void setPageStart(int pageStart) {
			this.PageStart = pageStart;
		}

		private void setPageStartAndSize(int pageStart, int pageSize) {
			this.setPageStart(pageStart);
			this.setPageSize(pageSize);
		}

		private void setReadActive(bool readActive)
		{
			this.ReadActive = readActive;
		}

		private void setReadInactive(bool readInactive)
		{
			this.ReadInactive = readInactive;
		}

		private void setReadActiveAndReadInactive(bool readActive, bool readInactive)
		{
			this.setReadActive(readActive);
			this.setReadInactive(readInactive);
		}
	}
}