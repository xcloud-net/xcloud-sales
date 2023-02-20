import u from '@/utils';
import http from '@/utils/http';
import { LoadingButton } from '@mui/lab';
import {
  Box,
  Card,
  CardActions,
  CardContent,
  Container,
  Typography,
  Chip,
} from '@mui/material';
import * as React from 'react';
import { useParams } from 'umi';
import LinearProgress from '@/components/loading/linear';

export default function BasicCard() {
  const [loading, _loading] = React.useState(false);
  const [loadingUse, _loadingUse] = React.useState(false);
  const [data, _data] = React.useState({
    Amount: 0,
    EndTime: '',
    Used: false,
    UserId: 0,
    Expired: false,
  });

  const cardUseable = !(data.Expired || data.Used);

  const params = useParams<any>();
  const id = params.id;

  const getPrepaidCard = (id: string) => {
    _loading(true);
    http.apiRequest
      .post('/mall/prepaid-card/by-id', {
        Id: id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || {});
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const useCard = () => {
    _loadingUse(true);
    http.apiRequest
      .post('/mall/prepaid-card/use-card', {
        CardId: id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          getPrepaidCard(id);
        });
      })
      .finally(() => {
        _loadingUse(false);
      });
  };

  React.useEffect(() => {
    u.isEmpty(id) || getPrepaidCard(id);
  }, []);

  return (
    <>
      <Container maxWidth="sm">
        {loading && (
          <Box sx={{ width: '100%' }}>
            <LinearProgress />
          </Box>
        )}
        <Box sx={{ marginBottom: 1 }}>
          <Card sx={{ minWidth: 275, margin: 1 }}>
            <CardContent>
              <Typography
                variant="h4"
                component={'div'}
                gutterBottom
                sx={{ mb: 2 }}
              >
                预付充值卡
              </Typography>
              <Typography gutterBottom variant="h5" component="div">
                卡面金额：<Chip label={data.Amount} size="small"></Chip>
              </Typography>
              {u.isEmpty(data.EndTime) || (
                <Typography variant="body2">
                  过期时间：{u.formatDateTime(data.EndTime)}
                </Typography>
              )}
              <Typography
                component={'div'}
                variant="overline"
                color="secondary"
              >
                此卡是非记名充值卡，请勿泄露给其他人！否则将造成您的财产损失。
              </Typography>
            </CardContent>
            <CardActions>
              <LoadingButton
                disabled={!cardUseable}
                variant="contained"
                color="error"
                loading={loadingUse}
                size="small"
                fullWidth
                onClick={() => {
                  if (confirm('确定使用此卡？')) {
                    useCard();
                  }
                }}
              >
                {cardUseable ? '使用此卡充值' : '此卡已经失效'}
              </LoadingButton>
            </CardActions>
          </Card>
        </Box>
      </Container>
    </>
  );
}
