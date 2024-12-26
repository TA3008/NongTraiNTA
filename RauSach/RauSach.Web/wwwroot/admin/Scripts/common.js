$(document).ready(function () {
    // show hide button Edit & Delete when click checkbox
    $(".checkAll").click(function () {
        if ($(this).is(':checked')) {
            $(".checkitem").prop('checked', true);
        } else {
            $(".checkitem").prop('checked', false);
        }
    });
    $(".checkitem").click(function () {
        var numberOfChecked = $('input.checkitem:checkbox:checked').length;
        var numberItem = $('input:checkbox.checkitem').length;

        if ($(this).is(':checked')) {
            if (numberItem === numberOfChecked) {
                $(".checkAll").prop('checked', true);
            }
        } else {
            $(".checkAll").prop('checked', false);
        }
    });

    // Event click with button Edit & Delete
    $(".DeleteItem").click(function () {
        if ($(".checkAll:checked, .checkitem:checked").length === 0) {
            return false;
        }

        if (!confirm("Bạn có chắc muốn xóa?")) {
            return false;
        }

        var listID = "";
        $('input.checkitem:checkbox:checked').each(function (index, item) {
            listID += "<input type='hidden' name='ids[" + index + "]' value='" + $(this).val() + "' />";
        });
        if (listID !== "") {
            $("#form-delete .form-content").html(listID);
            $("#form-delete").submit();
        } else {
            alert('Warring!');
        }
        return false;
    });

    initCurrencyInput();
})


function initCurrencyInput() {
    $('.currency').each(function (i, item) {
        if (item.value) {
            var value = formatMoney(convertToFloat(item.value), '')
            $(item).val(value);
        }

        $(item).maskMoney({ thousands: '.', precision: 0, allowZero: true, suffix: '' });
    });
}

function removeCurrencySeparators() {
    $('.currency').maskMoney('destroy');
    $('.currency').each(function (i, item) {
        var value = $(item).val().replace(/\./g, '');
        $(item).val(value);
    });
}

function formatMoney(n, currency) {
    return (currency ? currency + " " : "") + n.toFixed(0).replace(/./g, function (c, i, a) {
        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? "." + c : c;
    });
}

function convertToFloat(input) {
    if (input) {
        return parseFloat(input);
    }

    return input;
}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}