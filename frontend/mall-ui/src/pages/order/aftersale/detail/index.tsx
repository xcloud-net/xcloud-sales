import u from '@/utils';
import { AfterSaleDto } from '@/utils/models';
import { Alert, Box } from '@mui/material';
import { useEffect, useState } from 'react';
import XAction from './action';
import XGoods from './goods';
import XSummary from './summary';
import LinearProgress from '@/components/loading/linear';

export default (props: { detailId?: string }) => {
  const { detailId } = props;
  const [data, _data] = useState<AfterSaleDto>({});
  const [loading, _loading] = useState(false);

  const queryData = () => {
    if (u.isEmpty(detailId)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall/aftersale/by-id', {
        Id: detailId,
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

  useEffect(() => {
    queryData();
  }, [detailId]);

  const renderDetail = () => {
    return (
      <>
        <XAction
          model={data}
          ok={() => {
            queryData();
          }}
        />
        <XSummary model={data} />
        <XGoods model={data} />
      </>
    );
  };

  return (
    <>
      <Box sx={{}}>
        {loading && <LinearProgress />}
        {loading || (
          <>
            {u.isEmpty(data.Id) || renderDetail()}
            {u.isEmpty(data.Id) && (
              <Alert severity="warning">售后数据未关联</Alert>
            )}
          </>
        )}
      </Box>
    </>
  );
};
