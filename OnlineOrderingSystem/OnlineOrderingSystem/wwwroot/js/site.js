// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#cartLink").click(function (event) {
        event.preventDefault();
        $.ajax({
            url: "/Carts/Index",
            type: "GET",
            success: function (data) {
                $("#cartModalContainer").html(data);
                $("#cartModal").modal("show");
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });

    

});
