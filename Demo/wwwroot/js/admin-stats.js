// wwwroot/js/admin-stats.js

function renderCharts(model) {
    // 用户增长趋势
    new Chart(document.getElementById('userGrowth'), {
        type: 'line',
        data: {
            labels: model.months,
            datasets: [{
                label: 'Total User',
                data: model.userGrowth,
                borderColor: '#2980b9',
                fill: false
            }]
        }
    });

    // 岗位类型分布
    new Chart(document.getElementById('jobType'), {
        type: 'pie',
        data: {
            labels: Object.keys(model.jobCategories),
            datasets: [{
                data: Object.values(model.jobCategories),
                backgroundColor: ['#3498db', '#2ecc71', '#e67e22', '#9b59b6']
            }]
        }
    });

    // 投递趋势
    new Chart(document.getElementById('applicationTrend'), {
        type: 'bar',
        data: {
            labels: model.months,
            datasets: [{
                label: 'Total Application',
                data: model.applications,
                backgroundColor: '#e74c3c'
            }]
        }
    });

    // 收入趋势
    new Chart(document.getElementById('income'), {
        type: 'line',
        data: {
            labels: model.months,
            datasets: [{
                label: 'Business Income',
                data: model.incomes,
                borderColor: '#27ae60',
                backgroundColor: 'rgba(39,174,96,0.2)',
                fill: true
            }]
        }
    });

    // 热门行业排行榜（横向条形图）
    new Chart(document.getElementById('topIndustries'), {
        type: 'bar',
        data: {
            labels: Object.keys(model.topIndustries),   // 行业名
            datasets: [{
                label: 'Total Job',
                data: Object.values(model.topIndustries), // 岗位数
                backgroundColor: '#8e44ad'
            }]
        },
        options: {
            indexAxis: 'y', // 横向
            responsive: true,
            plugins: {
                legend: { display: false },
                title: {
                    display: true,
                    text: 'Top Industries'
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: { precision: 0 }
                }
            }
        }
    });
}
