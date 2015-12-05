$("*[name=deleteLink]").click(function (event) {
    $.confirm({
        'title': 'Delete Confirmation',
        'message': 'You are about to delete this file. <br />It cannot be restored at a later time! Continue?',
        'buttons': {
            'Yes': {
                'class': 'blue',
                'action': function () {
                   location = event.target.id;
                }
            },
            'No': {
                'class': 'gray',
                'action': function () {

                }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });

});