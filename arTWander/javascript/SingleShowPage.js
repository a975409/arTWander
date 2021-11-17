let stars = document.getElementsByClassName('star');
let input_star = document.getElementById("star-num");

var check = false;
var star = 0;

function Alldark() {
    if (!check) {
        for (var i = 0; i < stars.length; i++) {
            stars[i].classList.remove("fas");
            stars[i].classList.remove("fa-star");
            stars[i].classList.remove("text-warning");

            stars[i].classList.add("far");
            stars[i].classList.add("fa-star");
            stars[i].classList.add("text-dark");
        }
    }
}

function change(e) {
    let count = e.target.id;
    star = count;
    input_star.value = 0;
    check = false;

    for (var i = 0; i < count; i++) {
        stars[i].classList.remove("far");
        stars[i].classList.remove("fa-star");
        stars[i].classList.remove("text-dark");

        stars[i].classList.add("fas");
        stars[i].classList.add("fa-star");
        stars[i].classList.add("text-warning");
    }

    for (var i = count; i < stars.length; i++) {
        stars[i].classList.remove("fas");
        stars[i].classList.remove("fa-star");
        stars[i].classList.remove("text-warning");

        stars[i].classList.add("far");
        stars[i].classList.add("fa-star");
        stars[i].classList.add("text-dark");
    }
}

function checkClick() {
    check = true;
    input_star.value = star;
}

for (var item of stars) {
    item.addEventListener('mousedown', checkClick);
    item.addEventListener('mouseout', Alldark);
    item.addEventListener('mouseover', change);
}

function removeShowPage(showPageId) {
    Swal.fire({
        title: '移除展覽?',
        text: "是否要移除此展覽？",
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
                url: './ShowPageManage/Delete',
                data: { showPageId: showPageId },
                success: function (data) {
                    Swal.fire(
                        '移除成功!',
                        '已移除此展覽',
                        'success'
                    ).then(function () {
                        location.href = './Index/ShowPageManage';
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    Swal.fire({
                        icon: 'error',
                        title: textStatus,
                        text: errorThrown,
                        showConfirmButton: false,
                        showCancelButton: true
                    }).then(function () {
                        location.href = './Index/ShowPageManage';
                    });
                }
            });
        }
    });
}