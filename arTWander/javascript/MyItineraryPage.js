//iframe的網址，呈現Google Map導航路線
let searchCenter = '';

//Google Map路線網址
let googleMapUrl = '';

$(function () {
    $("#sortable").sortable();

    $('#StartDate').change(function () {
        searchShowPage();
        $('#sortable').html('');
    });

    $('#StartTime').change(function () {
        searchShowPage();
        $('#sortable').html('');
    });

    $('#EndTime').change(function () {
        searchShowPage();
        $('#sortable').html('');
    });

    $('#FK_City').change(function () {
        searchShowPage();
    });

    $('#sendToEmail').click(function () {
        sendRountToEmail();
    });

    const googleMap = new Vue({
        el: '#app',
        data: {
            map: null,
            autocomplete: null,
            site: '', // place API要綁定的搜尋框
            place: null // 存place確定後回傳的資料
        },
        methods: {
            // 地址自動完成 + 地圖的中心移到輸入結果的地址上
            siteAuto() {
                let options = {
                    componentRestrictions: {
                        country: 'tw'
                    } // 限制在台灣範圍
                };
                this.autocomplete = new google.maps.places.Autocomplete(this.$refs.site, options);
                //this.autocomplete.addListener('place_changed', () => {
                //    this.place = this.autocomplete.getPlace();
                //    if (this.place.geometry) {
                //        //searchCenter = this.place.geometry.location;
                //        searchCenter = this.place.formatted_address;
                //    }
                //});
            },
            getCurrentAddr: function (event) {
                searchCenter = event.target.value;
                resetVariable();
            }
        },
        mounted() {
            window.addEventListener('load', () => {
                this.siteAuto();
            });
        }
    });
});

//取得使用者目前所在位置
function getPosition() {
    if (navigator.geolocation) {

        // 使用者不提供權限，或是發生其它錯誤
        function error() {
            Swal.fire({
                icon: 'error',
                title: '錯誤',
                text: '無法取得你的位置',
                showConfirmButton: false,
                showCancelButton: true
            });
        }

        // 使用者允許抓目前位置，回傳經緯度
        function success(position) {
            //console.log(position.coords.latitude, position.coords.longitude);
            $('#positionOrAddr').val(`${position.coords.latitude},${position.coords.longitude}`);
            searchCenter = `${position.coords.latitude},${position.coords.longitude}`;
            googleMapUrl = '';
        }

        // 跟使用者拿所在位置的權限
        navigator.geolocation.getCurrentPosition(success, error);

    } else {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: 'Sorry, 你的裝置不支援地理位置功能。',
            showConfirmButton: false,
            showCancelButton: true
        });

        $('#positionOrAddr').val('');
    }
}

//取得多個地點的導航路線
function getAddrsUrl(event) {

    if (searchCenter == '') {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '未設定出發地點',
            showConfirmButton: false,
            showCancelButton: true
        });
        return false;
    }

    let addrDOMs = document.querySelectorAll('#sortable>tr>td[aria-address]');
    let mode = document.querySelectorAll('#travelMode>li>a[aria-selected="true"]').item(0).getAttribute('aria-mode');

    let url = '';

    if (addrDOMs.length >= 2) {
        url = `https://www.google.com/maps/embed/v1/directions?&key=AIzaSyAkbzgPohvus20UjyVFUYAW0rNnBlXBGM0&origin=${searchCenter}&waypoints=`;
        googleMapUrl = `https://www.google.com/maps/dir/?api=1&origin=${searchCenter}&waypoints=`;
        for (let index in addrDOMs) {
            if (addrDOMs[index].nodeType == 1) {
                if (index == addrDOMs.length - 1) {
                    url = url.slice(0, url.length - 1);
                    googleMapUrl = googleMapUrl.slice(0, googleMapUrl.length - 1);

                    url += `&destination=${addrDOMs[index].getAttribute('aria-address')}`;
                    googleMapUrl += `&destination=${addrDOMs[index].getAttribute('aria-address')}`;
                } else {
                    url += `${addrDOMs[index].getAttribute('aria-address')}|`;
                    googleMapUrl += `${addrDOMs[index].getAttribute('aria-address')}|`;
                }
            }
        }
    } else if (addrDOMs.length == 1) {
        url = `https://www.google.com/maps/embed/v1/directions?&key=AIzaSyAkbzgPohvus20UjyVFUYAW0rNnBlXBGM0&origin=${searchCenter}&destination=${addrDOMs[0].getAttribute('aria-address')}`;

        googleMapUrl = `https://www.google.com/maps/dir/?api=1&origin=${searchCenter}&destination=${addrDOMs[0].getAttribute('aria-address')}`;

    } else {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '未安排展覽行程',
            showConfirmButton: false,
            showCancelButton: true
        });

        return false;
    }

    if (mode != '') {
        url += `&mode=${mode}`;
        googleMapUrl += `&travelmode=${mode}`;
    }

    let resultUrl = document.getElementById('resultUrl');
    resultUrl.setAttribute('src', url);
}

function sendRountToEmail() {
    if (googleMapUrl == '') {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '沒有規劃路徑',
            showConfirmButton: false,
            showCancelButton: true
        });
        return false;
    }

    $('#sendToEmail').html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>正在寄送中...');
    $('#sendToEmail').attr("disabled", true);
    $.ajax({
        type: 'get',
        url: '/Common/SendRountToEmail',
        data: { url: googleMapUrl },
        dataType: 'script',
        success: function (data) {
            eval(data);
            $('#sendToEmail').html('將路線寄送至Email');
            $('#sendToEmail').removeAttr("disabled");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Swal.fire({
                icon: 'error',
                title: textStatus,
                text: errorThrown,
                showConfirmButton: false,
                showCancelButton: true
            });
            $('#sendToEmail').html('將路線寄送至Email');
            $('#sendToEmail').removeAttr("disabled");
        }
    });
}

