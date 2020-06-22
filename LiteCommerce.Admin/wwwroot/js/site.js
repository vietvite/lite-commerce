// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Click in image to show up upload image window
    $("#avatar").click(function () {
        $("input[id='PhotoPath_file']").click();
    });

    // Confirm delete item in list of each View index
    $("form#delete-selected").submit(function (e) {
        let ok = confirm("Không thể hoàn tác sau khi xóa. Bạn chắc chắn muốn xóa?")
        if (!ok) e.preventDefault()
    });

    // Select all item in list of each View index
    $("#checkAll").click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked);
    });

    // Delete product attribute
    $("a#delete-attribute").click(function (e) {
        let ok = confirm("Không thể hoàn tác sau khi xóa. Bạn chắc chắn muốn xóa?")
        if (!ok) e.preventDefault()
    })

    // Append has-error class to parent div if form has error message
    if ($(".field-validation-error").length) {
        $(".field-validation-error.help-block").parent().addClass("has-error")
    }

});