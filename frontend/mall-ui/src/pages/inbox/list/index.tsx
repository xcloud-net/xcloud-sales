import XEmpty from '@/components/empty';
import XLoadMore from '@/components/infiniteScroller';
import u from '@/utils';
import http from '@/utils/http';
import { NotificationDto } from '@/utils/models';
import { ClearAllOutlined } from '@mui/icons-material';
import { Box, Button, Container, Paper, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import XItem from './item';

export default function () {
  const [items, _items] = useState<NotificationDto[]>([]);
  const [more, _more] = useState(false);
  const [loading, _loading] = useState(false);
  const [q, _q] = useState({
    initial: true,
    Page: 1,
  });

  const queryPaging = (param: any) => {
    if (param.initial) {
      return;
    }
    param.Page = param.Page || 1;

    _loading(true);
    return new Promise<void>((resolve, reject) => {
      http.platformRequest
        .post('/user/notification/paging', {
          ...param,
        })
        .then((res) => {
          var data = res.data.Items || [];
          _more(!u.isEmpty(data));
          _items([...items, ...data]);
          resolve();
        })
        .catch((e) => reject(e))
        .finally(() => {
          _loading(false);
        });
    });
  };

  useEffect(() => {
    var param = {
      initial: false,
      Page: 1,
    };
    _q(param);
    queryPaging(param);
  }, []);

  const groupedData = u.groupBy(items, (x) =>
    u.getDate(x.CreationTime || u.now()),
  );
  const keys = u.sortBy(Object.keys(groupedData), (x) => u.getTimeStamp(x));

  if (u.isEmpty(keys)) {
    return <XEmpty />;
  }

  return (
    <>
      <Container disableGutters maxWidth="sm">
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'flex-end',
            mb: 1,
          }}
        >
          <Button
            startIcon={<ClearAllOutlined />}
            variant="text"
            onClick={() => {
              alert('todo');
            }}
            sx={{
              color: 'black',
            }}
          >
            清空
          </Button>
        </Box>
        <Paper square sx={{ py: 1 }}>
          <XLoadMore
            loading={loading}
            hasMore={more}
            onLoad={async () => {
              var param = {
                ...q,
                Page: q.Page + 1,
                initial: false,
              };
              _q(param);
              await queryPaging(param);
            }}
          >
            {u.map(keys, (x: string, index: number) => {
              var group = groupedData[x];
              if (u.isEmpty(group)) {
                return null;
              }

              return (
                <Box key={`group-${index}`} sx={{ px: 1 }}>
                  <Typography
                    component={'div'}
                    variant="button"
                    gutterBottom
                    sx={{
                      color: (theme) => theme.palette.text.secondary,
                    }}
                  >
                    {u.dateTimeFromNow(x)}
                  </Typography>
                  {u.map(group, (item, i) => {
                    return (
                      <Box key={`list-item-${i}`}>
                        <XItem
                          model={item}
                          ok={() => {
                            _items([
                              ...items.filter((x) => x.Id != item.Id),
                              {
                                ...item,
                                Read: true,
                              },
                            ]);
                          }}
                        />
                      </Box>
                    );
                  })}
                </Box>
              );
            })}
          </XLoadMore>
        </Paper>
      </Container>
    </>
  );
}
