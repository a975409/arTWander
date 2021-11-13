$(function () {
    var c = css.sheet;
    let fixAside = document.getElementById("aside1");
    let NavbarFixed = document.getElementById("navbarFixed");
    let menuCheckbox = document.getElementById("menuControl");
    let navCheckbox = document.getElementById("navControl");
    let backGroundClick = document.getElementById("mainPage");
    let MySubscriptionClick = document.getElementById("MySubscriptionBtn");

    //判斷是否存在aside
    if (fixAside) {
        // console.log("在");
    }
    else {
        // console.log("不在");
        c.insertRule("#navbarFixed#navbarFixed::before{display:none;}", 0);
    }

    window.addEventListener("click", function () {
        if (fixAside) {
            // console.log("在");

            if (menuCheckbox.checked == true) {
                navCheckbox.checked = true;
                backGroundClick.onclick = function () {
                    navCheckbox.checked = false;
                    menuCheckbox.checked = false;
                };

            } else {
                navCheckbox.checked = false;
                menuCheckbox.checked = false;
            }
        }
    });


    //用於Navbar下滑動畫
    window.addEventListener("scroll", function () {
        // 取得fixed導覽列及section的dom
        let topHeader = document.getElementById("topHeader");
        let fixedNav = document.getElementById("navbarFixed");
        let fixAside = document.getElementById("aside1");

        // 取得使用者滑動位置
        let y = window.scrollY;
        // console.log(y);

        // 取得Section的高
        let top = topHeader.offsetHeight;
        let timeOut;
        // navbar control開關
        if (fixAside) {
            // console.log("在");
            // 改變fixed導覽列位置
            if (y > top) {
                fixedNav.classList.add('navbarFixed_Slide');
                timeOut = this.setTimeout(function () {
                    fixedNav.classList.add('navbarFixed_active');
                    fixedNav.classList.add('fixed-top');
                    fixAside.classList.add('aside1_Slide');
                })
            } else {
                fixedNav.classList.remove('fixed-top', 'navbarFixed_active', 'navbarFixed_Slide');
                fixAside.classList.remove('aside1_Slide');
            }
        }
        else {
            // console.log("不在");
            if (y > top) {
                fixedNav.classList.add('navbarFixed_Slide');
                timeOut = this.setTimeout(function () {
                    fixedNav.classList.add('navbarFixed_active');
                    fixedNav.classList.add('fixed-top');
                })
            } else {
                fixedNav.classList.remove('fixed-top', 'navbarFixed_active', 'navbarFixed_Slide');
            }
        }


    });


    // 意見回饋功能 ==========================================================
    let btnFeedback = document.getElementById('btnFeedback');
    let feedBack = document.getElementById('feedBack');
    let userEmail = document.getElementById('userEmail');

    // 前端驗證
    let flagOne = false;
    let flagTwo = false;

    feedBack.addEventListener('blur', function () {
        if (feedBack.value.length <= 10) {
            document.getElementById('txtAreaWarningMsg').innerText = "請至少輸入10個字元";
        }
        else
        {
            document.getElementById('txtAreaWarningMsg').innerText = "";
            flagOne = true;
        } 
    })

    userEmail.addEventListener('blur', function () {
        if (userEmail.value.match(/^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/))
        {
            document.getElementById('emailWarningMsg').innerText = "";
            flagTwo = true;
        }
        else
        {
            document.getElementById('emailWarningMsg').innerText = "請輸入正確email格式";
        }
    })

    // 使用者輸入寄送
    btnFeedback.addEventListener('click', function (e) {
        e.preventDefault();

        if (flagOne == true && flagTwo == true) {
            feedBack = document.getElementById('feedBack').value;
            userEmail = document.getElementById('userEmail').value;

            sendMsgToAdmin(feedBack, userEmail);

            Swal.fire({
                icon: 'success',
                html: '<h5>已將您的回饋送給系統管理者</h5>',
                text: '感謝您的回饋，請靜待我們的回覆'
            })
            document.getElementById('feedBack').value = "";
            document.getElementById('userEmail').value = "";
        }
    })

    function sendMsgToAdmin(feedBack, userEmail)
    {
        let feedBackData = { feedBack: feedBack, userEmail: userEmail };
        $.ajax({
            type: 'post',
            contentType: 'application/json;charset=utf-8',
            url: '/Common/sendMsgToAdminAsync',
            data: JSON.stringify(feedBackData),
            success: function (data) {
                
            },
            error: function (data) {
                alert(XMLHttpRequest.status);
                alert(XMLHttpRequest.readyState);
            }

        });
    }
});

