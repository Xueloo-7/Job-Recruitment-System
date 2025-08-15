let pieChart, lineChart;

function loadCharts(userId, startDate, endDate) {
    $.getJSON('/Report/GetChartData', {
        userId: userId,
        start: startDate,
        end: endDate
    }, function (res) {
        // 饼图
        const pieCtx = document.getElementById('sourcePieChart').getContext('2d');
        if (pieChart) pieChart.destroy();
        pieChart = new Chart(pieCtx, {
            type: 'pie',
            data: {
                labels: res.pieLabels,
                datasets: [{
                    label: '投递来源',
                    data: res.pieData,
                    backgroundColor: ['#36A2EB', '#FF6384', '#FFCE56', '#4BC0C0', '#9966FF'],
                    hoverOffset: 10
                }]
            }
        });

        // 折线图
        const lineCtx = document.getElementById('applicationsLineChart').getContext('2d');
        if (lineChart) lineChart.destroy();
        lineChart = new Chart(lineCtx, {
            type: 'line',
            data: {
                labels: res.lineLabels,
                datasets: [{
                    label: '申请人数',
                    data: res.lineData,
                    fill: false,
                    borderColor: '#36A2EB',
                    tension: 0.3
                }]
            }
        });
    });
}

function updateChart(id) {
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;
    if (!startDate || !endDate) {
        alert('请选择开始和结束日期');
        return;
    }
    loadCharts(id, startDate, endDate);
}

// 页面加载默认显示最近 7 天
document.addEventListener("DOMContentLoaded", function () {
    // bind button
    const updateBtn = document.getElementById('updateBtn');
    if (updateBtn) {
        updateBtn.addEventListener('click', function () {
            updateChart(id);
        });
    }

    const today = new Date();
    const lastMonth = new Date();
    lastMonth.setMonth(today.getMonth() - 1);

    document.getElementById('startDate').value = lastMonth.toISOString().slice(0, 10);
    document.getElementById('endDate').value = today.toISOString().slice(0, 10);


    loadCharts(id, lastMonth.toISOString().slice(0, 10), today.toISOString().slice(0, 10));
});
