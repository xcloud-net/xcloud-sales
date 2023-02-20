import u from '@/utils';
import http from '@/utils/http';
import { Alert, AlertTitle, Box } from '@mui/material';
import * as React from 'react';
import { useParams } from 'umi';
import XAttr from './attr';
import XCombination from './combinations';
import XDescription from './description';
import XImages from './images';
import XLike from './like';
import XNotice from './notice';
import XSummary from './summary';
import { GoodsDto } from '@/utils/models';
import LinearProgress from '@/components/loading/linear';

export default function StandardImageList(props: { goodsId?: number }) {
  const { goodsId } = props;

  const [id, _id] = React.useState(0);
  const [loading, _loading] = React.useState(false);
  const [model, _model] = React.useState<GoodsDto>({});
  const [selectedCombinationId, _selectedCombinationId] = React.useState(0);

  const queryDetail = () => {
    if (id && id > 0) {
      //
    } else {
      return;
    }
    _loading(true);
    http.apiRequest
      .post('/mall/goods/detail', {
        Id: id,
      })
      .then((res) => {
        var m = res.data.Data || {};
        _model(m);
      })
      .finally(() => {
        _loading(false);
      });
  };

  const params = useParams<any>();

  //url entrance
  React.useEffect(() => {
    try {
      console.log(params);
      var queryId = params.id;
      u.isEmpty(queryId) || _id(parseInt(queryId));
    } catch (e) {
      console.log(e);
    }
  }, []);

  //component entrance
  React.useEffect(() => {
    goodsId && goodsId > 0 && _id(goodsId);
  }, [goodsId]);

  React.useEffect(() => {
    queryDetail();
  }, [id]);

  return (
    <>
      <Box sx={{ margin: 0, pb: 2 }}>
        {loading && <LinearProgress />}
        {loading || (
          <>
            {model.Published || (
              <Alert security="error">
                <AlertTitle>商品信息不可用</AlertTitle>
                <p>商品可能被下架或者删除了</p>
              </Alert>
            )}
            {model.Published && (
              <>
                <XImages model={model} combinationId={selectedCombinationId} />
                <Box sx={{ position: 'relative' }}>
                  <XLike
                    model={model}
                    ok={(liked: boolean) => {
                      _model({
                        ...model,
                        IsFavorite: liked,
                      });
                    }}
                  />
                </Box>
                <XSummary model={model} />
                <XNotice />
                <XCombination
                  model={model}
                  onSelect={(combinationId: number) => {
                    _selectedCombinationId(combinationId);
                  }}
                />
                <XAttr model={model} loading={loading} />
                <XDescription model={model} />
              </>
            )}
          </>
        )}
      </Box>
    </>
  );
}
