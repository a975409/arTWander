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
    accept: 'gif|jpeg|jpg||png|webp|svg|tiff|icon',
    STRING: {
        file: '<em title="Click to remove" onclick="$(this).parent().prev().click()">$file</em>',
        remove: '<i class="fas fa-times"></i>',
        selected: '$file',
        denied: '檔案指定失敗，原因如下：\n$ext!',
        duplicate: '下列圖檔已重複選取:\n$file!'
    },
    afterFileSelect: function (element, value, master_element) {

        if (value.length > 20) {
            let imgFiles = document.querySelectorAll('.MultiFile-remove+span>.MultiFile-label');
            let a_remove = document.getElementsByClassName('MultiFile-remove');

            for (let i in imgFiles) {
                if (imgFiles[i].nodeType == 1) {
                    //    console.log(imgFiles[i].title);

                    if (imgFiles[i].title == value) {
                        a_remove[i].click();
                        break;
                    }
                }
            }

            Swal.fire({
                icon: 'error',
                title: '無法指定檔案',
                text: '檔名+副檔名超過20個字',
                showConfirmButton: false,
                showCancelButton: true
            });
        }
    }
});

$('#inputKeywordBtn').click(function () {
    let inputKeyword = $('#inputKeyword').val();

    if (inputKeyword != '') {
        $('#taglist').append(`<div class="tag alert alert-primary alert-dismissible fade show" role="alert"><div class="d-flex justify-content-between align-items-center"><span name="keywordSpan">${inputKeyword}</span><button type="button" class="close d-flex align-items-center" data-dismiss="alert" aria-label="Close"><i class="fas fa-times"></i></button></div></div>`);

        $('#inputKeyword').val('');
    }
});

$('#CreateShowForm').submit(function () {

    let Description = $('#Description').summernote('code');
    $('#Description').val()

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

function getShowImages(showPageId) {
    $.ajax({
        type: 'get',
        url: './getShowImages/ShowPageManage',
        data: { showPageId: showPageId },
        success: function (data) {
            $('#showImgs').html('');
            $('#showImgs').append(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Swal.fire({
                icon: 'error',
                title: textStatus,
                text: errorThrown,
                showConfirmButton: false,
                showCancelButton: true
            });
        }
    })
}

function removeShowImage(imgId, showPageId) {
    Swal.fire({
        title: '移除圖片?',
        text: "是否要移除這張已上傳的圖片？",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '是！確定移除',
        cancelButtonText: '否！不要移除',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'post',
                url: './removeShowImage/ShowPageManage',
                data: { imgId: imgId, showPageId: showPageId},
                success: function (data) {
                    Swal.fire(
                        '移除成功!',
                        '已移除圖片',
                        'success'
                    ).then(function () {
                        getShowImages(showPageId);
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    Swal.fire({
                        icon: 'error',
                        title: textStatus,
                        text: errorThrown,
                        showConfirmButton: false,
                        showCancelButton: true
                    });
                }
            });
        }
    });
}