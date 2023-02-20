import u from '@/utils';
import { Card } from 'antd';
import * as echarts from 'echarts';
import { useEffect, useRef, useState } from 'react';

export default () => {
  const [data, _data] = useState<any>([
    {
      value: 40,
      name: 'Accessibility',
      path: 'Accessibility',
    },
  ]);
  const [loading, _loading] = useState(false);
  const chartRef = useRef<HTMLDivElement>(null);

  const queryData = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/report/grouped-keywords', {})
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryData();
  }, []);

  const chartsData = data.map((x: any) => ({
    value: x.Count,
    name: x.Keywords,
    path: x.Keywords,
  }));

  useEffect(() => {
    if (chartRef.current == null) {
      return;
    }
    var chart = echarts.init(chartRef.current);
    chart.setOption({
      series: [
        {
          name: '关键词',
          type: 'treemap',
          visibleMin: 300,
          label: {
            show: true,
            formatter: '{b}',
          },
          itemStyle: {
            borderColor: '#fff',
          },
          levels: [
            {
              itemStyle: {
                borderWidth: 0,
                gapWidth: 5,
              },
            },
            {
              itemStyle: {
                gapWidth: 1,
              },
            },
            {
              colorSaturation: [0.35, 0.5],
              itemStyle: {
                gapWidth: 1,
                borderColorSaturation: 0.6,
              },
            },
          ],
          data: chartsData,
        },
      ],
    });
  }, [chartsData, chartRef]);

  return (
    <Card title="搜索关键词统计" loading={loading}>
      <div style={{ minHeight: 500, margin: 20 }} ref={chartRef}></div>
    </Card>
  );
};
