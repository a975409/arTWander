let stars = document.getElementsByClassName('star');
let input_star = document.getElementById("star-num");

var check = false;
var star = 0;

function Alldark() {
  if (!check) {
    for (var i = 0; i < stars.length; i++) {
      stars[i].classList.remove("fas");
      stars[i].classList.remove("fa-star");
      stars[i].classList.remove("text-warning");

      stars[i].classList.add("far");
      stars[i].classList.add("fa-star");
      stars[i].classList.add("text-dark");
    }
  }
}

function change(e) {
  let count = e.target.id;
  star = count;
  input_star.value = 0;
  check = false;

  for (var i = 0; i < count; i++) {
    stars[i].classList.remove("far");
    stars[i].classList.remove("fa-star");
    stars[i].classList.remove("text-dark");

    stars[i].classList.add("fas");
    stars[i].classList.add("fa-star");
    stars[i].classList.add("text-warning");
  }

  for (var i = count; i < stars.length; i++) {
    stars[i].classList.remove("fas");
    stars[i].classList.remove("fa-star");
    stars[i].classList.remove("text-warning");

    stars[i].classList.add("far");
    stars[i].classList.add("fa-star");
    stars[i].classList.add("text-dark");
  }
}

function checkClick() {
  check = true;
  input_star.value = star;
}

for (var item of stars) {
  item.addEventListener('mousedown', checkClick);
  item.addEventListener('mouseout', Alldark);
  item.addEventListener('mouseover', change);
}