import personImg from '@/assets/static/images/avatars/avatar_default.jpg';
import { NotificationDto } from '@/utils/models';
import { Avatar, Box, CardActionArea, Typography } from '@mui/material';
import { Badge } from 'antd-mobile';
import u from '@/utils';
import { useState } from 'react';
import XLoading from '@/components/loading/linear';

export default function ({ model, ok }: { model: NotificationDto; ok: any }) {
  const [loading, _loading] = useState(false);
  const markAsRead = () => {
    if (model.Read) {
      return;
    }
    _loading(true);
    u.http.platformRequest
      .post('/user/notification/update-status', { Id: model.Id, Read: true })
      .then((res) => {
        u.handleResponse(res, () => {
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  return (
    <>
      {loading && <XLoading />}
      <CardActionArea
        sx={{
          py: 1,
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'flex-start',
          justifyContent: 'flex-start',
        }}
        onClick={() => {
          markAsRead();
        }}
      >
        <Avatar alt="Profile Picture" src={personImg} />
        <Box
          sx={{
            ml: 1,
            flexGrow: 1,
          }}
        >
          <Typography variant="subtitle2">{model.Title || '--'}</Typography>
          <Typography
            variant="overline"
            sx={{
              color: (t) => t.palette.text.secondary,
            }}
          >
            {model.Content || '--'}
          </Typography>
        </Box>
        <span>
          <Badge
            color={model.Read ? 'gray' : 'red'}
            content={Badge.dot}
          ></Badge>
        </span>
      </CardActionArea>
    </>
  );
}
