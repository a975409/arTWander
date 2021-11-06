////window.addEventListener('resize', windowResize);
////windowResize();
////function windowResize(){
////  let wd = this;
////  let h = wd.innerHeight;
////  let scrollContent = document.getElementById("scrollContent");
////  let height = h - scrollContent.offsetTop;
////  scrollContent.style.height = height + "px";
////}
$('#Cost').change(function () {
    let result = $(this).val();
    if (result == 'true') {
        $('#displayPrice').css({ "display": "block" });
    } else {
        $('#displayPrice').css({ "display": "none" });
        $('#Price').val('0');
    }
});

$('#imgFiles').MultiFile({
    max: 6,
    list: '#showImg',
    accept: 'gif|jpeg|jpg|png|webp|svg|tiff|icon',
    STRING: {
        file: '<em title="Click to remove" onclick="$(this).parent().prev().click()">$file</em>',
        remove: '<i class="fas fa-times"></i>',
        selected: '$file',
        denied: '檔案指定失敗，原因如下：\n$ext!',
        duplicate: '下列圖檔已重複選取:\n$file!'
    }
});

function checkImgFileName(fileName) {
    var regex = new RegExp(/^[^\/\:\*\?\""\<\>\|\,]+$/, "g");

    return !fileName.match(regex);
}

$('#inputKeywordBtn').click(function () {
    let inputKeyword = $('#inputKeyword').val();

    if (inputKeyword.search(',') > 0) {
        Swal.fire({
            icon: 'error',
            title: '無法新增關鍵字',
            text: '關鍵字裡面不能有半形逗號","',
            showConfirmButton: false,
            showCancelButton: true
        });
        $('#inputKeyword').val('');
        return;
    }

    if (inputKeyword != '') {
        $('#taglist').append(`<div class="tag alert alert-primary alert-dismissible fade show" role="alert"><div class="d-flex justify-content-between align-items-center"><span name="keywordSpan">${inputKeyword}</span><button type="button" class="close d-flex align-items-center" data-dismiss="alert" aria-label="Close"><i class="fas fa-times"></i></button></div></div>`);

        $('#inputKeyword').val('');
    }
});

$('#CreateShowForm').submit(function () {

    //先將選取的關鍵字新增至searchKeyword
    let keywords = document.getElementsByName('keywordSpan');
    let result = '';

    if (keywords.length > 0) {
        for (let index in keywords) {

            if (keywords[index].nodeType === 1) {
                if (result != '')
                    result += ',';

                result += keywords[index].textContent;
            }
        }
    }

    $('#searchKeyword').val(result);
});