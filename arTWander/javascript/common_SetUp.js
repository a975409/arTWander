$(function () {

    // 取得 上傳及預覽用的dom
    let avatarUploader = document.getElementById('avatarUploader');
    let avatarImgDom = document.getElementById('userAvatarImg');
    // 取得 儲存頭像檔案名稱的dom
    let dNoneInput = document.getElementById('avatarNameInput');
    // 取得 尺寸過大訊息的dom
    let fileSizeWarning = document.getElementById('fileSizeWarning');

    avatarUploader.addEventListener('change', function (e) {
        // 取得user上傳之檔案
        let avatarImg = avatarUploader.files[0];
        // 判斷檔案大小
        if (avatarImg.size > 5242880) {
            fileSizeWarning.innerHTML = "檔案尺寸過大";
        }
        else
        {
            // 使檔案尺寸警告訊息消失
            fileSizeWarning.innerHTML = "";

            // 取得user上傳之檔案的blob
            let objUrl = URL.createObjectURL(avatarImg);

            // 改變儲存的頭像檔案名稱、img src、img alt
            dNoneInput.value = avatarImg.name;
            avatarImgDom.getAttributeNode('alt').value = avatarImg.name;
            avatarImgDom.getAttributeNode('src').value = objUrl;
            var today = new Date();
            var date = today.getFullYear() + (today.getMonth() + 1).toString() + today.getDate() + "_" + today.getHours().toString() + today.getMinutes() + today.getSeconds();
            dNoneInput.value = date + "." + avatarImg.name.split('.').pop();
        }
    }, false);


    // 前端驗證
    let Birthday = document.getElementById('Birthday');
    let AccountAddress = document.getElementById('AccountAddress');
    let btnSubmit = document.getElementById('btnSubmit')

    btnSubmit.addEventListener('click', function (e) {

        if (Birthday.value == "" || AccountAddress.value == "") {
            e.preventDefault();
            Swal.fire({
                icon: 'warning',
                html: '<div class="container"><div class="row justify-content-center"><div class="col-12"><p style="font-size:16px;">為展示適合您年齡區間的展覽及使用行程安排功能<br>請提供您的出生年月日及一組常用地址</p></div></div></div>',

            })
        }

    });

});

