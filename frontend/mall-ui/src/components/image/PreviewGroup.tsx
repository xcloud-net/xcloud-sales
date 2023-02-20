import u from '@/utils';
import { MallStorageMetaDto } from '@/utils/models';
import { Image } from 'antd';
import { useState } from 'react';

export default (props: { data: MallStorageMetaDto[]; count?: number }) => {
  const { data, count } = props;
  const [show, _show] = useState(false);

  const metas = data || [];
  const previewCount = count || 3;

  var imgs = u.map(metas, (x) => {
    return {
      big: u.resolveUrlv2(x, {
        width: 900,
      }),
      small: u.resolveUrlv2(x, {
        width: 100,
        height: 100,
      }),
    };
  });

  if (u.isEmpty(imgs)) {
    return null;
  }

  return (
    <>
      <div style={{ display: 'none' }}>
        <Image.PreviewGroup
          preview={{
            visible: show,
            onVisibleChange: (visible) => {
              if (!visible) {
                _show(false);
              }
            },
          }}
        >
          {imgs.map((x, i) => (
            <Image key={i} src={x.big} width={100} height={100} />
          ))}
        </Image.PreviewGroup>
      </div>
      <div>
        {u.take(imgs, previewCount).map((x, i) => {
          return (
            <Image
              key={i}
              src={x.small || ''}
              width={60}
              height={60}
              preview={false}
              onClick={() => {
                _show(true);
              }}
            />
          );
        })}
      </div>
    </>
  );
};