function resetVariable() {
    googleMapUrl = '';
    //searchCenter = '';
    //$('#positionOrAddr').val('');
    document.getElementById('resultUrl').setAttribute('src', '');
}

/*依據日期時間篩選符合的展覽*/
function searchShowPage() {
    let cityId = $('#FK_City').val();
    let StartDate = $('#StartDate').val();
    let StartTimeStr = document.getElementById('StartTime').value;
    let EndTimeStr = document.getElementById('EndTime').value;
 
    loadingShowPages();

    $.ajax({
        type: 'get',
        url: '/Common/getMyShow',
        data: { cityId: cityId, StartDate: StartDate, StartTime: StartTimeStr, EndTime: EndTimeStr },
        success: function (data) {
            $('#MyShowPage').html(data);
            resetVariable();
        }, error: function (jqXHR, textStatus, errorThrown) {
            Swal.fire({
                icon: 'error',
                title: textStatus,
                text: errorThrown,
                showConfirmButton: false,
                showCancelButton: true
            });
            errorServerMsg();
            resetVariable();
        }
    });
}

//新增展覽至行程排序
function addShowPage(event) {
    let sibling = event.target;

    //取得父元素＝'td'
    do {
        sibling = sibling.parentElement;
    } while (sibling.nodeName != 'TD');

    console.log(sibling.nextElementSibling.getAttribute('aria-showid'));
    let getShowId = sibling.nextElementSibling.getAttribute('aria-showid');
    let checkDuplicateAdd = false;

    $('#sortable>tr>td[aria-showId]').each(function (index, element) {
        if (element.getAttribute('aria-showId') == getShowId) {
            checkDuplicateAdd = true;
        }
    });

    if (checkDuplicateAdd) {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '此展覽已加入至我的行程',
            showConfirmButton: false,
            showCancelButton: true
        });

        return false;
    }

    //複製元素
    sibling = sibling.nextElementSibling.cloneNode(true);

    let trSize = document.querySelectorAll('#sortable>tr').length;

    let thTag = document.createElement('th');
    thTag.textContent = `${trSize + 1}`;
    thTag.setAttribute('scope', 'row');

    let controls = document.createElement('td');
    controls.className = 'btn-group';

    //up
    controls.innerHTML += '<button type="button" class="px-1 controlBtn" onclick="upShowPage(event)"><i class="fas fa-chevron-up"></i></button>';

    //down
    controls.innerHTML += '<button type="button" class="px-1 controlBtn" onclick="downShowPage(event)"><i class="fas fa-chevron-down"></i></button>';

    //top
    controls.innerHTML += '<button type="button" class="px-1 controlBtn" onclick="topShowPage(event)"><i class="fas fa-angle-double-up"></i></button>';

    //last
    controls.innerHTML += '<button type="button" class="px-1 controlBtn" onclick="lastShowPage(event)"><i class="fas fa-angle-double-down"></i></button>';

    //remove
    controls.innerHTML += '<button type="button" class="px-1 controlBtn" onclick="removeShowPage(event)"><i class="fa fa-minus"></i></button>';

    let trTag = document.createElement('tr');
    trTag.appendChild(thTag);//th
    trTag.appendChild(sibling);//td
    trTag.appendChild(controls);//td

    document.getElementById('sortable').appendChild(trTag);
    resetVariable();
}

function upShowPage(event) {
    let target = event.target;
    let parent = document.getElementById('sortable');

    //取得父元素＝'td'
    do {
        target = target.parentElement;
    } while (target.nodeName != 'TR');

    if (target == parent.firstElementChild)
        return;

    let temp = target.cloneNode(true);
    parent.insertBefore(temp, target.previousElementSibling);
    parent.removeChild(target);
    resetVariable();
}

function downShowPage(event) {
    let target = event.target;
    let parent = document.getElementById('sortable');

    //取得父元素＝'td'
    do {
        target = target.parentElement;
    } while (target.nodeName != 'TR');

    if (target == parent.lastElementChild)
        return;

    parent.insertBefore(target.nextElementSibling, target);
    resetVariable();
}

function topShowPage(event) {
    let target = event.target;
    let parent = document.getElementById('sortable');

    //取得父元素＝'td'
    do {
        target = target.parentElement;
    } while (target.nodeName != 'TR');

    if (target == parent.firstElementChild)
        return;

    let temp = target.cloneNode(true);

    parent.replaceChild(parent.firstElementChild, target);
    parent.insertBefore(temp, parent.firstElementChild);
    resetVariable();
}

function lastShowPage(event) {
    let target = event.target;
    let parent = document.getElementById('sortable');

    //取得父元素＝'td'
    do {
        target = target.parentElement;
    } while (target.nodeName != 'TR');

    if (target == parent.lastElementChild)
        return;

    let temp = target.cloneNode(true);

    parent.replaceChild(parent.lastElementChild, target);
    parent.appendChild(temp);
    resetVariable();
}

//從行程排序移除展覽
function removeShowPage(event) {
    let target = event.target;

    //取得父元素＝'td'
    do {
        target = target.parentElement;
    } while (target.nodeName != 'TR');

    let sortable = document.getElementById('sortable');
    sortable.removeChild(target);

    resetVariable();
}

function loadingShowPages() {
    $('#MyShowPage').html('<div class="sk-wave" style="margin:50px auto;"><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div></div>');
}

function errorServerMsg() {
    $('#showPages').html('<br /><br /><h1>伺服器發生錯誤，無法取得資料</h1>');
}