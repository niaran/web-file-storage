
var singleFileUploadValue = "none";
var folderUploadValue = "none";
var folderCreateValue = "none";

function forSingleFileForm() {
    if (folderUploadValue == "block") {
        forFolderForm();
    }
    if (folderCreateValue == "block") {
        forFolderCreate();
    }
    if (singleFileUploadValue == "none") {
        $("#singleFileUpload").css("display", "block");
        singleFileUploadValue = "block";
    }
    else {
        $("#uploadedFile").replaceWith($("#uploadedFile").clone());
        $("#singleFileUpload").css("display", "none");
        singleFileUploadValue = "none";
    }
}
function forFolderForm() {
    if (singleFileUploadValue == "block") {
        forSingleFileForm();
    }
    if (folderCreateValue == "block") {
        forFolderCreate();
    }
    if (folderUploadValue == "none") {
        $("#folderUpload").css("display", "block");
        folderUploadValue = "block";
    }
    else {
        $("#uploadedDir").replaceWith($("#uploadedDir").clone());
        $("#folderUpload").css("display", "none");
        folderUploadValue = "none";
    }
}
function forFolderCreate() {
    if (singleFileUploadValue == "block") {
        forSingleFileForm();
    }
    if (folderUploadValue == "block") {
        forFolderForm();
    }
    if (folderCreateValue == "none") {
        $("#folderCreate").css("display", "block");
        folderCreateValue = "block";
    }
    else {
        $("#folderCreate").css("display", "none");
        folderCreateValue = "none";
    }
}

//код для переименования файлов и папок
var prevElem = null;
$(".fileName").bind("dblclick", function () {
    if (prevElem !== null) {
        $(prevElem).find(".hiddenTextBox").css("display", "none");
        $(prevElem).find(".nameText").css("display", "block");
        if (prevElem === this) {
            prevElem = null;
            return;
        }
    }
    prevElem = this;
    var htb = $(this).find(".hiddenTextBox");
    var nt = $(this).find(".nameText");

    htb.val($(this).find(".spcName").html());

    htb.css("display", "block");
    htb.focus().select();
    nt.css("display", "none");
});