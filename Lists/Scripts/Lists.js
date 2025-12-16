$(function () {

    $(".messagesTitle").click(function () {
        if ($(".informationSiteMessageGroup").is(":visible") || 
            $(".announcementSiteMessageGroup").is(":visible") || 
            $(".warningSiteMessageGroup").is(":visible")) {

            $(".informationSiteMessageGroup").hide();
            $(".warningSiteMessageGroup").hide();
            $(".announcementSiteMessageGroup").hide();
            $("#siteMessageText").html("Show Messages");
        } else {
            $(".informationSiteMessageGroup").show();
            $(".warningSiteMessageGroup").show();
            $(".announcementSiteMessageGroup").show();
            $("#siteMessageText").html("Hide Messages");
        }
    });

});

function extendColumn(context) {
    if ($(".proConWrapper")) {
        var $column = $(context).closest(".prosAndConsListItems").addClass("prosAndConsListItemsFocused");
        $column.attr("data-old-class", $column.hasClass("left") ? "left" : "right").removeClass($column.hasClass("left") ? "left" : "right");

        $(".proConWrapper").find(".prosAndConsListItems").each(function () {
            if (!$(this).hasClass("prosAndConsListItemsFocused")) {
                $(this).addClass("hidden");
            }
        });
    }
}

function revertToViewColumns() {
    if ($(".prosAndConsListItems")) {
        $(".prosAndConsListItemsFocused").addClass($(".prosAndConsListItemsFocused").attr("data-old-class"))
                                         .removeClass("prosAndConsListItemsFocused")
                                         .removeAttr("data-old-class");
        $(".prosAndConsListItems").removeClass("hidden");
    }
}

function clearForm(context, callback) {
    var $form = $(context).is("form") ? $(context) : $(context).closest("form");
    $form.find("input[type=text],textarea").val("").removeClass("input-validation-error");
    $form.find(".validation-summary-errors ul").empty();
    $form.find(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid").empty();
    $form.find(".validation-summary-errors").removeClass("validation-summary-errors").addClass("validation-summary-valid");
    $form.find("input[type=file]").each(function () {
        $(this).replaceWith($(this).clone(true));
    });
    $form.find("#selectedImage").empty();
    $form.find("input").trigger("change");

    $form.find("input[type=checkbox]").each(function () {
        if ($(this).is(":checked")) {
            $(this).trigger("click");
        }
    });

    if ($form.find("#quantityValue")) {
        $form.find("#quantityValue").html("1");
        $form.find("Quantity").val("1");
    }

    $form.find("select").val("");

    if (authGrid.domContainer) {
        authGrid.reset();
    }

    if (callback) {
        callback();
    }
}

function hideLoader() {
    $("#loadingContainer").hide();
}

function showLoader() {
    $("#loadingContainer").show();
}

function PostSuccess(passedBack) {
    var isSuccess = passedBack.getResponseHeader("PostSuccess")
    return (isSuccess != null && isSuccess != undefined && isSuccess.toLowerCase() == "true");
}