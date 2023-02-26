import { useEffect, useState } from 'react';
import { AfterSaleDto, AfterSalesCommentDto, PagedResponse } from '@/utils/models';
import u from '@/utils';
import XItem from './item';
import XForm from './form';
import { Box } from '@mui/material';

export default ({ model }: { model: AfterSaleDto }) => {

  const [loading, _loading] = useState(false);
  const [data, _data] = useState<AfterSalesCommentDto[]>([]);
  const [query, _query] = useState({
    Page: 1,
  });

  const queryComment = (model: AfterSaleDto) => {
    _loading(true);
    u.http.apiRequest.post<PagedResponse<AfterSalesCommentDto>>('/mall/aftersale/comment-paging', {
      ...query,
      PageSize: 10,
      AfterSalesId: model.Id,
    }).then(res => {
      u.handleResponse(res, () => {
        _data(res.data.Items || []);
      });
    }).finally(() => {
      _loading(false);
    });
  };

  useEffect(() => {
    model && model.Id && queryComment(model);
  }, [model]);

  return <>
    {loading && <div>loading...</div>}
    {loading || data.map((x, i) => <div key={i}><XItem model={x} /></div>)}
    <Box sx={{ my: 1 }}>
      <XForm model={model} />
    </Box>
  </>;
};
