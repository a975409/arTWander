window.addEventListener('resize', windowResize);
windowResize();
function windowResize(){
  let wd = this;
  let h = wd.innerHeight;
  let scrollContent = document.getElementById("scrollContent");
  let height = h - scrollContent.offsetTop;
  scrollContent.style.height = height + "px";
}