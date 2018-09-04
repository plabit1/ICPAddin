
// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker;
var peoplePickerBy;
var peoplePickerCC;

//Wait for the page to load
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Load the SP.UI.Controls.js file to render the App Chrome
                    //$.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
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

                        peoplePickerCC = new CAMControl.PeoplePicker(context, $('#spanAdministratorsCC'), $('#inputAdministratorsCC'), $('#divAdministratorsSearchCC'), $('#DBModel_AssignedToCCControl'));
                        peoplePickerCC.InstanceName = "peoplePickerCC";
                        peoplePickerCC.Language = spLanguage;
                        peoplePickerCC.MaxEntriesShown = 5;
                        peoplePickerCC.AllowDuplicates = false;
                        peoplePickerCC.ShowLoginName = true;
                        peoplePickerCC.ShowTitle = true;
                        peoplePickerCC.PrincipalType = 1;
                        peoplePickerCC.MinimalCharactersBeforeSearching = 2;
                        peoplePickerCC.MaxUsers = 10;
                        peoplePickerCC.Initialize();

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

