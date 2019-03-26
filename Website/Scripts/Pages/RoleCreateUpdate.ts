/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/SelectListItem.ts" />
/// <reference path="../Shared/ControlHelper.ts" />

class RoleCreateUpdate {
	private _allPermissions: SelectListItem[];
	private _currentSelection: SelectListItem[];

	constructor(allPermissions: SelectListItem[], currentSelection: SelectListItem[]) {
		this._allPermissions = allPermissions;
		this._currentSelection = currentSelection;
	}
	
	public addAllPermissions() {
		this._currentSelection = [].concat(this._allPermissions);
		this.setSelectedPermissions();
	}

	public addPermissions() {
		var options = $("#availablepermissions option:selected");
		for (var i = 0; i < options.length; i++) {
			var item: SelectListItem = { Text: options[i].innerHTML, Value: options[i].getAttribute("value") }
			this._currentSelection.push(item);
		}
		this.setSelectedPermissions();
	}

	public removeAllPermissions() {
		this._currentSelection = [];
		this.setSelectedPermissions();
	}

	public removePermissions() {
		var self = this;
		var options = $("#selectedpermissions option:selected");
		for (var i = 0; i < options.length; i++) {
			var items = this._currentSelection.filter(function (item: SelectListItem) {
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
	}

	private updateListboxes() {
		var self = this;

		$("#selectedpermissions option").remove();
		$("#availablepermissions option").remove();
		$.each(this._allPermissions, function (i, e) {
			var items = self._currentSelection.filter(function (item: SelectListItem) {
				return e.Text === item.Text && e.Value === item.Value;
			});
			if (items.length > 0) {
				$("#selectedpermissions").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
			}
			else {
				$("#availablepermissions").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
			}
		});
	}

	private setSelectedPermissions() {
		this.updateListboxes();

		$("#PermissionIds option:selected").prop("selected", false);

		$.each(this._currentSelection, function (i, e) {
			$("#PermissionIds option[value='" + e.Value + "']").prop("selected", true);
		});
	}
}