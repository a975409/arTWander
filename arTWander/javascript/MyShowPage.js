var HotSort = document.getElementById("HotSort");
var HotSortCount = 0;

var ViewSort = document.getElementById("ViewSort");
var ViewSortCount = 0;

var DateSort = document.getElementById("DateSort");
var DateSortCount = 0;

HotSort.addEventListener("click", function () {
  ViewSortDefault();
  DateSortDefault();
  if (HotSortCount == 0 || HotSortCount == 2) {
    HotSort.innerHTML =
      '<i class="fas fa-sort-amount-down" style="vertical-align: middle;"></i>熱門排序';
    HotSortCount = 1;
  } else if (HotSortCount == 1) {
    HotSort.innerHTML =
      '<i class="fas fa-sort-amount-up" style="vertical-align: middle;"></i>熱門排序';
    HotSortCount = 2;
  } else {}

  //$('#ShowList').load('SingleShow.html');
  //ShowList();
});

ViewSort.addEventListener("click", function () {
  HotSortDefault();
  DateSortDefault();
  if (ViewSortCount == 0 || ViewSortCount == 2) {
    ViewSort.innerHTML =
      '<i class="fas fa-sort-amount-down" style="vertical-align: middle;"></i>瀏覽數排序';
    ViewSortCount = 1;
  } else if (ViewSortCount == 1) {
    ViewSort.innerHTML =
      '<i class="fas fa-sort-amount-up" style="vertical-align: middle;"></i>瀏覽數排序';
    ViewSortCount = 2;
  } else {}

  //$('#ShowList').load('SingleShow.html');
  //ShowList();
});

DateSort.addEventListener("click", function () {
  HotSortDefault();
  ViewSortDefault();
  if (DateSortCount == 0 || DateSortCount == 2) {
    DateSort.innerHTML =
      '<i class="fas fa-sort-amount-down" style="vertical-align: middle;"></i>日期排序';
    DateSortCount = 1;
  } else if (DateSortCount == 1) {
    DateSort.innerHTML =
      '<i class="fas fa-sort-amount-up" style="vertical-align: middle;"></i>日期排序';
    DateSortCount = 2;
  } else {}
  //$('#ShowList').load('SingleShow.html');
  //ShowList();
});

HotSortDefault();
ViewSortDefault();
DateSortDefault();
//$('#ShowList').load('SingleShow.html');
//ShowList();

function HotSortDefault() {
  HotSort.innerHTML =
    '<i class="fas fa-arrows-alt-v" style="vertical-align: middle;"></i>熱門排序';
  HotSortCount = 0;
}

function ViewSortDefault() {
  ViewSort.innerHTML =
    '<i class="fas fa-arrows-alt-v" style="vertical-align: middle;"></i>瀏覽數排序';
  ViewSortCount = 0;
}

function DateSortDefault() {
  DateSort.innerHTML =
    '<i class="fas fa-arrows-alt-v" style="vertical-align: middle;"></i>日期排序';
  DateSortCount = 0;
}