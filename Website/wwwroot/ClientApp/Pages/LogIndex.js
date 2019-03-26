/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/ClientPageInfo.ts" />
/// <reference path="../Models/ActiveState.ts" />
/// <reference path="../Shared/ControlHelper.ts" />
/// <reference path="../Shared/ClientPageInfoHelper.ts" />
var LogIndex = /** @class */ (function () {
    function LogIndex() {
        this._dataUrl = '/log/readall';
        this._pageSize = 10;
        this._activeState = new ActiveState();
        this.setupGrid();
    }
    LogIndex.prototype.setupGrid = function () {
        var self = this;
        var grid = $("#jsGrid");
        var tokenelement = $('input[name="__RequestVerificationToken"]');
        var token = tokenelement.val();
        grid.jsGrid({
            width: "100%",
            height: "400px",
            inserting: false,
            editing: false,
            sorting: true,
            paging: true,
            datatype: 'json',
            autoload: true,
            pageLoading: true,
            pageSize: self._pageSize,
            controller: {
                loadData: function (filter) {
                    var deferred = $.Deferred();
                    if (typeof filter.sortField == 'undefined') {
                        filter.sortField = 'id';
                        filter.sortOrder = 'asc';
                    }
                    var postData = ClientPageInfoHelper.GetClientPageInfo(filter, self._pageSize, self._activeState);
                    $.ajax({
                        type: 'POST',
                        url: self._dataUrl,
                        data: {
                            __RequestVerificationToken: token,
                            clientPageInfo: postData
                        },
                        dataType: 'json'
                    }).done(function (response) {
                        deferred.resolve({ data: response.Data, itemsCount: response.Total });
                    });
                    return deferred.promise();
                }
            },
            fields: [
                {
                    type: "control",
                    width: 100,
                    headerTemplate: function () {
                        return $("<span>").text("Subject Name");
                    },
                    itemTemplate: function (value, item) {
                        var $customAnchor = $("<a>")
                            .attr({ href: "/TransmissionError/Index/" + item.SubjectRouteId })
                            .attr({ target: "_blank" })
                            .append(item.SubjectName);
                        return $("<div>").append($customAnchor);
                    }
                },
                { name: "Route", title: "Route", type: "text", width: 250, validate: "required" },
                { name: "Body", title: "Body", type: "textarea", width: 200 },
                { name: "SentSuccessfully", title: "Sent Successfully", type: "checkbox", width: 75, validate: "required" },
                {
                    name: "EventMessageCreatedDate", title: "Created Date", type: "date", width: 100,
                    itemTemplate: function (value, item) {
                        var date = new Date(Date.parse(item.EventMessageCreatedDate));
                        return date.toLocaleString();
                    }
                }
            ]
        });
    };
    LogIndex.prototype.changeActiveState = function (value) {
        this._activeState = ControlHelper.ChangeActiveState(value);
        ControlHelper.ReloadGrid();
    };
    return LogIndex;
}());
//# sourceMappingURL=LogIndex.js.map