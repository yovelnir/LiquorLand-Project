
$(document).ready(function () {
    let clicker = null;
    let open = false;

    $('#Beer, #Wine, #Whiskey').click(subNav);

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    async function subNav() {
        var el = $("#SubCat")
        var cat = $(this).attr('id')
        var subCatUrl = $(this).data("sub-url")

        $(el).html("<ul class='CategoryNavList'>");
        $("#SubNav").collapse("toggle");

        if (open)
            await sleep(300)

        if (cat == "Wine") {
            $(".CategoryNavList").html("");
            for (const sub of ["Red Wine", "White Wine", "Rose", "Sparkling", "Dessert"]) {
                const category = sub.split(" ")
                const url = subCatUrl + "&sub=" + category[0];
                $(".CategoryNavList").append("<a href=" + url + " class='nav-item'>" + sub + "</a>")
            }
        }
        else if (cat == "Beer") {
            $(".CategoryNavList").html("");
            for (const sub of ["Lager", "Ale", "IPA", "Wheat", "Cider"]) {
                const url = subCatUrl + "&sub=" + sub;
                $(".CategoryNavList").append("<a href=" + url + " class='nav-item'>" + sub + "</a>")
            }
        }
        else if (cat == "Whiskey") {
            $(".CategoryNavList").html("");
            for (const sub of ["Bourbon", "Single+Malt", "Blended", "American", "Japanese", "Irish", "Single+Grain"]) {
                const url = subCatUrl + "&sub=" + sub;
                $(".CategoryNavList").append("<a href=" + url + " class='nav-item'>" + sub.replace('+', ' ') + "</a>")
            }
        }
        if ($("#SubNav").hasClass("collapse") && clicker !== this) {
            // If navbar is already open and a different category is clicked, toggle the navbar
            $("#SubNav").collapse("toggle");
        }

        open = true;
        clicker = this;

        if ($("#SubNav").hasClass("collapse"))
            open = false;
    }
})
