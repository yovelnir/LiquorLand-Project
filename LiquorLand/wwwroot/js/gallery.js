$(document).ready(function () {
    var page = 1;
    var searchParams = new URLSearchParams(window.location.search);
    var cat = searchParams.get('category');
    var sub = searchParams.get('sub');
    var term = searchParams.get('word');
    var sortType = null;
    var filterData = null;

    if (cat != null) {
        $(document).ready(function () { $('#category').prop('value', cat); });
        $(document).ready(function () { $('#category').prop('disabled', true); });
    }
    if (sub != null) {
        $(document).ready(function () { $('#sub').prop('value', sub) });
        $(document).ready(function () { $('#sub').prop('disabled', true) });
    }

    $(document).ready(function () {
        const filterUrl = $(".searchFilters").data("filter-url");
        const sortUrl = $(".sortBox").data("sort-url")

        function getFilterData() {
            return {
                serial: $('#serial').val(),
                country: $('#country').val(),
                maker: $('#maker').val(),
                name: $('#name').val(),
                category: $('#category').val(),
                sub: $('#sub').val(),
                price: $('#price').val(),
                volume: $('#volume').val(),
                alpercent: $('#alch').val(),
                searchTerm: term,
                sort: sortType,
                productAmount: 10,
                page: page
            };
        };

        function loadMore() {
            page++

            filterData = getFilterData();
            const loadUrl = $(".loadProducts").data('load-url');

            $.ajax({
                url: loadUrl,
                type: 'Get',
                data: {
                    sort: sortType,
                    filterData: filterData,
                },
                success: function (result) {
                    $('#productGallery').html(result);
                    $(".loadProducts").click(loadMore);
                },
                error: function (xhr, status, error) {
                    // Handle error
                    alert('Error: ' + error);
                }
            })
        };

        function sort() {
            sortType = $(this).data("sort")
            page = 1;
            filterData = getFilterData();

            $.ajax({
                url: sortUrl,
                type: 'Get',
                data: {
                    sort: sortType,
                    filterData: filterData
                },
                success: function (result) {
                    $('#productGallery').html(result);
                    $(".loadProducts").click(loadMore);
                },
                error: function (xhr, status, error) {
                    // Handle error
                    alert('Error: ' + error);
                }
            });
        }

        function filter() {
            page = 1;

            filterData = getFilterData()

            $.ajax({
                url: filterUrl,
                type: 'GET',
                data: filterData,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    // Replace the product list with filtered data
                    $('#productGallery').html(result);
                    $(".loadProducts").click(loadMore);
                },
                error: function (xhr, status, error) {
                    // Handle error
                    alert('Error: ' + error);
                }
            });
        }

        function clear() {
            const inputs = document.querySelectorAll("input")
            for (const input of inputs) {
                input.value = null;
                input.disabled = false;
            }

            sortType = null;
            page = 1;

            $.ajax({
                url: filterUrl,
                type: 'GET',
                data: { searchTerm: term, productAmount: 10, page: page },
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    // Replace the product list with filtered data
                    $('#productGallery').html(result);
                    $(".loadProducts").click(loadMore);
                },
                error: function (xhr, status, error) {
                    // Handle error
                    alert('Error: ' + error);
                }
            });
        }

        $(".loadProducts").click(loadMore)
        $(".SortType").click(sort)
        $("#filterAll").click(filter)
        $("#clearAll").click(clear)

        $(".sortButton").click(function () {
            // Toggle width between 0 and 50vw
            var newWidth = $(".sortBox").width() > 0 ? 0 : '81vw';
            page = 1;

            $(".sortBox").animate({
                width: newWidth
            }, 150);
        });
    });
});