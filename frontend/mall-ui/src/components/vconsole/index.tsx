import u from '@/utils';
import { useEffect, useRef } from 'react';
import VConsole from 'vconsole';

export default function SimpleContainer(props: any) {
  const vconsole = useRef<VConsole | null>(null);

  useEffect(() => {
    if (u.config.isDev) {
      vconsole.current = new VConsole();
    }
    return () => {
      vconsole.current && vconsole.current.destroy();
    };
  }, []);

  return <></>;
}
