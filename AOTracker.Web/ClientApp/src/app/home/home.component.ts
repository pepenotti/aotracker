import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public servers: ServerData[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<ServerData[]>(baseUrl + 'servers').subscribe(result => {
      result.forEach(r => {
        r.chart = this.getCharOptions(r);
      });

      this.servers = result;
    }, error => console.error(error));
  }

  private getCharOptions(serverData: ServerData) {
    var maxDate = serverData.snapshots.length === 0 ? new Date() : new Date(serverData.snapshots[serverData.snapshots.length - 1].timeStamp);
    var minDate = new Date(maxDate.getTime() - (6 * 60 * 60 * 1000));

    return {
      series: [
        {
          name: "Users",
          data: serverData.snapshots.map(sd => sd.totalUsers)
        }
      ],
      chart: {
        type: "area",
        height: "100px",
        zoom: {
          enabled: false
        },
        toolbar: {
          show: false
        },
      },
      dataLabels: {
        enabled: false
      },
      grid: {
        strokeDashArray: 3
      },
      labels: serverData.snapshots.map(sd => {
        // Fucked up UTC fix
        var str = sd.timeStamp.toString();

        if (!str.endsWith('Z')) {
          str = str.concat('Z');
        }

        return str;
      }),
      xaxis: {
        type: "datetime",
        min: minDate.getTime(),
        max: maxDate.getTime(),
        labels: { 
          datetimeUTC: false,
          format: "HH:mm",
          style: {
            colors: [],
            fontSize: '8px',
            fontFamily: 'Helvetica, Arial, sans-serif',
            fontWeight: 400,
            cssClass: 'apexcharts-xaxis-label',
          },
        },
      },
      yaxis: {
        opposite: true,
        tickAmount: 3,
        labels: {
          style: {
            colors: [],
            fontSize: '8px',
            fontFamily: 'Helvetica, Arial, sans-serif',
            fontWeight: 400,
            cssClass: 'apexcharts-xaxis-label',
          },
          formatter: val => {
            return val.toFixed(0)
          },
        }
      },
      legend: {
        show: false
      },
      tooltip: {
        enabled: true,
        x: {
          format: "HH:mm" 
        }
      }
    };
  }
}

interface ServerDataSnapshot {
  serverId?: number;
  name?: string;
  webUrl?: string;
  usersEndpoint?: string;
  isOnline?: string;
  totalUsers: number;
  timeStamp: Date;
}

interface ServerData {
  name: string;
  webUrl: string;
  totalUsers: number;
  isOnline: boolean;
  snapshots: ServerDataSnapshot[];
  chart: any;
}
