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

(function () {
    var bar = $('.progress-bar');
    var percent = $('.progress-bar');

    $('form').ajaxForm({
        beforeSend: function () {
            var percentValue = '0%';
            bar.width(percentValue);
            percent.html(percentValue);
        },
        uploadProgress: function (event, position, total, percentComplete) {
            var percentValue = percentComplete + '%';
            bar.width(percentValue);
            percent.html(percentValue);
        },
        success: function (d) {
            var percentValue = '100%';
            bar.width(percentValue);
            percent.html(percentValue);
            location.reload();
        }
    });
})();