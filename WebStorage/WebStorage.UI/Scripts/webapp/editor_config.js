$(document).ready(function () {

    CKEDITOR.config.extraPlugins = 'save';
    CKEDITOR.on('instanceReady',
            function (evt) {
                var instanceName = '_editor'; // the HTML id configured for editor
                var editor = CKEDITOR.instances[instanceName];
                editor.execCommand('maximize');
                CKEDITOR.config.removePlugins = 'maximize';
            });

    CKEDITOR.instances['_editor'].on('change', function (evt) {
        // getData() returns CKEditor's HTML content.
        var byteLimit = 1 * 1024;
        if (evt.editor.getData().length >= byteLimit) {
            alert("Размер документа больше чем может позволить хранить приложение. При сохранении часть данных будет утеряна." +
                "Пожалуйста, создайте новый документ.");
        }
    });
});