/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/SelectListItem.ts" />
/// <reference path="../Shared/ControlHelper.ts" />

class UserCreateUpdate {
	private _allRoles: SelectListItem[];
	private _currentSelection: SelectListItem[];

	constructor(allRoles: SelectListItem[], currentSelection: SelectListItem[]) {
		this._allRoles = allRoles;
		this._currentSelection = currentSelection;
	}
	
	public addAllRoles() {
		this._currentSelection = [].concat(this._allRoles);
		this.setSelectedRoles();
	}

	public addRoles() {
		var options = $("#availableroles option:selected");
		for (var i = 0; i < options.length; i++) {
			var item: SelectListItem = { Text: options[i].innerHTML, Value: options[i].getAttribute("value") }
			this._currentSelection.push(item);
		}
		this.setSelectedRoles();
	}

	public removeAllRoles() {
		this._currentSelection = [];
		this.setSelectedRoles();
	}

	public removeRoles() {
		var self = this;
		var options = $("#selectedroles option:selected");
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
		this.setSelectedRoles();
	}

	private updateListboxes() {
		var self = this;

		$("#selectedroles option").remove();
		$("#availableroles option").remove();
		$.each(this._allRoles, function (i, e) {
			var items = self._currentSelection.filter(function (item: SelectListItem) {
				return e.Text === item.Text && e.Value === item.Value;
			});
			if (items.length > 0) {
				$("#selectedroles").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
			}
			else {
				$("#availableroles").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
			}
		});
	}

	private setSelectedRoles() {
		this.updateListboxes();

		$("#RoleIds option:selected").prop("selected", false);

		$.each(this._currentSelection, function (i, e) {
			$("#RoleIds option[value='" + e.Value + "']").prop("selected", true);
		});
	}
}