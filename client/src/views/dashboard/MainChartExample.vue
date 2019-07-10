<script>
import { Line, Bar, mixins } from "vue-chartjs";
import { getStyle, hexToRgba } from "@coreui/coreui/dist/js/coreui-utilities";
import { CustomTooltips } from "@coreui/coreui-plugin-chartjs-custom-tooltips";
import { random } from "@/shared/utils";
const { reactiveProp } = mixins;

export default {
  extends: Line,
  mixins: [reactiveProp],
  props: ["height", "chartData"],
  mounted() {
    this.renderThisChart();
  },
  computed: {
    computedChartData: function() {
      return this.chartData;
    }
  },
  methods: {
    renderThisChart: function() {
      let options = {
        tooltips: {
          enabled: false,
          custom: CustomTooltips,
          intersect: true,
          mode: "index",
          position: "nearest",
          callbacks: {
            labelColor: function(tooltipItem, chart) {
              return {
                backgroundColor:
                  chart.data.datasets[tooltipItem.datasetIndex].borderColor
              };
            }
          }
        },
        maintainAspectRatio: false,
        legend: {
          display: false
        },
        scales: {
          xAxes: [
            {
              gridLines: {
                drawOnChartArea: false
              }
            }
          ],
          yAxes: [
            {
              ticks: {
                beginAtZero: true,
                maxTicksLimit: 5,
                stepSize: Math.ceil(250 / 5),
                max: 250
              },
              gridLines: {
                display: true
              }
            }
          ]
        },
        elements: {
          point: {
            radius: 0,
            hitRadius: 10,
            hoverRadius: 4,
            hoverBorderWidth: 3
          }
        }
      };

      let cData = {
        labels: [
          "7am",
          "8am",
          "9am",
          "10am",
          "11am",
          "12pm",
          "1pm",
          "2pm",
          "3pm",
          "4pm",
          "5pm"
        ],
        datasets: [
          {
            label: "My First dataset",
            backgroundColor: "#44f",
            borderColor: "#f33",
            pointHoverBackgroundColor: "#fff",
            borderWidth: 2,
            data: [12, 22, 23, 34]
          },
          {
            label: "My Second dataset",
            backgroundColor: "transparent",
            borderColor: "#f35",
            pointHoverBackgroundColor: "#fff",
            borderWidth: 2,
            data: this.computedChartData
          }
        ]
      };

      this.renderChart(this.computedChartData, this.options, {
        responsive: true,
        maintainAspectRatio: false
      });
    }
  }
};
</script>
