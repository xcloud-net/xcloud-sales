import u from '@/utils';
import { Card } from 'antd';
import * as echarts from 'echarts';
import { useEffect, useRef, useState } from 'react';

export default () => {
  const [data, _data] = useState<any[]>([]);
  const [loading, _loading] = useState(false);

  const chartRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!chartRef.current) {
      return;
    }
    var chart = echarts.init(chartRef.current);
    chart.setOption({
      legend: {
        top: 'bottom',
      },
      toolbox: {
        show: true,
        feature: {
          mark: { show: true },
          dataView: { show: true, readOnly: false },
          restore: { show: true },
          saveAsImage: { show: true },
        },
      },
      series: [
        {
          name: '城市分布',
          type: 'pie',
          radius: [50, 250],
          center: ['50%', '50%'],
          roseType: 'area',
          itemStyle: {
            borderRadius: 8,
          },
          data: [
            ...u.map(data, (x) => ({
              name: `${x.Country}-${x.City}`,
              value: x.Count,
            })),
          ],
        },
      ],
    });
  }, [data]);

  const queryData = () => {
    _loading(false);
    u.http.apiRequest
      .post('/mall-admin/report/user-activity-by-geo-location', {})
      .then((res) => {
        u.handleResponse(res, () => {
          var response: any[] = res.data.Data || [];
          _data(response);
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
        <div style={{ minHeight: 500, margin: 20 }} ref={chartRef}></div>
      </Card>
    </>
  );
};
