function sizeCheck() {
    if ($(this).val() !== "") {
        for (var i = 0; i < $(this)[0].files.length; i++) {
            var file = $(this)[0].files[i];
            if (file.size > 104857600) {
                $("[id=uploadErr]").show();
                setTimeout(function () { $("[id=uploadErr]").hide(); }, 5000);
                $(this).replaceWith($(this).clone());
            }
        }
        
    }
};

$(document).on('change', '#uploadedFile', sizeCheck);
$(document).on('change', '#uploadedDir', sizeCheck);

(function () {
    var bar = $('.progress-bar');
    var percent = $('.progress-bar');

    $('*[name=uploadForm]').ajaxForm({
        beforeSend: function () {
            if ($('#uploadedFile').val() !== "" || $('#uploadedDir').val() !== "") {
                var percentValue = '0%';
                bar.width(percentValue);
                percent.html(percentValue);

            }
            
        },
        uploadProgress: function (event, position, total, percentComplete) {
            if ($('#uploadedFile').val() !== "" || $('#uploadedDir').val() !== "") {
                var percentValue = percentComplete + '%';
                bar.width(percentValue);
                percent.html(percentValue);
            }
        },
        success: function (d) {
            if ($('#uploadedFile').val() !== "" || $('#uploadedDir').val() !== "") {
                var percentValue = '100%';
                bar.width(percentValue);
                percent.html(percentValue);
                location.reload();
            }
        }
    });
})();