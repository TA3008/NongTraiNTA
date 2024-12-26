function DrawChartOrderStatus(labels, counts) {
    const CHART_COLORS = [
        'rgb(54, 162, 235)',
        'rgb(255, 99, 132)',
        'rgb(255, 159, 64)',
        'rgb(255, 205, 86)',
        'rgb(75, 192, 192)',
        'rgb(153, 102, 255)'];
    const data = {
        labels: labels,
        datasets: [
            {
                label: 'Tình trạng đơn hàng',
                data: counts,
                backgroundColor: CHART_COLORS
            }
        ]
    };
    const configOrderStatus = {
        type: 'doughnut',
        data: data,
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Đơn hàng theo trạng thái'
                }
            }
        }
    };

    const ctx = document.getElementById('chartOrderStatus');
    new Chart(ctx, configOrderStatus);
}