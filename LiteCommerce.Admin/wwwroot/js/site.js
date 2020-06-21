// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#avatar").click(function () {
        $("input[id='PhotoPath_file']").click();
    });

    $("form#delete-selected").submit(function (e) {
        let ok = confirm("Không thể hoàn tác sau khi xóa. Bạn chắc chắn muốn xóa?")
        if (!ok) e.preventDefault()
    });

    $("#checkAll").click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked);
    });

    $("a#delete-attribute").click(function (e) {
        let ok = confirm("Không thể hoàn tác sau khi xóa. Bạn chắc chắn muốn xóa?")
        if (!ok) e.preventDefault()
    })
});