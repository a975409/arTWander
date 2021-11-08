$(function () {

    // 取得 上傳及預覽用的dom
    let avatarUploader = document.getElementById('avatarUploader');
    let avatarImgDom = document.getElementById('userAvatarImg');
    // 取得 儲存頭像檔案名稱的dom
    let dNoneInput = document.getElementById('avatarNameInput');

    avatarUploader.addEventListener('change', function (e) {
        // 取得user上傳之檔案
        let avatarImg = avatarUploader.files[0];

        // 取得user上傳之檔案的blob
        let objUrl = URL.createObjectURL(avatarImg);

        // 改變儲存的頭像檔案名稱、img src、img alt
        dNoneInput.value = avatarImg.name;
        avatarImgDom.getAttributeNode('alt').value = avatarImg.name;
        avatarImgDom.getAttributeNode('src').value = objUrl;
    }, false);


    // 前端驗證
    let Birthday = document.getElementById('Birthday');
    let AccountAddress = document.getElementById('AccountAddress');
    let btnSubmit = document.getElementById('btnSubmit')

    btnSubmit.addEventListener('click', function (e) {

        if (Birthday.value == "" || AccountAddress.value == "") {
            e.preventDefault();
            Swal.fire({
                //title: '<h5 style="color: rgb(197, 126, 104); font-weight: bold;">歡迎您加入arTwander !</h5>',
                icon: 'warning',
                html: '<div class="container"><div class="row justify-content-center"><div class="col-12"><p style="font-size:16px;">為展示適合您年齡區間的展覽及使用行程安排功能<br>請提供您的出生年月日及一組常用地址</p></div></div></div>',

            })
        }

    });

    //$('#btnSubmit').on('submit', function (e) {

    //    alert('進來了');
    //    if (Birthday.value == "") {
    //        alert('no birthday');
    //    }

    //});
});