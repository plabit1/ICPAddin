function validateForm(e) {
    $("#formValidation").hide();
    var errorMessage = '';
    if (!$("#DBModel_AssignedToControl").val())
        errorMessage = "- Seleccione a un responsable.<br/>";
    if (!$("#DBModel_DueToDate").val())
        errorMessage += "- Seleccione la fecha de vencimiento.<br/>";
    else {
        var tmpDate = new Date($("#DBModel_DueToDate").val());
        if(tmpDate < new Date())
            errorMessage += "- La fecha de vencimiento debe ser mayor a hoy.<br/>";
    }
    if (!$("#DBModel_FirstReminderDays").val())
        errorMessage += "- Ingrese los días para el primer recordatorio.<br/>";
    if (!$("#DBModel_SecondReminderDays").val())
        errorMessage += "- Ingrese los días para el segundo recordatorio.<br/>";
    if ($("#DBModel_FirstReminderDays").val() && $("#DBModel_SecondReminderDays").val()) {
        var tmp1 = parseInt($("#DBModel_FirstReminderDays").val());
        var tmp2 = parseInt($("#DBModel_SecondReminderDays").val());
        if(tmp1 <= 0 || tmp1 > 30)
            errorMessage += "- La cantidad de días para el primer recordatorio debe estar en el rango de 1 a 30 días.<br/>";
        if (tmp2 <= 0 || tmp2 > 30)
            errorMessage += "- La cantidad de días para el segundo recordatorio debe estar en el rango de 1 a 30 días.<br/>";
        if(tmp2 > tmp1)
            errorMessage += "- El segundo recordatorio no puede ser mayor al primero.<br/>";
    }
    if (errorMessage) {
        $("#formValidation").html("Por favor revise lo siguiente para continuar:<br/>" + errorMessage);
        $("#formValidation").show();
    } else {
        $("#btnSubmit").click();
    }
}

