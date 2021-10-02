//CANVAS繪圖放置區
//折線圖
function lineChart(context, fillmode, point, curve, showline, label) {
    var chart = new Chart(context, {
      type: "line",
      data: {
        labels: ['1月', '2月', '3月', '4月', '5月', '6月'],
        datasets: [{
          label: label,
          data: [16, 20, 50, 100, 115, 200],
          fill: fillmode,
          pointStyle: point,
          lineTension: curve,
          showLine: showline,
          pointRadius: 4,
          pointHoverRadius: 8,
          backgroundColor: 'rgba(85,165,250,0.3)',
          borderColor: 'rgb(0,123,255)',
          pointBackgroundColor: 'rgb(255,0,0)',
          pointBorderColor: 'rgb(0,0,255)'
        },{
            label: label,
            data: [163, 210, 50, 10, 11, 20],
            fill: fillmode,
            pointStyle: point,
            lineTension: curve,
            showLine: showline,
            pointRadius: 4,
            pointHoverRadius: 8,
            backgroundColor: 'rgba(85,255,250,0.3)',
            borderColor: 'rgb(0,255,255)',
            pointBackgroundColor: 'rgb(255,255,0)',
            pointBorderColor: 'rgb(0,255,255)'
          }]
      },
      options: {
        responsive: true,
        title: {
          display: true,
          fontSize: 26,
          text: label
        },
        tooltips: {
          mode: 'point',
          intersect: true
        },
        legend: {
          position: 'none'
        }
      }
    });
  }



  //bar圖
function bar(context, fillmode, point, curve, showline, label) {
    var chart = new Chart(context, {
      type: "bar",
      data: {
        labels: ['1月', '2月', '3月', '4月', '5月', '6月'],
        datasets: [{
          label: label,
          data: [16, 20, 50, 100, 115, 200],
          fill: fillmode,
          pointStyle: point,
          lineTension: curve,
          showLine: showline,
          pointRadius: 4,
          pointHoverRadius: 8,
          backgroundColor: 'rgba(85,165,250,0.3)',
          borderColor: 'rgb(0,123,255)',
          pointBackgroundColor: 'rgb(255,0,0)',
          pointBorderColor: 'rgb(0,0,255)'
        }]
      },
      options: {
        responsive: true,
        title: {
          display: true,
          fontSize: 26,
          text: label
        },
        tooltips: {
          mode: 'point',
          intersect: true
        },
        legend: {
          position: 'none'
        }
      }
    });
  }