/// <reference path="../Models/ActiveState.ts" />
var ClientPageInfoHelper = /** @class */ (function () {
    function ClientPageInfoHelper() {
    }
    ClientPageInfoHelper.GetClientPageInfo = function (filter, pageSize, activeState) {
        if (!filter.pageIndex) {
            filter.pageIndex = 0;
        }
        var postData = new ClientPageInfo();
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
    };
    return ClientPageInfoHelper;
}());
//# sourceMappingURL=ClientPageInfoHelper.js.map