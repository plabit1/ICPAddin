function validateForm(e) {
    $("#formValidation").hide();
    var errorMessage = '';
    if (!$("#DBModel_DocumentTitle").val())
        errorMessage = "- Ingrese el asunto del documento.<br/>";
    if (errorMessage) {
        $("#formValidation").html("Por favor revise lo siguiente para continuar:<br/>" + errorMessage);
        $("#formValidation").show();
    } else {
        $("#btnSubmit").click();
    }
}

