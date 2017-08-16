$(document).on('ready', function (e) {

    /**
     * If the repeat checkbox is checked when the page is loaded (after validation fail), show the repeat options section
     * 
     * File: ./Views/Jobs/Create.cshtml
     */
    if ($("#Repeat").is(":checked")) {
        $("#repeat-section").show();
    }

    $(".search-expand button").on("click", function () {
        var $searchBoxInput = $(this).closest(".search-expand").find("#searchBoxInput");
        if ($searchBoxInput.val() == '') {
            $searchBoxInput.focus();
            return false;
        }
    });
    $(".search-expand #searchBoxInput").on("focus", function () {
        $(this).closest(".search-expand").toggleClass("open", true);
    });
    $(".search-expand #searchBoxInput").on("blur", function () {
        $(this).closest(".search-expand").toggleClass("open", false);
    });

    $(".page-buttons .open-menu").on("click", function () {
        $(this).closest(".page-buttons").toggleClass("open");
    });

    $(".page-buttons .menu-background").on("click", function () {
        $(this).closest(".page-buttons").toggleClass("open", false);
    });

    $(".settings").on("click", function () {
        $(this).toggleClass("open");
    });

    $(".settings-open .settings-background").on("click", function () {
        $(this).closest(".search-bar").children(".settings").toggleClass("open", false);
    });



    $(".nav-bar > .button").on("click", function () {
        $(this).closest(".nav-bar").toggleClass("open", true);
    });

    $(".nav-bar > .separator").on("click", function () {
        $(this).closest(".nav-bar").toggleClass("open", false);
    });

    $(".fire-once").closest("form").on("submit", function () {
        if ($(this).valid()) {
            $(this).find(".fire-once").attr("disabled", "disabled");
        }
    });
});



/**
 * Show / Hide the repeat Job options when the #Repeat checkbox is triggered
 *
 * File: ./Views/Jobs/Create.cshtml
 */
$("#Repeat").on("change", function(e) {
    if ($(this).is(":checked"))
        $("#repeat-section").show();
    else
        $("#repeat-section").hide();
});


/**
 * Activity start and end datepicker validation.
 *
 * File: ./Views/Activities/Edit.cshtml
 */
$('#StartDateTime, #EndDateTime, #StartDateHour, #EndDateHour, #StartDateMinute, #EndDateMinute').on('keyup', function (e) {

    var startDateTime = $('#StartDateTime').val(),
        endDateTime = $('#EndDateTime').val(),
        startDateHour = $('#StartDateHour').val(),
        endDateHour = $('#EndDateHour').val(),
        startDateMinute = $('#StartDateMinute').val(),
        endDateMinute = $('#EndDateMinute').val(),

        // Create timestamps from the start and end dates to compare the two
        startTimestamp = Math.round(new Date(startDateTime + " " + startDateHour + ":" + startDateMinute).getTime() / 1000),
        endTimestamp = Math.round(new Date(endDateTime + " " + endDateHour + ":" + endDateMinute).getTime() / 1000);

    if (startTimestamp > endTimestamp) {
        // Add validation error classes and error message
        $('#EndDateTime').closest('.form-group').addClass('has-error').find('.help-block').removeClass('field-validation-valid').addClass('field-validation-error').html(Resources.ActivityEndDateBeforeStartDate);
    } else {
        // Remove validation error classes and error message
        $('#EndDateTime').closest('.form-group').removeClass('has-error').find('.help-block').removeClass('field-validation-error').addClass('field-validation-valid').html('');
    }
});

/**
 * When the user clicks the edit message link, show the edit message form and the remove attachment option
 *
 * File: ./Views/Circles/_Message.cshtml
 */
$(".label-change-msg").on('click', function (e) {
    var message = $(this).closest('.message');
    message.find(".edit_message").show();
    message.find(".circle-message-attachment-remove").show();
});

/**
 * When the user clicks the remove attachment link, submit the form to remove the attachment
 *
 * File: ./Views/Circles/_Message.cshtml
 */
$(".circle-message-attachment-remove").on("click", function(e) {
    $("#RemoveAttachmentForm").submit();
});

