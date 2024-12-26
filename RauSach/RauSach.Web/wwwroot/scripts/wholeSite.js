jQuery(document).ready(function () {

    "use strict";
    
    preventInputNegativeNumber();

    initPreviewImage();
    $("img.lazyload").lazyload();   
});

$(document).on('DOMNodeInserted', 'body', function () {
    $("img.lazyload").lazyload();
});

$(document).on('click', 'div.productItem01.proGarden .proThumb', function () {
    $(this).parent().find('a.aGarden')[0].click();
});


function preventInputNegativeNumber() {
    $("input[type=number]").keydown(function (e) {
        if (e.keyCode === 9 || e.keyCode === 13) {
            return true;
        }
        if (!((e.keyCode > 95 && e.keyCode < 106)
            || (e.keyCode > 47 && e.keyCode < 58)
            || e.keyCode === 8)) {
            return false;
        }
    });
}

function initPreviewImage() {
    var canvas = $("#canvas");
    if (canvas.length)
        context = canvas.get(0).getContext("2d");

    $('#fileInput').on('change', function () {
        if (this.files && this.files[0]) {
            if (this.files[0].type.match(/^image\//)) {
                var reader = new FileReader();
                reader.onload = function (evt) {
                    var img = new Image();
                    img.onload = function () {
                        context.canvas.height = img.height;
                        context.canvas.width = img.width;
                        context.drawImage(img, 0, 0);
                    };
                    img.src = evt.target.result;
                };
                reader.readAsDataURL(this.files[0]);
                $("#img-origin").hide();
            }
            else {
                alert("Invalid file type! Please select an image file.");
            }
        }
        else {
            alert('No file(s) selected.');
        }
    });
}

var Common = function() {};

Common.prototype.initTinyMce = function(selector) {
    tinymce.init({
        selector: selector,
        plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
        images_upload_url: '/article/upload',
        image_class_list: [
            { title: 'Responsive', value: 'img-responsive' }
            //{ title: 'None', value: '' } 
        ]
    });
}

var common = new Common();