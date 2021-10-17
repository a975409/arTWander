//驗證欄位是否為空
function inputRequire(value) {
    return value != '';
}

//驗證是否輸入正確的Email
function IsEmail(email) {
    //var regex = /^([\.a-zA-Z0-9_-]) @@([a-zA-Z0-9_-]) (\.[a-zA-Z0-9_-]) /;
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

//驗證密碼格式
function pwdformatCheck(pwd) {
    // 以下是這個正規表示式的 pattern 的內容:
    // 密碼長度須超過八個字 {8,}
    // 密碼複雜度須包含:
    // - 小寫字母 [a-z]
    // - 大寫字母 [A-Z]
    // - 數字 \d
    // - 特殊符號 "#$%&'()*+,./:;<=>?@@[]^_`{|}~-

    var regex = new RegExp(/^((?=.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{8,}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@@'[\]\^_`{|}~-]).*)/, "g");

    return pwd.match(regex);
}

//密碼比對
function pwdCompare(pwd, pwdConfirm) {
    return pwd == pwdConfirm;
}