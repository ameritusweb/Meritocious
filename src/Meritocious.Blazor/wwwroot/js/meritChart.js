let chart;

export function initializeChart(canvas, dotNetHelper) {
    const ctx = canvas.getContext('2d');
    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: [],
            datasets: []
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function(context) {
                            return context.dataset.label + ': ' + context.parsed.y.toFixed(2);
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    max: 100,
                    ticks: {
                        callback: function(value) {
                            return value + '%';
                        }
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            },
            elements: {
                line: {
                    tension: 0.4
                },
                point: {
                    radius: 3,
                    hoverRadius: 5
                }
            }
        }
    });
}

export function updateChartData(canvas, labels, datasets) {
    if (chart) {
        chart.data.labels = labels;
        chart.data.datasets = datasets.map(dataset => ({
            ...dataset,
            fill: true,
            tension: 0.4,
            pointRadius: 3,
            pointHoverRadius: 5
        }));
        chart.update();
    }
}

export function dispose() {
    if (chart) {
        chart.destroy();
        chart = null;
    }
}