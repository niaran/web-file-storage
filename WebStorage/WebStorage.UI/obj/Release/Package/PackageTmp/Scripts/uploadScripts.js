function sizeCheck() {
    if ($(this).val() !== "") {
        if ($(this)[0].files.length > 50)
        {
            showError(this, "Загрузка отменена. Количество одновременно загружаемых файлов не должно превышать 50");
            return;
        }
        var sum = 0;
        for (var i = 0; i < $(this)[0].files.length; i++) {
            var file = $(this)[0].files[i];
            sum += file.size;
            if (file.size > 104857600) {
                showError(this, "Загрузка отменена. Один из загружаемых файлов по размеру превышает 100Mb");
                return;
            }
            if (sum > 104857600 * 3)
            {
                showError(this, "Загрузка отменена. Суммарный размер загружаемых файлов превышает 300Mb");
                return;
            }
        }
        
    }
};

function showError(sender, mess)
{
    $("[id=uploadErr]").html(mess);
    $("[id=uploadErr]").show();
    setTimeout(function () { $("[id=uploadErr]").hide(); }, 5000);
    $(sender).replaceWith($(sender).clone());
}

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