/**
 * When an admin hovers ove a circle photo, show the photo options menu
 *
 * File: ./Views/Circles/Photos.cshtml
 */
//$(".circle-photo").on("mouseenter", function (e) {
//    $(this).find(".circle-photo-dropdown").show();
//});
//$(".circle-photo").on("mouseleave", function (e) {
//    $(this).find(".circle-photo-dropdown").hide();
//});

/**
 * When an item has data-val-target AND data-val attibute, another field should be updated
 *
 * File: ./Views/Marketplace/Overview.cshtml >> but can be re-used for other purpose
 */
$('a[data-val-target][data-val]').on("click", function (e) {
    var target = $('#' + $(this).data('val-target'));
    var val = $(this).data('val');
    if (target && (val == '' || val)) {
        $(target).val(val);
    }

    // Check if the form should be triggered
    var formTrigger = $(this).data('form-trigger');
    if (formTrigger)
    {
        $('#' + formTrigger).submit();
    }
});
(function ($) {
    $.fn.scrollTo = function (target, options, callback) {
        if (typeof options == 'function' && arguments.length == 2) { callback = options; options = target; }
        var settings = $.extend({
            scrollTarget: target,
            offsetTop: 50,
            duration: 500,
            easing: 'swing'
        }, options);
        return this.each(function () {
            var scrollPane = $(this);
            var scrollTarget = (typeof settings.scrollTarget == "number") ? settings.scrollTarget : $(settings.scrollTarget);
            var scrollY = (typeof scrollTarget == "number") ? scrollTarget : scrollTarget.offset().top + scrollPane.scrollTop() - parseInt(settings.offsetTop);
            scrollPane.animate({ scrollTop: scrollY }, parseInt(settings.duration), settings.easing, function () {
                if (typeof callback == 'function') { callback.call(this); }
            });
        });
    }

    $.validator.unobtrusive.adapters.addBool("mandatory", "required");
}(jQuery));

function marketPlaceLoadMore(btn, formSelector) {
    var form = $(formSelector);
    var container = $('.marketplace-item-placeholder');
    var pageSize = $('#marketplace-items').attr('pagesize');
    var count = container.find('.marketplace-item').length;

    $.ajax({
        url: form.attr('action') + '?Skip=' + count,
        type: form.attr('method'),
        data: form.serialize(),
        success: function(response) {
            var items = $(response);

            if (items.length < pageSize) {
                $(btn).hide();
            }

            container.append(items);
            itemTiler.layout();
        }
    });
}
/**
 * When there is a space in the e-mail validation field, trim it
 *
 * File: ./Views/Register/Index.cshtml >> but can be re-used for other purpose
 */
//$.each($('[data-val-email]'), function(key, data) {
//    window.setInterval(function() {
//        data.value = data.value.trim();
//    }, 2200);
//});
$('[data-val-email]').on("blur", function() {
    $(this).val($(this).val().trim());
});

/**
 * Startpage items can open pages
 *
 * File: ./Views/Home/IndexAuthenticated.cshtml
 */
$(".startpage-item").on("click", function(e) {
    window.location = $(this).data("link");
});

/**
 * Sticky menubox
 *
 * File: ./Views/Shared/_Layout.cshtml
 */
function sticky_relocate() {
    if ($('#sticky-anchor').length && screen.width > 640) {
        var window_top = $(window).scrollTop();
        var div_top = $('#sticky-anchor').offset().top;
        if (window_top > div_top) {
            $('#sticky').addClass('stick');
        } else {
            $('#sticky').removeClass('stick');
        }
    }
}

$(function () {
    $(window).scroll(sticky_relocate);
    sticky_relocate();
});

function markNotifications() {
    $.ajax({
        url: "/Notifications/SetAllRead"
    });

    $(".notification-menu").find(".badge").hide();

    $("#latestNotifications").find('li').each(function () {
        $(this).find("label").hide();
    });
}

$(".notification-menu").on("click", function (e) {
    setTimeout(function () { markNotifications(); }, 1500);
});

//Scroll to top after opening menu
$(".navbar-toggle").click(function () {
    if (!$("#side-menu").hasClass("in")) {
        $('html, body').animate({
            scrollTop: $("#view-wrapper").offset().top
        }, 300);
    }
});

