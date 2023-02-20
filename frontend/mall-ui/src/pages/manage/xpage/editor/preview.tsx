import IPhoneFramePic from '@/assets/static/images/iPhoneX_model.png';
import u from '@/utils';
import { Box } from '@mui/material';
import React, { useEffect } from 'react';

export default function (props: any) {
  const { data } = props;

  const frame = React.useRef<HTMLIFrameElement>(null);

  const postData = () => {
    console.log('post data to preview page', data);
    frame.current?.contentWindow?.postMessage({
      data: data,
    });
  };

  useEffect(() => {
    postData();
  }, [data]);

  const previewPageUrl = u.concatUrl([
    u.config.appBase as string,
    'manage/content/pages/preview',
  ]);

  return (
    <>
      <Box
        sx={{
          backgroundColor: 'white',
          backgroundImage: `url(${IPhoneFramePic})`,
          backgroundSize: 'cover',
          backgroundRepeat: 'no-repeat',
          width: '310px',
          height: '627.87024px',
          borderRadius: '30px',
          position: 'relative',
        }}
      >
        <Box
          sx={{
            backgroundColor: 'rgb(245,245,245)',
            position: 'absolute',
            overflow: 'hidden',
            //overflowY: 'scroll',
            right: 10,
            left: 10,
            bottom: 40,
            top: 45,
          }}
        >
          <iframe
            ref={frame}
            onLoad={() => {
              console.log('preview page onload');
              postData();
            }}
            src={`/${previewPageUrl}`}
            style={{
              border: 'none',
              height: '100%',
              width: '100%',
              margin: 0,
              padding: 0,
            }}
          ></iframe>
        </Box>
      </Box>
    </>
  );
}
