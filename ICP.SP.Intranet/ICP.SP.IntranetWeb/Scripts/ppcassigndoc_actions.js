
var context;
var peoplePicker;
var peoplePickerBy;

$(document).ready(function () {
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    var layoutsRoot = spHostUrl + '/_layouts/15/';
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                        peoplePicker = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#DBModel_AssignedToControl'));
                        peoplePicker.InstanceName = "peoplePicker";
                        peoplePicker.Language = spLanguage;
                        peoplePicker.MaxEntriesShown = 5;
                        peoplePicker.AllowDuplicates = false;
                        peoplePicker.ShowLoginName = true;
                        peoplePicker.ShowTitle = true;
                        peoplePicker.PrincipalType = 1;
                        peoplePicker.MinimalCharactersBeforeSearching = 2;
                        peoplePicker.MaxUsers = 1;
                        peoplePicker.Initialize();

                        // force a peoplePicker warmup
                        peoplePickerBy = new CAMControl.PeoplePicker(context, $('#spanAssignedBy'), $('#inAssignedBy'), $('#divAssignedBySearch'), $('#hdnAssignedBy'));
                        peoplePickerBy.InstanceName = "peoplePickerBy";
                        peoplePickerBy.Language = spLanguage;
                        peoplePickerBy.MaxEntriesShown = 5;
                        peoplePickerBy.AllowDuplicates = false;
                        peoplePickerBy.ShowLoginName = true;
                        peoplePickerBy.ShowTitle = true;
                        peoplePickerBy.PrincipalType = 1;
                        peoplePickerBy.MinimalCharactersBeforeSearching = 2;
                        peoplePickerBy.MaxUsers = 1;
                        peoplePickerBy.Initialize();

                        $('#inAssignedBy').val($('#DBModel_AssignedByName').val());
                        var e = jQuery.Event("keydown");
                        e.which = 32; // # Space keycode
                        $("#inAssignedBy").trigger(e);
                    });
                });
        });
});

//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] === urlParameterKey)
            return singleParam[1];
    }
}

