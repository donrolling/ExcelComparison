/// <reference path="../Models/ActiveState.ts" />

class ClientPageInfoHelper {
	public static GetClientPageInfo(filter: any, pageSize: number, activeState: ActiveState): ClientPageInfo {
		if (!filter.pageIndex) {
			filter.pageIndex = 0;
		}
		var postData: ClientPageInfo = new ClientPageInfo();
		postData.PageStart = (filter.pageIndex - 1) * pageSize;
		postData.PageSize = filter.pageIndex * pageSize;		
		postData.OrderBy = filter.sortField + ' ' + filter.sortOrder;
		postData.ReadActive = activeState.ReadActive;
		postData.ReadInactive = activeState.ReadInactive;
		// page: filter.pageIndex,
		// sort: filter.sortField,
		// order: filter.sortOrder
		console.log(filter);
		console.log(postData);
		return postData;
	}
}