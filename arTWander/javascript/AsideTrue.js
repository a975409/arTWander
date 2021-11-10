var c = css.sheet;
let fixAside = document.getElementById("aside1");
let NavbarFixed = document.getElementById("navbarFixed");
let menuCheckbox = document.getElementById("menuControl");
let navCheckbox = document.getElementById("navControl");
let backGroundClick = document.getElementById("mainPage");
let MySubscriptionClick = document.getElementById("MySubscriptionBtn");

//判斷是否存在aside
if (fixAside){
    // console.log("在");
}
else{
    // console.log("不在");
     c.insertRule("#navbarFixed#navbarFixed::before{display:none;}", 0);
}

window.addEventListener("click", function () {
    if (fixAside){
        // console.log("在");
    
        if(menuCheckbox.checked == true)
        {
            navCheckbox.checked = true;
            backGroundClick.onclick=function(){
                navCheckbox.checked = false;
                menuCheckbox.checked = false;
            };

        }else{
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
    if (fixAside){
        // console.log("在");
        // 改變fixed導覽列位置
        if (y > top) {
            fixedNav.classList.add('navbarFixed_Slide');
            timeOut = this.setTimeout(function(){
                fixedNav.classList.add('navbarFixed_active');
                fixedNav.classList.add('fixed-top');
                fixAside.classList.add('aside1_Slide');
            },)
        } else {
            fixedNav.classList.remove('fixed-top','navbarFixed_active','navbarFixed_Slide');
            fixAside.classList.remove('aside1_Slide');
        }
    }
    else{
        // console.log("不在");
        if (y > top) {
            fixedNav.classList.add('navbarFixed_Slide');
            timeOut = this.setTimeout(function(){
                fixedNav.classList.add('navbarFixed_active');
                fixedNav.classList.add('fixed-top');
            },)
        } else {
            fixedNav.classList.remove('fixed-top','navbarFixed_active','navbarFixed_Slide');
        }
    }

    
});

