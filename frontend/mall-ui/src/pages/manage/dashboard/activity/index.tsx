import u from '@/utils';
import { Card } from 'antd';
import * as echarts from 'echarts';
import { useEffect, useRef, useState } from 'react';

export default () => {
  const [data, _data] = useState<any[]>([]);
  const [loading, _loading] = useState(false);
  const chart1Ref = useRef<HTMLDivElement>(null);

  const groupByHour = () => {
    var groupedData: any[] = [];
    for (var i = 0; i <= 24; ++i) {
      groupedData.push({
        Hour: i,
        Count: u.sumBy(
          data.filter((x) => x.Hour == i),
          (d) => d.Count,
        ),
      });
    }
    return groupedData;
  };

  const finalData = groupByHour();

  useEffect(() => {
    if (chart1Ref.current == null) {
      return;
    }
    var chart = echarts.init(chart1Ref.current);
    chart.setOption({
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow',
        },
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true,
      },
      xAxis: [
        {
          type: 'category',
          data: u.map(finalData, (x) => x.Hour),
          axisTick: {
            alignWithLabel: true,
          },
        },
      ],
      yAxis: [
        {
          type: 'value',
        },
      ],
      series: [
        {
          name: '活跃度',
          type: 'bar',
          barWidth: '60%',
          data: u.map(finalData, (x) => x.Count),
        },
      ],
    });
  }, [finalData]);

  const queryData = () => {
    _loading(false);
    u.http.apiRequest
      .post('/mall-admin/report/user-activity-by-hour', {})
      .then((res) => {
        u.handleResponse(res, () => {
          var response: any[] = res.data.Data || [];
          _data(
            response.map((x) => ({
              ...x,
              Hour: (x.Hour + u.timezoneOffset) % 24,
            })),
          );
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryData();
  }, []);

  return (
    <>
      <Card
        title="用户活跃时间段"
        style={{ marginBottom: 10 }}
        loading={loading}
      >
        <div style={{ minHeight: 400 }} ref={chart1Ref}></div>
      </Card>
    </>
  );
};
