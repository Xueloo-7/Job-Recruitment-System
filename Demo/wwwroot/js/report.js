<script>
    var applicationsByJob = echarts.init(document.getElementById('applicationsByJob'));
    applicationsByJob.setOption({
        tooltip: {},
        xAxis: { type: 'category', data: @Html.Raw(Json.Encode(ViewBag.JobTitles)) },
        yAxis: { type: 'value' },
        series: [{ data: @Html.Raw(Json.Encode(ViewBag.ApplicationsCount)), type: 'bar' }]
    });

    var sourceChart = echarts.init(document.getElementById('sourceChart'));
    sourceChart.setOption({
        tooltip: { trigger: 'item' },
        series: [{
            type: 'pie',
            radius: '70%',
            data: @Html.Raw(Json.Encode(ViewBag.SourceData))
        }]
    });

    // ===== 平均招聘周期 =====
    var timeToHireChart = echarts.init(document.getElementById('timeToHireChart'));
    timeToHireChart.setOption({
        tooltip: {},
        xAxis: { type: 'category', data: @Html.Raw(Json.Encode(ViewBag.JobTitles)) },
        yAxis: { type: 'value', name: '天' },
        series: [{ data: @Html.Raw(Json.Encode(ViewBag.TimeToHireData)), type: 'bar', color: '#4caf50' }]
    });

    // ===== 招聘漏斗 =====
    var funnelChart = echarts.init(document.getElementById('funnelChart'));
    funnelChart.setOption({
        tooltip: { trigger: 'item' },
        series: [{
            type: 'funnel',
            left: '10%',
            top: 20,
            bottom: 20,
            width: '80%',
            data: @Html.Raw(Json.Encode(ViewBag.FunnelData))
        }]
    });

    // ===== 投递趋势 =====
    var trendChart = echarts.init(document.getElementById('trendChart'));
    trendChart.setOption({
        tooltip: {},
        xAxis: { type: 'category', data: @Html.Raw(Json.Encode(ViewBag.Days)) },
        yAxis: { type: 'value' },
        series: [{ data: @Html.Raw(Json.Encode(ViewBag.DailyApplications)), type: 'line' }]
    });
</script>