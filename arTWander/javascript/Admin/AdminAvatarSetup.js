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
        
        avatarImgDom.getAttributeNode('alt').value = avatarImg.name;
        avatarImgDom.getAttributeNode('src').value = objUrl;
        var today = new Date();
        var date = today.getFullYear() + (today.getMonth() + 1).toString() + today.getDate() + "_" + today.getHours().toString() + today.getMinutes() + today.getSeconds();
        dNoneInput.value = date + "." + avatarImg.name.split('.').pop();
    }, false);
});