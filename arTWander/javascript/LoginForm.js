

//當Email欄位的值改變時，驗證該欄位是否成功，失敗的話則顯示錯誤訊息
$('#LoginEmail').change(function () {
    let data = $(this).val();
    let LoginEmailMsg = $('#LoginEmailMsg');

    if (!inputRequire(data)) {
        LoginEmailMsg.html('請輸入Email !!');
    }
    else if (!IsEmail(data)) {
        LoginEmailMsg.html('請輸入有效的Email！!');
    } else {
        LoginEmailMsg.html('');
    }
});

//當密碼欄位的值改變時，驗證該欄位是否成功，失敗的話則顯示錯誤訊息
$('#LoginPwd').change(function () {
    let data = $(this).val();
    let LoginPwdMsg = $('#LoginPwdMsg');

    if (!inputRequire(data)) {
        LoginPwdMsg.html('請輸入密碼 !!');
    } else {
        LoginPwdMsg.html('');
    }
});