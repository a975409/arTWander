//提交表單時，驗證Email和密碼欄位，如果驗證失敗，則取消提交表單
$('#registerForm').submit(function () {
    let RegEmail = $('#RegEmail').val();
    let RegPwd = $('#RegPwd').val();
    let RegPwdConfirm = $('#RegPwdConfirm').val();

    let RegEmailResult = inputRequire(RegEmail) && IsEmail(RegEmail);
    let RegPwdResult = inputRequire(RegPwd) && pwdformatCheck(RegPwd);
    let RegPwdConfirmResult = inputRequire(RegPwdConfirm) && pwdformatCheck(RegPwdConfirm) && pwdCompare(RegPwdConfirm, RegPwd);

    if (!RegEmailResult || !RegPwdResult || !RegPwdConfirmResult) {
        $('#RegEmail').trigger('change');
        $('#RegPwd').trigger('change');
        $('#RegPwdConfirm').trigger('change');
        return false;
    }
});

//當Email欄位的值改變時，驗證該欄位是否成功，失敗的話則顯示錯誤訊息
$('#RegEmail').change(function () {
    let data = $(this).val();
    let RegEmailMsg = $('#RegEmailMsg');

    if (!inputRequire(data)) {
        RegEmailMsg.html('請輸入Email');
    }
    else if (!IsEmail(data)) {
        RegEmailMsg.html('請輸入有效的Email！');
    } else {
        RegEmailMsg.html('');
    }
});

//當密碼欄位的值改變時，驗證該欄位是否成功，失敗的話則顯示錯誤訊息
$('#RegPwd').change(function () {
    let data = $(this).val();
    let RegPwdMsg = $('#RegPwdMsg');

    if (!inputRequire(data)) {
        RegPwdMsg.html('請輸入密碼 !!');
    } else if (!pwdformatCheck(data)) {
        RegPwdMsg.html('密碼必須8個字以上，包含英文大小寫、數字和特殊符號');
    } else {
        RegPwdMsg.html('');
    }
});

//當密碼欄位的值改變時，驗證該欄位是否成功，失敗的話則顯示錯誤訊息
$('#RegPwdConfirm').change(function () {
    let data = $(this).val();
    let RegConfirmMsg = $('#RegConfirmMsg');

    if (!inputRequire(data)) {
        RegConfirmMsg.html('請輸入密碼 !!');
    } else if (!pwdCompare(data, $('#RegPwd').val())) {
        RegConfirmMsg.html('與輸入的密碼不相符');
    } else {
        RegConfirmMsg.html('');
    }
});