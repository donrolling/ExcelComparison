/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/ClientPageInfo.ts" />
/// <reference path="../Models/ActiveState.ts" />
/// <reference path="../Shared/ControlHelper.ts" />
/// <reference path="../Shared/ClientPageInfoHelper.ts" />
var RoleIndex = /** @class */ (function () {
    function RoleIndex() {
        this._dataUrl = '/role/readall';
        this._pageSize = 10;
        this._activeState = new ActiveState();
        this.setupGrid();
    }
    RoleIndex.prototype.setupGrid = function () {
        var self = this;
        var grid = $("#jsGrid");
        var tokenelement = $('input[name="__RequestVerificationToken"]');
        var token = tokenelement.val();
        grid.jsGrid({
            width: "100%",
            height: "600px",
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
                { name: "Name", title: "Role Name", type: "text", width: 100, validate: "required" },
                {
                    type: "control",
                    width: 50,
                    itemTemplate: function (value, item) {
                        var $iconPencil = $("<i>").attr({ class: "far fa-edit" });
                        var $iconTrash = $("<i>").attr({ class: "far fa-trash-alt" });
                        var $customEditButton = $("<a>")
                            .attr({ class: "customButton btn btn-primary btn-xs" })
                            .attr({ href: "/Role/Update/" + item.Id })
                            .attr({ title: "Update Role" })
                            .append($("<span>").append($iconPencil));
                        var $customDeleteButton = $("<a>")
                            .attr({ class: "customButton btn btn-danger btn-xs" })
                            .attr({ href: "/Role/Delete/" + item.Id })
                            .attr({ title: "Delete Role" })
                            .append($("<span>").append($iconTrash));
                        return $("<div>").append($customEditButton).append($customDeleteButton);
                    }
                }
            ]
        });
    };
    RoleIndex.prototype.changeActiveState = function (value) {
        this._activeState = ControlHelper.ChangeActiveState(value);
        ControlHelper.ReloadGrid();
    };
    return RoleIndex;
}());
//# sourceMappingURL=RoleIndex.js.map