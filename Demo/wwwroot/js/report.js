// 饼图数据（投递来源占比）
var pieCtx = document.getElementById('sourcePieChart').getContext('2d');
var sourcePieChart = new Chart(pieCtx, {
    type: 'pie',
    data: {
        labels: ['LinkedIn', 'Google Ads', 'Indeed', '招聘会', '内推'],
        datasets: [{
            label: '投递来源',
            data: [50, 10, 20, 10, 10],
            backgroundColor: ['#36A2EB', '#FF6384', '#FFCE56', '#4BC0C0', '#9966FF'],
            hoverOffset: 10
        }]
    }
});

// 折线图数据（每日投递趋势）
var lineCtx = document.getElementById('applicationsLineChart').getContext('2d');
var applicationsLineChart = new Chart(lineCtx, {
    type: 'line',
    data: {
        labels: ['8月1日', '8月2日', '8月3日', '8月4日', '8月5日', '8月6日', '8月7日'],
        datasets: [{
            label: '申请人数',
            data: [5, 10, 8, 15, 12, 18, 20],
            fill: false,
            borderColor: '#36A2EB',
            tension: 0.3
        }]
    },
    options: {
        responsive: true,
        plugins: {
            legend: { display: true }
        }
    }
});