$('#CompanyEdit').submit(function (event) {
    let CompanyName = $('#CompanyName').val();
    let CompanyDescription = $('#CompanyDescription').val();
    let HomePage = $('#HomePage').val();
    let BusinessHours = $('#BusinessHours').val();
    let Address = $('#Address').val();
    let Email = $('#Email').val();
    let Phone = $('#Phone').val();
    let Fax = $('#Fax').val();

    let CompanyNameResult = inputRequire(CompanyName) && (CompanyName.length <= 10);
    let CompanyDescriptionResult = inputRequire(CompanyDescription) && (CompanyDescription.length >= 200) && (CompanyDescription.length <= 300);
    let HomePageResult = !inputRequire(HomePage) || ((HomePage.length <= 100) && IsUrl(data));
    let BusinessHoursResult = inputRequire(BusinessHours) && (BusinessHours.length <= 100);
    let AddressResult = inputRequire(Address) && (Address.length <= 100);
    let EmailResult = inputRequire(Email) && (Email.length <= 100) && IsEmail(Email);
    let PhoneResult = inputRequire(Phone) && (Phone.length <= 20);
    let FaxResult = Fax.length <= 20;

    if (!CompanyNameResult || !CompanyDescriptionResult || !HomePageResult || !BusinessHoursResult || !AddressResult || !EmailResult || !PhoneResult || !FaxResult) {
        $('#CompanyName').trigger('change');
        $('#CompanyDescription').trigger('change');
        $('#HomePage').trigger('change');
        $('#BusinessHours').trigger('change');
        $('#Address').trigger('change');
        $('#Email').trigger('change');
        $('#Phone').trigger('change');
        $('#Fax').trigger('change');
        return false;
    }
});

$('#Promotional').change(function (event) {

    if (checkfile(this)) {
        let ImgFile = document.getElementById('Promotional').files[0];
        let objUrl = URL.createObjectURL(ImgFile);
        document.getElementById('PromotionalImage').src = objUrl;
    } else {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '請選擇圖檔',
            showConfirmButton: false,
            showCancelButton: true
        });
    }
});

$('#PhotoSticker').change(function () {

    if (checkfile(this)) {
        let ImgFile = document.getElementById('PhotoSticker').files[0];
        let objUrl = URL.createObjectURL(ImgFile);
        document.getElementById('PhotoStickerImg').src = objUrl;
    } else {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '請選擇圖檔',
            showConfirmButton: false,
            showCancelButton: true
        });
    }
});

function checkfile(sender) {

    // 可接受的附檔名
    var validExts = new Array(".gif", ".jpeg", ".jpg", ".png", ".webp", ".svg", ".tiff", ".icon");

    var fileExt = sender.value;
    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
    if (validExts.indexOf(fileExt) < 0) {
        //alert("檔案類型錯誤，可接受的副檔名有：" + validExts.toString());
        sender.value = null;
        return false;
    }
    else return true;
}

$('#CompanyName').change(function () {
    let data = $(this).val();
    let Msg = $('#CompanyNameMsg');

    if (!inputRequire(data)) {
        Msg.html('請填寫展演單位名稱 !!');
    } else if (data.length > 10) {
        Msg.html('展演單位名稱不得超過10個字 !!');
    }else {
        Msg.html('');
    }
});

$('#CompanyDescription').change(function () {
    let data = $(this).val();
    let Msg = $('#CompanyDescriptionMsg');

    if (!inputRequire(data)) {
        Msg.html('請填寫展演單位簡介 !!');
    }
    else if (data.length < 200 || data.length > 300) {
        Msg.html('填寫展演單位簡介，限字數200～300字 !!');
    }
    else {
        Msg.html('');
    }
});

$('#HomePage').change(function () {
    let data = $(this).val();
    let Msg = $('#HomePageMsg');

    if (data.length > 100) {
        Msg.html('網址字數不得超過100個字 !!');
    }
    else if (inputRequire(data) && !IsUrl(data)) {
        Msg.html('請輸入有效網址 !!');
    }
    else {
        Msg.html('');
    }
});

$('#BusinessHours').change(function () {
    let data = $(this).val();
    let Msg = $('#BusinessHoursMsg');

    if (!inputRequire(data)) {
        Msg.html('請填寫營業時間 !!');
    }
    else if (data.length > 100) {
        Msg.html('營業時間字數不得超過100個字 !!');
    }
    else {
        Msg.html('');
    }
});

$('#Address').change(function () {
    let data = $(this).val();
    let Msg = $('#AddressMsg');

    if (!inputRequire(data)) {
        Msg.html('請填寫地址 !!');
    }
    else if (data.length > 100) {
        Msg.html('地址不得超過100個字 !!');
    }
    else {
        Msg.html('');
    }
});

//當Email欄位的值改變時，驗證該欄位是否成功，失敗的話則顯示錯誤訊息
$('#Email').change(function () {
    let data = $(this).val();
    let LoginEmailMsg = $('#EmailMsg');

    if (!inputRequire(data)) {
        LoginEmailMsg.html('請輸入Email !!');
    }
    else if (!IsEmail(data)) {
        LoginEmailMsg.html('請輸入有效的Email！!');
    } else {
        LoginEmailMsg.html('');
    }
});

$('#Phone').change(function () {
    let data = $(this).val();
    let Msg = $('#PhoneMsg');

    if (!inputRequire(data)) {
        Msg.html('請填寫聯絡電話 !!');
    }
    else if (data.length > 20) {
        Msg.html('聯絡電話不得超過20個字 !!');
    }
    else {
        Msg.html('');
    }
});

$('#Fax').change(function () {
    let data = $(this).val();
    let Msg = $('#FaxMsg');

    if (data.length > 20) {
        Msg.html('傳真號碼不得超過20個字 !!');
    }
    else {
        Msg.html('');
    }
});


//驗證欄位是否為空
function inputRequire(value) {

    if (!$.trim(value.replace(/\　/g, "")))
        return false;

    return true;
}

//驗證是否輸入正確的Email
function IsEmail(email) {
    //var regex = /^([\.a-zA-Z0-9_-]) @@([a-zA-Z0-9_-]) (\.[a-zA-Z0-9_-]) /;
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

//驗證網址
function IsUrl(url) {
    //var regex = /^([\.a-zA-Z0-9_-]) @@([a-zA-Z0-9_-]) (\.[a-zA-Z0-9_-]) /;
    var regex = /^((https?|ftp|file):\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/;
    return regex.test(url);
}