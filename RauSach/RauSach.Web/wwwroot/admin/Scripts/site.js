// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

jQuery(document).ready(function () {

    "use strict";
    function validateMenuLink($this) {
        var itemUrl = $this.attr('href') + '/' + $this.attr('data-href');
        var isActive = itemUrl.toLowerCase().indexOf(iclassic.currentLink.toLowerCase()) !== -1;
        if (isActive) {
            if ($this.parent().parent().hasClass("br-menu-sub"))
                $this.parent().parent().css("display", "block")
        }
        return isActive;
    }

    function activeMenu() {
        var found = false;
        jQuery("#LeftMenu a").each(function () {
            var $this = jQuery(this);
            var isActive = validateMenuLink($this);
            if (!isActive) return;

            found = true;
            $this.addClass("active");
        });

        if (found) return;

        jQuery("#LeftMenu a").each(function () {
            var $this = jQuery(this);
            var isActive = validateMenuLink($this);
            if (!isActive) return;

            $this.parent().addClass("active");
            if (!$("body").hasClass("collapsed-menu")) {
                $this.parent().parent().show();
            }
            $this.parents(".nav-parent").addClass("active nav-active");
        });
    }
    activeMenu();

});

function removeRow($this) {
    $($this).parents('tr').remove();
}

function addLoader(selector, transparent) {
    var filename = transparent ? "ajax-loader2.gif" : "ajax-loader.gif";
    $(selector).append('<img src="/images/' + filename + '" class="loader" alt="Loading"/>');
}

function removeLoaderFromSelector(parentSelector) {
    if (parentSelector === null || parentSelector === undefined) return;
    if ($(parentSelector).find(".loader").length > 0) {
        $(parentSelector).find(".loader").each(function (index, element) {
            if (element) {
                element.remove();
            }
        });
    }
}
