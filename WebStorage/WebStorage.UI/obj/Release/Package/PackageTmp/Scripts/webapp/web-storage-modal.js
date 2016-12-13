$("#docName").keyup(function () {
    var empty = false;
    $("#docName").each(function () {
        if ($(this).val().length == 0 || !$(this).val().trim())
            empty = true;
    });
    if (empty) {
        $("#crtBtn").attr("disabled", true);
    } else {
        $("#crtBtn").attr("disabled", false);
    }
});


$("#docBtn").click(function () {
    $("#docMod").modal({ backdrop: false });
    if (!$("#docName").val().trim())
        $("#crtBtn").attr("disabled", true);
});

$("#crtBtn").click(function () {
    $("#docMod").modal('hide');
    $('#docMod').on('hidden.bs.modal', function () {
        window.location.reload();
    });
});