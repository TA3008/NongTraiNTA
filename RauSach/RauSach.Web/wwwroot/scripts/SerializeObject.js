//
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        //if checkbox
        var vlu = this.value;
        if ($('[name="' + this.name + '"]').is(':checkbox')) {
            if ($('[name="' + this.name + '"]').is(':checked')) {
                vlu = true;
            } else {
                vlu = false;
            }
        }

        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(vlu || '');
        } else {
            o[this.name] = vlu || '';
        }
    });
    return o;
};