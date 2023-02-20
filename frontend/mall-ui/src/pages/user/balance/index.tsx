import XLoadMore from '@/components/infiniteScroller';
import u from '@/utils';
import { Box, Button } from '@mui/material';
import * as React from 'react';
import XBalanceAndPointsMenu from '../ucenter/balance';
import XItem from './item';

export default function AlignItemsList() {
  const [finalQuery, _finalQuery] = React.useState({});
  const [loading, _loading] = React.useState(false);
  const [hasMore, _hasMore] = React.useState(false);
  const [items, _items] = React.useState<any>([]);

  const queryList = (q: any) => {
    if (q.Page && q.Page >= 1) {
    } else {
      q.Page = 1;
    }
    _loading(true);
    return new Promise<void>((resolve, reject) => {
      u.http.apiRequest
        .post('/mall/user/balance-history', {
          ...q,
        })
        .then((res) => {
          var data = res.data.Items || [];
          _hasMore(!u.isEmpty(data));
          _items((x: any) => [...x, ...data]);
          resolve();
        })
        .catch((e) => reject(e))
        .finally(() => {
          _loading(false);
        });
    });
  };

  React.useEffect(() => {
    queryList({ ...finalQuery, first: false });
  }, []);

  return (
    <>
      <XLoadMore
        loading={loading}
        hasMore={hasMore}
        onLoad={async () => {
          var param = {
            Page: 1,
            ...finalQuery,
          };
          param.Page++;
          console.log('load more', param, finalQuery);
          _finalQuery(param);
          await queryList(param);
        }}
      >
        <Box sx={{ my: 2 }}>
          <XBalanceAndPointsMenu />
        </Box>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'flex-end',
          }}
        >
          <Button onClick={() => {}}>预充值</Button>
        </Box>
        <Box sx={{ backgroundColor: 'white' }}>
          {u.map(items, (x, index) => (
            <Box key={index}>
              <XItem model={x} />
            </Box>
          ))}
        </Box>
      </XLoadMore>
    </>
  );
}
