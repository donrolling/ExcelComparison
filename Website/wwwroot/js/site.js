$(document).ready(function () {
	getVersion();
	setupSignout();
});

function setupSignout() {
	$('#signOut').on('click', function (e) {
		e.preventDefault();
		$.ajax({
			url: "/Settings/SignOut", success: function (result) {
				alert('Signed Out');
			}
		});
	});
}

function setVersion(result) {
	if (!result) {
		return;
	}

	if (result.BuildNumber && result.BuildNumber !== '{{BuildNumber}}') {
		$('#versionNumber').text(result.BuildNumber);
		$('#version').removeClass('hidden');
	}
	if (result.Login) {
		$('#userLogin').text(result.Login);
		$('#userLogin').removeClass('hidden');
	}
}

function getVersion() {
	$.ajax({
		url: "/Settings/GetAppSettings", success: function (result) {
			setVersion(result);
		}
	});
}
