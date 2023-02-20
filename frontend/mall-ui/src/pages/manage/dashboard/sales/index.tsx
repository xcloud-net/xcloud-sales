import u from '@/utils';
import { Card } from 'antd';
import * as echarts from 'echarts';
import { useEffect, useRef, useState } from 'react';

export default () => {
  const [data, _data] = useState<any>([]);
  const [loading, _loading] = useState(false);

  const chart1Ref = useRef<HTMLDivElement>(null);
  const chart2Ref = useRef<HTMLDivElement>(null);

  const queryData = () => {
    _loading(false);
    u.http.apiRequest
      .post('/mall-admin/report/order-sum-by-date', {})
      .then((res) => {
        u.handleResponse(res, () => {
          var response: any[] = res.data.Data || [];
          _data(
            response.map((x) => ({
              ...x,
              formatedDate: u.getDate(x.Date) || x.Date,
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
          data: u.map(data, (x) => x.formatedDate),
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
          name: '订单总数',
          type: 'bar',
          barWidth: '60%',
          data: u.map(data, (x) => x.Total),
        },
      ],
    });
  }, [data]);

  useEffect(() => {
    if (chart2Ref.current == null) {
      return;
    }
    var chart = echarts.init(chart2Ref.current);
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
          data: u.map(data, (x) => x.formatedDate),
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
          name: '销售金额💰',
          type: 'bar',
          barWidth: '60%',
          data: u.map(data, (x) => x.Amount),
        },
      ],
    });
  }, [data]);

  return (
    <>
      <Card title="订单数量统计" style={{ marginBottom: 10 }} loading={loading}>
        <div style={{ minHeight: 400 }} ref={chart1Ref}></div>
      </Card>

      <Card title="订单金额统计" style={{ marginBottom: 10 }} loading={loading}>
        <div style={{ minHeight: 400 }} ref={chart2Ref}></div>
      </Card>
    </>
  );
};
