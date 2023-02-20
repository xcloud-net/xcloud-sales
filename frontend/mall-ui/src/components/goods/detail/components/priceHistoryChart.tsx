import { LineChart } from 'echarts/charts';
import { GridComponent, TooltipComponent } from 'echarts/components';
import * as echarts from 'echarts/core';
import { UniversalTransition } from 'echarts/features';
import { CanvasRenderer } from 'echarts/renderers';
import { useEffect, useRef } from 'react';
import u from '@/utils';

echarts.use([
  TooltipComponent,
  GridComponent,
  LineChart,
  CanvasRenderer,
  UniversalTransition,
]);

export default (props: { data: any[] }) => {
  const { data } = props;
  const chart1Ref = useRef<HTMLDivElement>(null);

  const formatDate = (dateStr: string) => {
    try {
      return u.dayjs(dateStr).format(u.dateFormat);
    } catch {
      return dateStr;
    }
  };

  useEffect(() => {
    if (chart1Ref.current == null) {
      return;
    }
    var chart = echarts.init(chart1Ref.current);
    chart.setOption({
      color: ['#80FFA5', '#00DDFF', '#37A2FF', '#FF0087', '#FFBF00'],
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'cross',
          label: {
            backgroundColor: '#6a7985',
          },
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
          boundaryGap: false,
          data: u.map(data, (x) => formatDate(x.Date)),
        },
      ],
      yAxis: [
        {
          type: 'value',
        },
      ],
      series: [
        {
          name: '价格走势💰',
          type: 'line',
          smooth: true,
          lineStyle: {
            width: 0,
          },
          showSymbol: false,
          areaStyle: {
            opacity: 0.8,
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              {
                offset: 0,
                color: 'rgb(255, 0, 135)',
              },
              {
                offset: 1,
                color: 'rgb(135, 0, 157)',
              },
            ]),
          },
          emphasis: {
            focus: 'series',
          },
          data: u.map(data, (x) => x.Price),
        },
      ],
    });
  }, [data]);

  return (
    <>
      <div style={{ height: 200 }} ref={chart1Ref}></div>
    </>
  );
};
