// Jquery and Ajax calls for Admin's Product Manager page
$(document).ready(function () {
    //Tooltips for buttons
    let tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
    let tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
    //Tooltips for buttons

    // Variables for the loadMore function
    var page = 1;
    var productAmount = 10;

    const filterUrl = $(".productManager").data("filter-url")

    // Function to get all filter data that was entered
    function getFilterData() {
        return {
            serial: $('#serial').val(),
            country: $('#country').val(),
            maker: $('#maker').val(),
            name: $('#name').val(),
            instock: $('#instock').is(':checked'),
            outstock: $('#outstock').is(':checked'),
            category: $('#category').val(),
            sub: $('#sub').val(),
            price: $('#price').val(),
            volume: $('#volume').val(),
            alpercent: $('#alch').val(),
            page: page,
            productAmount: 10,
            // Add other form parameters here
        };
    };

    function applyFilter() {
        // Determine the state of instock and outstock
        var isInStock = $('#instock').is(':checked');
        var outOfStock = $('#outstock').is(':checked');

        // Disable or enable the opposite checkbox based on the current checkbox state
        $('#instock').prop('disabled', outOfStock);
        $('#outstock').prop('disabled', isInStock);

        page = 1;
        // Prepare the filter data
        var filterData = getFilterData();

        // Make the AJAX call
        $.ajax({
            url: filterUrl, // Make sure this URL is correct in your context
            type: 'GET',
            data: { filterData: filterData },
            success: function (result) {
                // Replace the product list with filtered data
                $('.productTable').html(result);
                $(".loadProducts").click(loadMore);
            },
            error: function (xhr, status, error) {
                // Handle error
                alert('Error: ' + error);
            }
        });
    };

    // Bind the applyFilter function to change events of the checkboxes
    $('#instock, #outstock').change(applyFilter);

    // Bind the applyFilter function to the click event of the filter button
    $('#filterAll').click(applyFilter);

    // Load 10 more products in the table
    function loadMore() {
        page++

        filterData = getFilterData();

        $.ajax({
            url: filterUrl,
            type: 'Get',
            data: {
                filterData: filterData,
            },
            success: function (result) {
                $('.productTable').html(result);
                $(".loadProducts").click(loadMore);
            },
            error: function (xhr, status, error) {
                // Handle error
                alert('Error: ' + error);
            }
        })
    };

    // Bind the loadMore function to the click event of the load more button
    $(".loadProducts").click(loadMore);

    // Clear all filters
    $('#clearAll').click(function () {
        const inputs = document.querySelectorAll("input")
        for (const input of inputs) {
            input.value = null;
        }
        page = 1;
        $.ajax({
            url: filterUrl,
            type: 'GET',
            success: function (result) {
                // Replace the product list with filtered data
                $('.productTable').html(result);
                $(".loadProducts").click(loadMore);
            },
            error: function (xhr, status, error) {
                // Handle error
                alert('Error: ' + error);
            }
        });
    });

    // Update the remove item modal with the right serial when product is clicked
    $('.removeitem').click(function () {
        const modalBody = document.getElementById("modalbody")
        const modalRemove = document.getElementById("removebtn")
        const serial = $(this).data('serial')
        const removeUrl = $(this).data('remove-url')

        modalBody.innerHTML = "<p>Are you sure you want to remove " + serial + "?</p>"

        modalRemove.href = removeUrl + "?serial=" + serial;
    });

    $('.orderStock').click(function () {
        const serial = $(this).data('serial')

        $("#stockModalBody").html("<p>How much stock do you want to order for " + serial + "?</p>" + "<input type='number' class='stockAmount'/>")
        $("#stockModalBody").attr("data-serial", serial)
    });

    $("#orderBtn").click(function () {
        var amount = $(".stockAmount").val();
        var serial = $("#stockModalBody").data("serial");
        const orderUrl = $(".orderStock").data('order-url')

        $.ajax({
            url: orderUrl,
            type: "POST",
            data: { serial: serial, stock: amount },
            success: function (result) {
                if (result.success) {
                    alert("Succefully ordered " + amount + " more stock for " + serial + "!");
                    location.reload();
                }
                else {
                    alert("Error ordering more stock for " + serial + ", please try again.");
                }
            }
        })

    })
});