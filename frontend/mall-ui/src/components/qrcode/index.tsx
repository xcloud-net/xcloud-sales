import { useEffect, useRef } from 'react';
import QRCode from 'qrcode';
import u from '@/utils';

export default (props: { value: string; height?: number; width?: number }) => {
  const { value, height, width } = props;

  const container = useRef<HTMLCanvasElement | null>(null);

  useEffect(() => {
    if (container.current == null || u.isEmpty(value)) {
      return;
    }
    QRCode.toCanvas(container.current, value, { errorCorrectionLevel: 'H' });
  }, [value]);

  return (
    <>
      <canvas
        style={{
          height: height,
          width: width,
        }}
        ref={container}
      ></canvas>
    </>
  );
};
