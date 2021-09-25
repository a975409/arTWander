// fixed導覽列的移動功能

window.addEventListener("scroll", function () {
    // 取得fixed導覽列及section的dom
    let topHeader = document.getElementById("topHeader");
    let fixedNav = document.getElementById("navbarFixed");
    let fixAside = document.getElementById("aside1");
    let menuCheckbox = document.getElementById("menuControl");
    let navCheckbox = document.getElementById("navControl");
    // 取得使用者滑動位置
    let y = window.scrollY;
    console.log(y);

    // 取得Section的高
    let top = topHeader.offsetHeight;
    let timeOut;
    // navbar control開關
    if(menuCheckbox.checked == true)
    {
        navCheckbox.checked = true;
    }else{
        navCheckbox.checked = false;
    }
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
    
});