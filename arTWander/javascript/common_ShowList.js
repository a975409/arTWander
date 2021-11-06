
$(document).ready(function () {

    let theShowId;
    let btnATMS = document.querySelectorAll('.addToMyShow');

    for (let item of btnATMS) {
        item.addEventListener('click', function (e) {
            // 阻止a標籤預設行為
            e.preventDefault();
            // 取得所選展覽id
            theShowId = e.target.childNodes[1].value;
            addToMyShow(theShowId);
        });
    };

    function addToMyShow(myshowId)
    {
        let addToMyShowData = { showId: myshowId };
        $.ajax({
            type: 'post',
            contentType: 'application/json;charset=utf-8',
            url: '/Common/addToMyShow',
            data: JSON.stringify(addToMyShowData),
            success: function (data) {
                Swal.fire({
                    icon: 'success',
                    text: '已添加到我的展覽'
                })
                /*alert(data);*/
            },
            error: function (data) {
                alert(XMLHttpRequest.status); 
                alert(XMLHttpRequest.readyState);
                /*alert(textStatus);*/
            }
            
        });

    };
});
    

