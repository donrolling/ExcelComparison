/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/SelectListItem.ts" />
/// <reference path="../Shared/ControlHelper.ts" />
var RoleCreateUpdate = /** @class */ (function () {
    function RoleCreateUpdate(allPermissions, currentSelection) {
        this._allPermissions = allPermissions;
        this._currentSelection = currentSelection;
    }
    RoleCreateUpdate.prototype.addAllPermissions = function () {
        this._currentSelection = [].concat(this._allPermissions);
        this.setSelectedPermissions();
    };
    RoleCreateUpdate.prototype.addPermissions = function () {
        var options = $("#availablepermissions option:selected");
        for (var i = 0; i < options.length; i++) {
            var item = { Text: options[i].innerHTML, Value: options[i].getAttribute("value") };
            this._currentSelection.push(item);
        }
        this.setSelectedPermissions();
    };
    RoleCreateUpdate.prototype.removeAllPermissions = function () {
        this._currentSelection = [];
        this.setSelectedPermissions();
    };
    RoleCreateUpdate.prototype.removePermissions = function () {
        var self = this;
        var options = $("#selectedpermissions option:selected");
        for (var i = 0; i < options.length; i++) {
            var items = this._currentSelection.filter(function (item) {
                return options[i].innerHTML === item.Text && options[i].getAttribute("value") === item.Value;
            });
            if (items.length > 0) {
                $.each(items, function (i, e) {
                    var index = self._currentSelection.indexOf(e);
                    if (index >= 0) {
                        self._currentSelection.splice(index, 1);
                    }
                });
            }
        }
        this.setSelectedPermissions();
    };
    RoleCreateUpdate.prototype.updateListboxes = function () {
        var self = this;
        $("#selectedpermissions option").remove();
        $("#availablepermissions option").remove();
        $.each(this._allPermissions, function (i, e) {
            var items = self._currentSelection.filter(function (item) {
                return e.Text === item.Text && e.Value === item.Value;
            });
            if (items.length > 0) {
                $("#selectedpermissions").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
            }
            else {
                $("#availablepermissions").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
            }
        });
    };
    RoleCreateUpdate.prototype.setSelectedPermissions = function () {
        this.updateListboxes();
        $("#PermissionIds option:selected").prop("selected", false);
        $.each(this._currentSelection, function (i, e) {
            $("#PermissionIds option[value='" + e.Value + "']").prop("selected", true);
        });
    };
    return RoleCreateUpdate;
}());
//# sourceMappingURL=RoleCreateUpdate.js.map