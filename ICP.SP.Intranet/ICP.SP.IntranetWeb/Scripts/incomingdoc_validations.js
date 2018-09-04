function validateForm(e) {
    $("#formValidation").hide();
    var errorMessage = '';
    if (!$("#DBModel_DocumentSubject").val())
        errorMessage = "- Ingrese el asunto del documento.<br/>";
    /*if (!$("#DBModel_DocumentDate").val())
        errorMessage += "- Seleccione la fecha del documento.<br/>";
    else {
        var tmpDate = new Date($("#DBModel_DocumentDate").val());
        if (tmpDate > new Date())
            errorMessage += "- La fecha del documento no puede ser mayor a hoy.<br/>";
    }*/
    if (!$("#DBModel_AssignedToControl").val())
        errorMessage += "- Seleccione a un responsable.<br/>";
    if (errorMessage) {
        $("#formValidation").html("Por favor revise lo siguiente para continuar:<br/>" + errorMessage);
        $("#formValidation").show();
    } else {
        $("#preSave").hide();
        $("#saving").toggleClass("hidden");
        $("#btnSubmit").click();
    }
}

