$(document).ready(function(){

    window.addEventListener("scroll",()=>{
        
        let y = window.scrollY;

        let main = document.querySelector('body>main');
        let section1 = document.querySelector('#section1');
        let section2 = document.querySelector('#section2');

        // let bodyH = document.querySelector('body').offsetHeight;
        let headerH = document.querySelector('header').offsetHeight;
        let mainH = main.offsetHeight;
        let section1H = section1.offsetHeight;
        // let section2H = section2.offsetHeight;
        // let footerH = document.querySelector('footer').offsetHeight;

        if( y > (headerH - mainH)){
            main.classList.add('show');
        }
        if( y > (headerH + mainH)){
            section1.classList.add('show');
        }
        if( y > (headerH + mainH + section1H)){
            section2.classList.add('show');
        }
    });
});