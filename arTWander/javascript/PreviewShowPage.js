
function PreviewShowPage() {
    let imgFiles = document.querySelector('[name="imgFiles"][id="imgFiles"]');
    let showImgs = document.querySelectorAll('#showImgs img');
    let StartDate = $('#StartDate').val();
    let EndDate = $('#EndDate').val();
    let Todays = document.getElementsByName('Todays');
    let StartTime = $('#StartTime').val();
    let EndTime = $('#EndTime').val();
    let Cost = document.getElementById("Cost");
    let AgeRange = document.getElementById("AgeRange");
    let City = document.getElementById("FK_City");
    let District = document.getElementById("FK_District");

    //展演標題
    $('#titleName').html($('#Title').val());

    //輪播圖
    let imgcount = 0;
    $('#carousel-indicators').html('');
    $('#carousel-inner').html('');

    for (let index in showImgs) {

        if (showImgs[index].nodeType === 1) {
            let objUrl = showImgs[index].src;

            if (index == 0) {
                $('#carousel-indicators').append('<li data-target="#carouselExampleIndicators" data-slide-to="1" class="active"></li>');
                $('#carousel-inner').append(`<div class="carousel-item text-center active"><img src="${objUrl}" class="img-fluid" alt="..."></div>`);
            } else {
                $('#carousel-indicators').append(`<li data-target="#carouselExampleIndicators" data-slide-to="${imgcount + 1}"></li>`);
                $('#carousel-inner').append(`<div class="carousel-item text-center"><img src="${objUrl}" class="img-fluid" alt="..."></div>`);
            }

            imgcount++;
        }
    }

    if (imgFiles.files != null && imgFiles.files.length > 0) {
        for (let index = 0; index < imgFiles.files.length; index++) {

            let img = imgFiles.files[index];
            let objUrl = URL.createObjectURL(img);

            if (index == 0 && showImgs.length <= 0) {
                $('#carousel-indicators').append(`<li data-target="#carouselExampleIndicators" data-slide-to="${imgcount}" class="active"></li>`);
                $('#carousel-inner').append(`<div class="carousel-item text-center active"><img src="${objUrl}" class="img-fluid" alt="..."></div>`);
            } else {
                $('#carousel-indicators').append(`<li data-target="#carouselExampleIndicators" data-slide-to="${imgcount}"></li>`);
                $('#carousel-inner').append(`<div class="carousel-item text-center"><img src="${objUrl}" class="img-fluid" alt="..."></div>`);
            }

            imgcount++;
        }
    }

    //日期、時間、開放時段
    $('#dateRange').html(`${StartDate} - ${EndDate}`);
    $('#timeRange').html(`${StartTime} - ${EndTime}`);
    $('#todayItems').html('');
    let days = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];

    for (let index in Todays) {
        if (Todays[index].nodeType === 1) {
            if (Todays[index].checked) {
                $('#todayItems').append(`<span class="col-lg-2"><i class="fas fa-check-square text-primary"></i> ${days[index]}</span>`)
            } else {
                $('#todayItems').append(`<span class="col-lg-2"><i class="far fa-square""></i> ${days[index]}</span>`)
            }
        }
    }

    //地點
    let cityName = City.options[City.selectedIndex].innerHTML;
    let districtName = District.options[District.selectedIndex].innerHTML;
    let Address = `${cityName}${districtName}${$('#Address').val()}`;

    $('#addr').href = `https://www.google.com/maps/search/?api=1&query=${Address}`;
    $('#addr').html(`<i class="fas fa-map-marker-alt"> ${Address}</i>`);

    $('#cost').html(Cost.options[Cost.selectedIndex].innerHTML);
    $('#price').html(`${$('#Price').val()} 元`);
    $('#ageRange').html(AgeRange.options[AgeRange.selectedIndex].innerHTML);

    $('#remark').html($('#Remark').val());

    $('#description').html($('#Description').val());

    //關鍵字
    let keywords = document.getElementsByName('keywordSpan');
    $('#keywordItem').html('');

    if (keywords.length > 0) {
        for (let index in keywords) {

            if (keywords[index].nodeType === 1) {
                $('#keywordItem').append(`<a href="#" class="badge badge-primary"><i class="fas fa-tag"> ${keywords[index].textContent}</i></a>`);
            }
        }
    }
}

function submitEditForm() {
    Swal.fire({
        title: '更新展覽?',
        text: "是否要更新此展覽？",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '是！確定更新',
        cancelButtonText: '否！繼續編輯',
    }).then((result) => {
        if (result.isConfirmed) {
            $('#CreateShowForm').submit();
        }
    });
}

function submitCreateForm() {
    Swal.fire({
        title: '發佈展覽?',
        text: "是否要發佈此展覽？",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '是！確定發佈',
        cancelButtonText: '否！繼續編輯',
    }).then((result) => {
        if (result.isConfirmed) {
            $('#CreateShowForm').submit();
        }
    });
}