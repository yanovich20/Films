$('#poster').change(function (e) {
    var input = $(this)[0];
    if (input.files && input.files[0]) {
        if (input.files[0].size > 200000000) {
            e.preventDefault();
            alert("Файл слишком большой");
            input.val("");
            return false;
        }
        if (input.files[0].type.match('image.*')) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#img').attr('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        } else {
            alert("Не изображение");
            e.preventDefault();
            console.log('ошибка, не изображение');
            input.val("");
            return false;
        }
    } else {
        alert("Ошибка");
        console.log('хьюстон у нас проблема');
    }
});
$('#reset').on("click", function () {
    $("#img").attr("src", "");
    $("#poster").val("");
});