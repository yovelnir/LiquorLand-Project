$(document).ready(function () {
    const removeUrl = $(".remove_item").data("remove-url");
    const plusUrl = $(".plus_button").data("plus-url");
    const cartUrl = $("#cartUrl").data("cart-url");
    const quantityUrl = $(".quantity-value").data("quantity-url");

    $('.remove_item').click(function () {
        var serial = $(this).data('serial');
        

        $.ajax({
            type: "GET",
            url: removeUrl,
            data: { serial: serial },
            success: async function (response) {
                await itemCount();
                $('.item-count').html(items);
                $(".cart-container").html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });

    function updateQuantity(serial, action) {
        $.ajax({
            type: "POST",
            url: plusUrl,
            data: { serial: serial, action: action },
            success: async function (response) {
                if (response.success === false) {
                    $("#alert-failure").fadeIn().delay(100).fadeOut();
                    await itemCount();
                    $('.item-count').html(items);
                    $(".cart-container").load(cartUrl, { partial: true });

                }
                else {
                    await itemCount();
                    $('.item-count').html(items);
                    $(".cart-container").html(response);
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    };

    $('.plus_button').click(function () {
        var serial = $(this).data('serial');
        updateQuantity(serial, true);
    });

    $('.minus_button').click(function () {
        var serial = $(this).data('serial');
        updateQuantity(serial, false);
    });

    $(".quantity-value").keypress(function (e) {
        var char = String.fromCharCode(e.which);
        var currentValue = $(this).val();
        var reg = new RegExp("^[0-9]");

        var selection = document.getSelection().toString();
        if (!reg.test(String.fromCharCode(e.which))) {
            e.preventDefault();
        }
    });

    $(".quantity-value").change(function () {
        var serial = $(this).data('serial')
        var quantity = $(this).val()
        $.ajax({
            type: "POST",
            url: quantityUrl,
            data: { serial: serial, quantity: quantity },
            success: async function (response) {
                if (response.success === false) {
                    $("#alert-failure").fadeIn().delay(100).fadeOut();
                    await itemCount();
                    $('.item-count').html(items);
                    $(".cart-container").load(cartUrl, { partial: true });

                }
                else {
                    await itemCount();
                    $('.item-count').html(items);
                    $(".cart-container").html(response);
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });
});