var homepage = (function () {
    function closeBanner()
    {
        $(".homepage-banner").remove();
        return false;
    }
    return {
        CloseBanner: closeBanner
    }
}())

var accordion = (function () {
    function open(item) {
        $(item).parent().parent().children(".accordion").toggleClass("closed");
        $(item).parent().remove();
    }

    return {
        Open: open
    }
}());

var multiUploader = (function () {
    String.prototype.endsWith = function(suffix) {
        return this.indexOf(suffix, this.length - suffix.length) !== -1;
    };

    var items = 1;
    function onUpload(item) {
        var $item = $(item);
        var copy = $item.clone();
        $item.addClass("hidden");
        var fileList = $item.closest(".profile-multi-upload").children().children(".uploaded-files");
        $item.attr("id", "file" + items);
        $item.attr("name", "file" + items);
        items++;

        var filename = $item.val().substr(12).toLowerCase();

        var $listitem = $(document.createElement("div"));
        $listitem.addClass("newFile").addClass("uploaded-file");
        fileList.append($listitem);

        var $image = $(document.createElement("div"));
        $image.addClass("image");
        $listitem.append($image);
        if (filename.endsWith(".pdf")) {
            $image.append("<i class=\"fa fa-file-pdf-o\"></i>");
        } else if (filename.endsWith(".xls") || filename.endsWith(".xlsx") || filename.endsWith(".ods")) {
            $image.append("<i class=\"fa fa-file-excel-o\"></i>");
        } else if (filename.endsWith(".doc") || filename.endsWith(".docx") || filename.endsWith(".odt")) {
            $image.append("<i class=\"fa fa-file-word-o\"></i>");
        } else {
            if (window.File && window.FileReader && window.FileList && window.Blob) {
                var fileobj = $item[0].files[0];
                var fr = new FileReader();
                fr.$image = $image;
                fr.onload = function(file, $image) {
                    this.$image[0].innerHTML = '<img src="' + file.target.result + '">';
                    this.$image.prepend("<span></span>");
                };
                fr.readAsDataURL(fileobj);
            } else {
                $image.append("<i class=\"fa fa-file\"></i>");
            }
        }

        var $name = $(document.createElement("div"));
        $name.addClass("name");
        $name.text(filename);
        $listitem.append($name);
        $name.prepend("<span class=\"close\"><i class=\"fa fa-times\"></i></span>");
        $name.children().children().on("click", function () {
            removeFile(this);
        });

        $item.parent().append(copy);
        $listitem.append($item);
        copy.val("");
    }

    function removeFile(item)
    {
        $(item).closest(".uploaded-file").remove();
    }
    return {
        OnUpload: onUpload,
        RemoveFile: removeFile
    }
}());


$.datepicker.regional["nl"] = { // Default regional settings
    closeText: "Done", // Display text for close link
    prevText: "Vorige", // Display text for previous month link
    nextText: "Volgende", // Display text for next month link
    currentText: "Vandaag", // Display text for current month link
    monthNames: ["Januari", "Februari", "Maart", "April", "Mei", "Juni",
        "Juli", "Augustus", "September", "Oktober", "November", "December"], // Names of months for drop-down and formatting
    monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"], // For formatting
    dayNames: ["Zondag", "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag"], // For formatting
    dayNamesShort: ["Zon", "Maa", "Din", "Woe", "Don", "Vrij", "Zat"], // For formatting
    dayNamesMin: ["Zo", "Ma", "Di", "Wo", "Do", "Vr", "Za"], // Column headings for days starting at Sunday
    weekHeader: "Wk", // Column header for week of the year
    dateFormat: "dd-mm-yy", // See format options on parseDate
    firstDay: 0, // The first day of the week, Sun = 0, Mon = 1, ...
    isRTL: false, // True if right-to-left language, false if left-to-right
    showMonthAfterYear: false, // True if the year select precedes month, false for month then year
    yearSuffix: "" // Additional text to append to the year in the month headers
};

$.datepicker.setDefaults($.datepicker.regional['nl']);