/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/ActiveState.ts" />
var ControlHelper = /** @class */ (function () {
    function ControlHelper() {
    }
    ControlHelper.ChangeActiveState = function (value) {
        var activeState = new ActiveState();
        switch (value) {
            case 'active':
                activeState.ReadActive = true;
                activeState.ReadInactive = false;
                break;
            case 'inactive':
                activeState.ReadActive = false;
                activeState.ReadInactive = true;
                break;
            case 'both':
                activeState.ReadActive = true;
                activeState.ReadInactive = true;
                break;
            default:
                activeState.ReadActive = true;
                activeState.ReadInactive = false;
                break;
        }
        $("#active").prop("checked", activeState.ReadActive && !activeState.ReadInactive);
        $("#inactive").prop("checked", !activeState.ReadActive && activeState.ReadInactive);
        $("#both").prop("checked", activeState.ReadActive && activeState.ReadInactive);
        return activeState;
    };
    ControlHelper.ReloadGrid = function () {
        var grid = $("#jsGrid");
        grid.jsGrid("loadData");
    };
    return ControlHelper;
}());
//# sourceMappingURL=ControlHelper.js.map