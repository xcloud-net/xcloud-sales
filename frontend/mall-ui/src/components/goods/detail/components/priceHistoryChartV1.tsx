import u from '@/utils';
import { Chart } from 'chart.js';
import { useEffect, useRef } from 'react';

export default (props: { data: any[] }) => {
  const { data } = props;
  const chart1Ref = useRef<HTMLCanvasElement>(null);

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

    var chart = new Chart(chart1Ref.current, {
      type: 'line',
      data: {
        labels: u.map(data, (x) => formatDate(x.Date)),
        datasets: [
          {
            label: '价格走势💰',
            data: u.map(data, (x) => x.Price),
            borderColor: 'rgb(255, 99, 132)',
            backgroundColor: 'rgb(255, 99, 132)',
            fill: true,
          },
        ],
      },
      options: {
        plugins: {
          filler: {
            propagate: false,
          },
        },
        backgroundColor: '#fff',
        interaction: {
          intersect: false,
        },
      },
    });
  }, [data]);

  return (
    <>
      <canvas style={{ height: 200 }} ref={chart1Ref}></canvas>
    </>
  );
};
