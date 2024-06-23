// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    $(document).ready(function () {
        $("#cartLink").on('click', function (event) {
            event.preventDefault();
            console.log("UserLink clicked");


            $.ajax({
                url: "/Carts/CartMenu",
                type: "GET",
                success: function (data) {
                    $("#cartModalContainer").html(data);
                },
                error: function (xhr, status, error) {
                    console.error(error);
                    console.log("not work")
                }
            });
        });

        $("#UserLink").on('click', function (event) {
            event.preventDefault();
            console.log("UserLink clicked");


            $.ajax({
                url: "/Home/UserP",
                type: "GET",
                success: function (data) {
                    $("#ProfileModalContainer").html(data);
                },
                error: function (xhr, status, error) {
                    console.error(error);
                    console.log("not work")

                }
            });
        });

       
    });

