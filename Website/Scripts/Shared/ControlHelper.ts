/// <reference path="../../wwwroot/lib/@types/jquery/index.d.ts" />
/// <reference path="../Models/ActiveState.ts" />

class ControlHelper {
	public static ChangeActiveState(value: string): ActiveState {
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
	}

	public static ReloadGrid(): void {
		var grid: any = $("#jsGrid");
		grid.jsGrid("loadData");
	}
}