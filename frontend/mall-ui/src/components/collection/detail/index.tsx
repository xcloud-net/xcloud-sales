import { ApiResponse, GoodsCollectionDto } from '@/utils/models';
import { useEffect, useState } from 'react';
import u from '@/utils';
import XCombinationItem from '@/components/goods/combinationItem';
import { Box, Typography } from '@mui/material';
import XLoading from '@/components/loading/content';

export default ({ detailId }: { detailId?: string }) => {

  const [loading, _loading] = useState(false);
  const [data, _data] = useState<GoodsCollectionDto>({});

  const queryCollection = (id: string) => {
    if (u.isEmpty(id)) {
      return;
    }
    _loading(true);
    u.http.apiRequest.post<ApiResponse<GoodsCollectionDto>>('/mall/collection/by-id', { Id: id }).then(res => {
      u.handleResponse(res, () => {
        _data(res.data.Data || {});
      });
    }).finally(() => {
      _loading(false);
    });
  };

  useEffect(() => {
    detailId && queryCollection(detailId);
  }, [detailId]);

  const renderDetail = () => {
    return <>
      <Box>
        <Typography variant={'h3'} gutterBottom>{data.Name || '--'}</Typography>
        <Typography variant={'subtitle2'} gutterBottom color={'text.disabled'}>{data.Description || '--'}</Typography>
      </Box></>;
  };

  const renderGoods = () => {
    if (u.isEmpty(data.Items)) {
      return <span>此集合未包含商品</span>;
    }

    return data.Items?.map((x, i) => <Box sx={{
      mb: 1,
      mx: 1,
    }} key={i} onClick={() => {
    }}>
      <XCombinationItem model={x.GoodsSpecCombination || {}} />
    </Box>);
  };

  return <>
    {loading && <XLoading />}
    {loading || <>
      <Box sx={{ m: 1 }}>{renderDetail()}</Box>
      <Box sx={{ mb: 1 }}>
        {renderGoods()}
      </Box>
    </>}
  </>;
};
