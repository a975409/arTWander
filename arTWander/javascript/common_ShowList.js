
$(document).ready(function () {

    let theShowId;
    let btnATMS = document.querySelectorAll('.addToMyShow');

    for (let item of btnATMS) {
        item.addEventListener('click', function (e) {
            // 阻止a標籤預設行為
            e.preventDefault();
            // 
            theShowId = e.target.childNodes[1].value;
            alert('被點擊');
            addToMyShow(theShowId);
        });
    };

    function addToMyShow(myshowId)
    {
        let addToMyShowData = { showId: myshowId };
        $.ajax({
            type: 'post',
            dataType: 'json',
            contentType: 'application/json;charset=utf-8',
            url: '/Common/addToMyShow',
            data: JSON.stringify(addToMyShowData),
            success: function (data) {
                if (data == 'success')
                    alert('yeah');
            },
            error: function (data) {
                alert('已添加到我的展覽');
                alert(XMLHttpRequest.status); //unde
                alert(XMLHttpRequest.readyState);
                /*alert(textStatus);*/
            }
            
        });

    };
});
    

