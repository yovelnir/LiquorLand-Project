var items = 0;
$(document).ready(async function () {
    await itemCount();
    $('.item-count').html(items);
})

async function itemCount() {
    const countUrl = $("#countUrl").data("count-url")
    await $.ajax({
        type: "GET",
        url: countUrl,
        success: function (response) {
            items = response;
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

$(document).ready(function () {
    $('.addToCartButton').click(async function () {
        var serial = $(this).data('serial');
        var actionUrl = $(this).data('add-url');
        var input = $('.quantityInput').val();
        var quantity;

        if (input > 1) {
            quantity = parseInt(input, 10);
        }
        else {
            quantity = 1;
        }
        await $.ajax({
            type: "GET",
            async: true,
            url: actionUrl,
            data: { serial: serial, quantity: quantity },
            success: async function (response) {
                if (response.success) {
                    $('#alert-success').fadeIn().delay(800).fadeOut();
                    await itemCount();
                    $('.item-count').html(items);
                }
                else {
                    $('#alert-failure').fadeIn().delay(800).fadeOut();
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });
});