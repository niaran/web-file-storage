$("*[name=deleteLink]").click(function (event) {
    $.confirm({
        'title': 'Подтверждение удаления',
        'message': 'Вы хотите удалить данный файл. <br />Он не сможет быть восстановлен позже! Продолжаем?',
        'buttons': {
            'Да': {
                'class': 'blue',
                'action': function () {
                   location = event.target.id;
                }
            },
            'Нет': {
                'class': 'gray',
                'action': function () {

                }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });

});