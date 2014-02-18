define(['jquery', 'config'], function ($, config) {
	var enabled = console && console.log && config.consoleEnabled;

	$.log = function () {
		if (enabled && arguments.length > 0) {
			console.log.apply(console, arguments);
		}
	};
});