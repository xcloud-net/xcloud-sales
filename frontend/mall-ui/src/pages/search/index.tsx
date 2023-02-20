import { Box } from '@mui/material';
import { useEffect, useState } from 'react';
import XSearch from './search';
import XTag from './tag';
import XBrand from './brand';
import XCollection from './collection';
import u from '@/utils';

export default () => {
  const [data, _data] = useState<any>({});
  const [loading, _loading] = useState(false);

  const queryView = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall/search/search-view')
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || {});
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryView();
  }, []);

  useEffect(() => {
    const originColor = document.body.style.backgroundColor;
    document.body.style.backgroundColor = 'white';
    return () => {
      document.body.style.backgroundColor = originColor;
    };
  }, []);

  return (
    <>
      <Box sx={{}}>
        <XSearch model={data} />
        <XBrand model={data} />
        <XTag model={data} />
        <XCollection model={data} />
      </Box>
    </>
  );
};
