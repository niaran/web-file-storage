$(document).ready(function () {
    try {
        $("div.file").bind("contextmenu", function (e) {
            e.preventDefault();
            $("#custom-menu", this).css({ top: e.pageY + "px", left: e.pageX + "px" }).show(100);
        });
        $(document).mouseup(function (e) {
            $("[id=custom-menu]").hide();
        });
    }
    catch (err) {
        alert(err);
    }
});