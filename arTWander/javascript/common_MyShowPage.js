
$(document).ready(function () {

    // 當地區選擇器的值改變時
    $("#citySelector").change(function () {

        let cityId = $('#citySelector').val()

        $.ajax({
            type: 'post',
            url: '/Common/MyshowPage?cityId='+ cityId,
            data: {},
            success: function (data) {
                $('#mainPage').html(data);
            },
            error: function (data) {
                alert(XMLHttpRequest.status);
                alert(XMLHttpRequest.readyState);

            }

        });
    });

    // 刪除我的展覽清單中的展覽
    let theShowId;
    let btnDeleFromMyShow = document.querySelectorAll('.MyshowDeleteIcon');

    for (let item of btnDeleFromMyShow) {
        item.addEventListener('click', function (e) {
            // 阻止a標籤預設行為
            e.preventDefault();

            // sweetAlert
            Swal.fire({
                icon: 'warning',
                html: '<p>確定要從我的展覽中移除此展覽嗎 ?</p>',
                showConfirmButton: true,
                showCancelButton: true,
                confirmButtonText: '是',
                cancelButtonText: '否',
            }).then((result) => {
                if (result.isConfirmed) {
                    // 取得所選展覽id
                    theShowId = item.childNodes[3].value;
                    DeleFromMyShow(theShowId);

                } else if (result.isDismissed) {}
            })
           
        });
    };

    function DeleFromMyShow(showId) {
        let DeleFromMyShowData = { showId: showId };
        $.ajax({
            type: 'post',
            contentType: 'application/json;charset=utf-8',
            url: '/Common/deleFromMyShow',
            data: JSON.stringify(DeleFromMyShowData),
            success: function (data) {
                Swal.fire({
                    icon: 'success',
                    text: '已從我的展覽清單移除此展覽'
                })
                reloadShowPage();
            },
            error: function (data) {
                alert(XMLHttpRequest.status);
                alert(XMLHttpRequest.readyState);
            }

        });

    };

    function reloadShowPage() {
        $.ajax({
            type: 'post',
            url: '/Common/MyshowPage',
            data: {},
            success: function (data) {
                $('#mainPage').html(data);
            },
            error: function (data) {
                alert(XMLHttpRequest.status);
                alert(XMLHttpRequest.readyState);
            }
        });
    };

});
