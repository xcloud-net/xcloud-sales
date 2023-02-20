import XPage from '@/components/xpage';
import { useEffect, useState } from 'react';

export default function (props: any) {
  const [data, _data] = useState({});

  const handler = (event: any) => {
    console.log(event);
    event?.data?.data && _data(event.data.data);
  };

  // regular way
  useEffect(() => {
    window.addEventListener('message', handler);
    return () => window.removeEventListener('message', handler);
  }, []);

  return (
    <>
      <XPage data={data} />
    </>
  );
}
