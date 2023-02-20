import u from '@/utils';
import { InfoOutlined } from '@mui/icons-material';
import {
  Alert,
  AlertTitle,
  Dialog,
  DialogContent,
  DialogTitle,
  IconButton,
  Tooltip as MuiTooltip,
  Typography
} from '@mui/material';
import { useEffect, useState } from 'react';
import XChart from './components/priceHistoryChart';
import LinearProgress from '@/components/loading/linear';

export default (props: any) => {
  const { id } = props;
  const [historyData, _historyData] = useState<any>([]);
  const [loading, _loading] = useState(false);
  const [open, _open] = useState(false);

  const hasNoData =
    u.isEmpty(historyData) || u.every(historyData, (x) => x.NoData);

  const loadData = () => {
    if (id && id > 0) {
    } else {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall/goods/combination-price-history', {
        Id: id,
        LastDays: 30,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _historyData(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    open && loadData();
  }, [open]);

  return (
    <>
      <Dialog
        maxWidth="sm"
        fullWidth
        open={open}
        onClose={() => {
          _open(false);
        }}
      >
        <DialogTitle>近期价格走势</DialogTitle>
        <DialogContent>
          {loading && <LinearProgress />}
          {loading || (
            <>
              {hasNoData || (
                <>
                  <Typography variant="overline" color="secondary">
                    数据基于历史价格变动统计，仅供参考！
                  </Typography>
                  <XChart data={historyData} />
                </>
              )}
              {hasNoData && (
                <Alert severity="info">
                  <AlertTitle>暂无数据</AlertTitle>
                  <p>此商品在过去的一段时间内没有价格调整！ </p>
                </Alert>
              )}
            </>
          )}
        </DialogContent>
      </Dialog>
      <MuiTooltip title="查看历史价格趋势">
        <IconButton
          onClick={() => {
            _open(true);
          }}
        >
          <InfoOutlined />
        </IconButton>
      </MuiTooltip>
    </>
  );
};
