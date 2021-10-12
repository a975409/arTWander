window.addEventListener("scroll", function () {
    // 取得fixed導覽列及section的dom
    let topHeader = document.getElementById("topHeader");
    let fixedNav = document.getElementById("navbarFixed");

    // 取得使用者滑動位置
    let y = window.scrollY;

    // 取得Section的高
    let top = topHeader.offsetHeight;
    let timeOut;
    // 改變fixed導覽列位置
    if (y > top) {
        fixedNav.classList.add('navbarFixed_Slide');
        timeOut = this.setTimeout(function () {
            fixedNav.classList.add('navbarFixed_active');
            fixedNav.classList.add('fixed-top');
        });
    } else {
        fixedNav.classList.remove('fixed-top', 'navbarFixed_active', 'navbarFixed_Slide');
    }
});

//點選左上角選單，回到最上方的畫面
var mainleftmenu = document.getElementById("mainleftmenu");
mainleftmenu.addEventListener('click', function () {
    document.body.scrollIntoView(true);
});