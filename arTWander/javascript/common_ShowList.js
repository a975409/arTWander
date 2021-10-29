
$(function () {
    let isSelectedCitys = document.querySelectorAll('#cityForm input[name="isSelectedCity"]');
    let submitBtns = document.querySelectorAll('#cityForm input[name="submit"]');

    for (var item of submitBtns) {
        item.addEventListener('click', function () {
            for (var SelectedCityInput of isSelectedCitys) {
                SelectedCityInput.nodeValue = "true";
            }
        });;
    }
})
