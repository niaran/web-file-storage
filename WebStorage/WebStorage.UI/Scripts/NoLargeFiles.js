function sizeCheck() {
    if ($("#uploadedFile").val() !== "") {
        var file = $('#uploadedFile')[0].files[0];
        if (file.size > 104857600) {
            $("#uploadErr").show();
            setTimeout(function () { $("#uploadErr").hide(); }, 5000);
            $("#uploadedFile").replaceWith($("#uploadedFile").clone());
        }
    }
}