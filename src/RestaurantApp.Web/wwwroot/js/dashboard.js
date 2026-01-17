window.dashboardCharts = {
    renderStatusChart: function (canvasId, data) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.statusChart) window.statusChart.destroy();
        
        window.statusChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: data.labels,
                datasets: [{
                    data: data.values,
                    backgroundColor: [
                        '#f59e0b', // Pending
                        '#3b82f6', // Confirmed
                        '#8b5cf6', // Preparing
                        '#10b981', // Delivered
                        '#ef4444'  // Cancelled
                    ],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    }
                },
                cutout: '70%'
            }
        });
    },
    
    renderRevenueChart: function (canvasId, data) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.revenueChart) window.revenueChart.destroy();
        
        window.revenueChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Revenue',
                    data: data.values,
                    borderColor: '#f57c00',
                    backgroundColor: 'rgba(245, 124, 0, 0.1)',
                    fill: true,
                    tension: 0.4,
                    pointRadius: 4,
                    pointBackgroundColor: '#f57c00'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            display: false
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    },
    
    downloadCSV: function (filename, content) {
        const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
};

window.themeManager = {
    setTheme: function(isDark) {
        if (isDark) {
            document.body.classList.add('dark-mode');
            localStorage.setItem('admin-theme', 'dark');
        } else {
            document.body.classList.remove('dark-mode');
            localStorage.setItem('admin-theme', 'light');
        }
    },
    initTheme: function() {
        const savedTheme = localStorage.getItem('admin-theme');
        if (savedTheme === 'dark') {
            document.body.classList.add('dark-mode');
        }
    }
};
