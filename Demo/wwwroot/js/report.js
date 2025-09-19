let pieChart, lineChart;

function loadCharts(userId, startDate, endDate) {
    $.getJSON('/Report/GetChartMockData', {
        userId: userId,
        start: startDate,
        end: endDate
    }, function (res) {
        // Pie Chart
        const pieCtx = document.getElementById('sourcePieChart').getContext('2d');
        if (pieChart) pieChart.destroy();
        pieChart = new Chart(pieCtx, {
            type: 'pie',
            data: {
                labels: res.pieLabels,
                datasets: [{
                    label: 'Apply Source',
                    data: res.pieData,
                    backgroundColor: ['#36A2EB', '#FF6384', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40', '#C9CBCF'],
                    hoverOffset: 10
                }]
            }
        });

        // Line Chart
        const lineCtx = document.getElementById('applicationsLineChart').getContext('2d');
        if (lineChart) lineChart.destroy();
        lineChart = new Chart(lineCtx, {
            type: 'line',
            data: {
                labels: res.lineLabels,
                datasets: [{
                    label: 'Apply Count',
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
        alert('Please select Start - End Date');
        return;
    }
    loadCharts(id, startDate, endDate);
}

// default show recently one month data
document.addEventListener("DOMContentLoaded", function () {

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
